using System.Buffers;
using System.Text;
using Microsoft.IO;
using RedisNaruto.Exceptions;
using RedisNaruto.Internal.Serialization;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.Internal.Message;

/// <summary>
/// 消息传输
/// </summary>
internal sealed class MessageTransport : IMessageTransport
{
    /// <summary>
    /// 池
    /// </summary>
    private static readonly RecyclableMemoryStreamManager MemoryStreamManager = new();

    /// <summary>
    /// 换行
    /// </summary>
    private static readonly byte[] NewLine = "\r\n".ToEncode();

    /// <summary>
    /// 序列化
    /// </summary>
    private readonly ISerializer _serializer = new Serializer();

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public async Task<object> ReciveAsync(Stream stream)
    {
        //获取首位的 符号 判断消息回复类型
        var bytes = new byte[1];
        _ = await stream.ReadAsync(bytes);
        var head = (char) bytes[0];
        switch (head)
        {
            case RespMessage.SimpleString:
            case RespMessage.Number:
            {
                var result = ReadLine(stream);
                return result;
            }
            //数组
            case RespMessage.ArrayString:
            {
                var result = await ReadMLineAsync(stream);
                return result;
            }
            case RespMessage.BulkStrings:
            {
                var strlen = ReadLine(stream);
                //如果为null
                if (strlen == "-1")
                {
                    return default;
                }

                var result = ReadLine(stream);
                return result;
            }
            default:
            {
                //错误
                var result = ReadLine(stream);
                throw new RedisExecException(result.ToString());
            }
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="args"></param>
    public async Task SendAsync(Stream stream, object[] args)
    {
        if (args is not {Length: > 0})
        {
            return;
        }

        await using var ms = MemoryStreamManager.GetStream();
        ms.Position = 0;
        await ms.WriteAsync(await _serializer.SerializeAsync($"{RespMessage.ArrayString}{args.Length}"));
        await ms.WriteAsync(NewLine);
        //判断参数长度
        foreach (var item in args)
        {
            if (item is byte[] argBytes)
            {
            }
            else
            {
                argBytes = await _serializer.SerializeAsync(item);
            }

            await ms.WriteAsync(await _serializer.SerializeAsync($"{RespMessage.BulkStrings}{argBytes.Length}"));
            await ms.WriteAsync(NewLine);
            await ms.WriteAsync(argBytes);
            await ms.WriteAsync(NewLine);
        }

        ms.Position = 0;
        await ms.CopyToAsync(stream);
    }

    /// <summary>
    /// 读取行数据
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private RedisValue ReadLine(Stream stream)
    {
        var bytes = new List<byte>();
        while (true)
        {
            var msg = stream.ReadByte();
            if (msg < 0) break;
            //判断是否为换行 \r\n
            if (msg == '\r')
            {
                var msg2 = stream.ReadByte();
                if (msg2 < 0) break;
                if (msg2 == '\n') break;
                // stringBuilder.Append((char) msg);
                bytes.Add((byte) msg);
                // stringBuilder.Append((char) msg2);
                bytes.Add((byte) msg2);
            }
            else
                // stringBuilder.Append((char) msg);
                bytes.Add((byte) msg);
        }

        return new RedisValue(bytes.ToArray());
    }

    /// <summary>
    /// 多行读取
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private async Task<List<Object>> ReadMLineAsync(Stream stream)
    {
        List<Object> resultList = new();

        //读取数组的长度
        var length = ReadLine(stream).ToInt();
        for (var i = 0; i < length; i++)
        {
            //获取 符号 判断消息类型 是字符串还是 数字 
            var bytes = new byte[1];
            _ = await stream.ReadAsync(bytes);
            var head = (char) bytes[0];
            switch (head)
            {
                case RespMessage.SimpleString:
                {
                    var result = ReadLine(stream);
                    resultList.Add(result);
                    break;
                }
                case RespMessage.Number:
                {
                    var result = ReadLine(stream).ToLong();
                    resultList.Add(result);
                    break;
                }
                case RespMessage.BulkStrings:
                {
                    //去除第一位的长度
                    var strlen = ReadLine(stream);
                    //如果为null
                    if (strlen == "-1")
                    {
                        resultList.Add(null);
                        break;
                    }

                    //读取结果
                    var result = ReadLine(stream);
                    resultList.Add(result);
                    break;
                }
                //数组
                case RespMessage.ArrayString:
                {
                    var result = await ReadMLineAsync(stream);
                    resultList.Add(result);
                    break;
                }
            }
        }

        return resultList;
    }
}