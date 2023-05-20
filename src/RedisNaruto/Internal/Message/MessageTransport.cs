using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using Microsoft.IO;
using RedisNaruto.Exceptions;
using RedisNaruto.Internal.Models;
using RedisNaruto.Internal.Serialization;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.Internal.Message;

/// <summary>
/// 消息传输
/// </summary>
internal class MessageTransport : IMessageTransport
{
    /// <summary>
    /// 池
    /// </summary>
    protected static readonly RecyclableMemoryStreamManager MemoryStreamManager = new();

    /// <summary>
    /// 换行
    /// </summary>
    protected static readonly byte[] NewLine = "\r\n".ToEncode();

    protected static readonly byte CR = (byte) '\r';


    protected static readonly byte LF = (byte) '\n';

    /// <summary>
    /// 序列化
    /// </summary>
    protected static readonly ISerializer Serializer = new Serializer();

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="command"></param>
    public virtual async Task SendAsync(Stream stream, Command command)
    {
        await using var ms = MemoryStreamManager.GetStream();
        ms.Position = 0;
        await ms.WriteAsync($"{RespMessage.ArrayString}{command.Length}".ToEncode());
        await ms.WriteAsync(NewLine);
        //写入命令
        var cmdBytes = command.Cmd.ToEncode();
        await ms.WriteAsync($"{RespMessage.BulkStrings}{cmdBytes.Length}".ToEncode());
        await ms.WriteAsync(NewLine);
        await ms.WriteAsync(cmdBytes);
        await ms.WriteAsync(NewLine);
        if (command.Length > 1)
        {
            //判断参数长度
            foreach (var item in command.Args)
            {
                //处理null
                if (item is null)
                {
                    await ms.WriteAsync($"{RespMessage.BulkStrings}0".ToEncode());
                    await ms.WriteAsync(NewLine);
                    await ms.WriteAsync(NewLine);
                    continue;
                }

                if (item is not byte[] argBytes)
                {
                    var bytes = await Serializer.SerializeAsync(item);

                    await ms.WriteAsync($"{RespMessage.BulkStrings}{bytes.Item2}".ToEncode());
                    await ms.WriteAsync(NewLine);
                    if (bytes.Item2 == 0)
                    {
                        await ms.WriteAsync(bytes.Item1);
                    }
                    else
                    {
                        await ms.WriteAsync(bytes.Item1, 0, bytes.Item2);
                        ArrayPool<byte>.Shared.Return(bytes.Item1);
                    }

                    await ms.WriteAsync(NewLine);
                    continue;
                }

                await ms.WriteAsync($"{RespMessage.BulkStrings}{argBytes.Length}".ToEncode());
                await ms.WriteAsync(NewLine);
                await ms.WriteAsync(argBytes);
                await ms.WriteAsync(NewLine);
            }
        }

        ms.Position = 0;
        await ms.CopyToAsync(stream);
    }

    /// <summary>
    /// 转换消息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public async Task<object> ReceiveMessageAsync(Stream stream)
    {
        //获取首位的 符号 判断消息回复类型
        var head = ReadFirstChar(stream);
        switch (head)
        {
            case RespMessage.SimpleString:
            case RespMessage.Number:
            {
                return ReadLine(stream);
            }
            //数组
            case RespMessage.ArrayString:
            {
                var result = await ReadMLineAsync(stream, ReadLine(stream));
                return result;
            }
            case RespMessage.BulkStrings:
            {
                int offset = ReadLine(stream);
                //如果为null
                if (offset == -1)
                {
                    return RedisValue.Null();
                }

                var result = await ReadBlobLineAsync(stream, offset);

                return new RedisValue(result);
            }
            default:
            {
                //错误
                throw new RedisExecException(ReadLine(stream).ToString());
            }
        }
    }

    /// <summary>
    /// 读取简易消息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="RedisExecException"></exception>
    public async Task<RedisValue> ReceiveSimpleMessageAsync(Stream stream)
    {
        //获取首位的 符号 判断消息回复类型
        var head = ReadFirstChar(stream);
        switch (head)
        {
            case RespMessage.SimpleString:
            case RespMessage.Number:
            {
                return ReadLine(stream);
            }
            //数组
            case RespMessage.ArrayString:
            {
                throw new NotSupportedException(nameof(RespMessage.ArrayString));
            }
            case RespMessage.BulkStrings:
            {
                int offset = ReadLine(stream);
                //如果为null
                if (offset == -1)
                {
                    return RedisValue.Null();
                }

                var result = await ReadBlobLineAsync(stream, offset);

                return new RedisValue(result);
            }
            default:
            {
                Console.WriteLine("输出：" + head.ToString().ToInt());
                //错误
                throw new RedisExecException(ReadLine(stream).ToString());
            }
        }
    }

    /// <summary>
    /// 多行读取
    /// </summary>
    /// <param name="pipeReader"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    private static async Task<List<object>> ReadMLineAsync(Stream stream, int length)
    {
        //读取数组的长度
        if (length == -1)
        {
            return default;
        }

        List<object> resultList = new();
        for (var i = 0; i < length; i++)
        {
            //获取 符号 判断消息类型 是字符串还是 数字 
            var head = ReadFirstChar(stream);
            switch (head)
            {
                case RespMessage.SimpleString:
                case RespMessage.Number:
                {
                    //读取剩下的消息
                    resultList.Add(ReadLine(stream));
                    break;
                }
                case RespMessage.BulkStrings:
                {
                    //获取字符串的长度
                    int offset = ReadLine(stream);
                    //如果为null
                    if (offset == -1)
                    {
                        resultList.Add(RedisValue.Null());
                        break;
                    }

                    //读取结果
                    var result = await ReadBlobLineAsync(stream, offset);
                    resultList.Add(new RedisValue(result));
                    break;
                }
                //数组
                case RespMessage.ArrayString:
                {
                    //读取剩下的消息
                    var result = await ReadMLineAsync(stream, ReadLine(stream));
                    resultList.Add(result);
                    break;
                }
                case RespMessage.Error:
                {
                    //todo 错误消息
                    resultList.Add(ReadLine(stream));
                    break;
                }
            }
        }

        return resultList;
    }

    /// <summary>
    /// 读取指定长度数据
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="length">长度</param>
    /// <returns></returns>
    private static async Task<ReadOnlyMemory<byte>> ReadBlobLineAsync(Stream stream, int length)
    {
        //从内存池中租借
        await using var ms = MemoryStreamManager.GetStream();
        ms.Position = 0;
        var totalLength = 0;
        while (true)
        {
            var bytes2 = ArrayPool<byte>.Shared.Rent(length);
            try
            {
                var mem = await stream.ReadAsync(bytes2, 0, length - totalLength);
                await ms.WriteAsync(bytes2[..mem]);
                totalLength += mem;
                //读取
                if (totalLength == length)
                {
                    break;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes2);
            }
        }

        //读取换行信息
        await ReadCrlfAsync(stream);
        //获取真实的数据
        return new ReadOnlyMemory<byte>(ms.ToArray());
    }

    /// <summary>
    /// 读取换行
    /// </summary>
    /// <param name="stream"></param>
    private static async Task ReadCrlfAsync(Stream stream)
    {
        var crlfByte = ArrayPool<byte>.Shared.Rent(2);
        _ = await stream.ReadAsync(crlfByte, 0, 2);
        ArrayPool<byte>.Shared.Return(crlfByte);
    }

    /// <summary>
    /// 读取第一行的字节信息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private static char ReadFirstChar(Stream stream) => (char) stream.ReadByte();

    /// <summary>
    /// 读取行数据
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private static RedisValue ReadLine(Stream stream)
    {
        //从内存池中租借
        using var ms = MemoryStreamManager.GetStream();
        ms.Position = 0;
        while (true)
        {
            var msg = stream.ReadByte();
            if (msg < 0) break;
            //判断是否为换行 \r\n
            if (msg == CR)
            {
                var msg2 = stream.ReadByte();
                if (msg2 < 0) break;
                if (msg2 == LF) break;
                ms.WriteByte((byte) msg);
                ms.WriteByte((byte) msg2);
            }
            else
                ms.WriteByte((byte) msg);
        }

        return new RedisValue(ms.ToArray());
    }
}