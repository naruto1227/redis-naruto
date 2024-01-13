using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using Microsoft.IO;
using RedisNaruto.Exceptions;
using RedisNaruto.Internal.Models;
using RedisNaruto.Internal.Serialization;
using RedisNaruto.Models;
using RedisNaruto.Utils;
using System;
using RedisNaruto.Internal.Enums;

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
    /// 空
    /// </summary>
    protected static readonly byte[] Nil = $"{RespMessage.BulkStrings}0".ToEncode();

    /// <summary>
    /// 序列化
    /// </summary>
    protected static readonly ISerializer Serializer = new Serializer();

    /// <summary>
    /// 发送消息
    ///使用MemoryStream 进行消息的缓冲再发送优点，一 是为了当数据过大进行分块处理，二 利于扩展，如果进行二次修改的话
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="command"></param>
    public virtual async Task SendWithMemoryAsync(Stream stream, Command command)
    {
        await using var ms = MemoryStreamManager.GetStream();
        ms.Position = 0;
        using (var encode1 = $"{RespMessage.ArrayString}{command.Length}".ToEncodePool())
        {
            await ms.WriteAsync(encode1.Bytes.AsMemory(0, encode1.Length));
        }

        await ms.WriteAsync(NewLine);
        //写入命令
        using (var encode1 = command.Cmd.ToEncodePool())
        {
            await ms.WriteAsync($"{RespMessage.BulkStrings}{encode1.Length}".ToEncode());
            await ms.WriteAsync(NewLine);
            await ms.WriteAsync(encode1.Bytes.AsMemory(0, encode1.Length));
        }

        await ms.WriteAsync(NewLine);
        if (command.Length > 1)
        {
            //判断参数长度
            foreach (var item in command.Args)
            {
                //处理null
                if (item is null)
                {
                    await ms.WriteAsync(Nil);
                    await ms.WriteAsync(NewLine);
                    await ms.WriteAsync(NewLine);
                    continue;
                }

                if (item is not byte[] argBytes)
                {
                    using (var encode = await Serializer.SerializeAsync(item))
                    {
                        await ms.WriteAsync($"{RespMessage.BulkStrings}{encode.Length}".ToEncode());
                        await ms.WriteAsync(NewLine);
                        await ms.WriteAsync(encode.Bytes.AsMemory(0, encode.Length));
                    }

                    await ms.WriteAsync(NewLine);
                    continue;
                }

                using (var encode1 = $"{RespMessage.BulkStrings}{argBytes.Length}".ToEncodePool())
                {
                    await ms.WriteAsync(encode1.Bytes.AsMemory(0, encode1.Length));
                }

                await ms.WriteAsync(NewLine);
                await ms.WriteAsync(argBytes);
                await ms.WriteAsync(NewLine);
            }
        }

        ms.Position = 0;
        await ms.CopyToAsync(stream);
    }


    /// <summary>
    ///发送消息 直接往网络中写入
    ///优点 减少额外的内存分配
    ///直接使用NetworkStream 进行发送，原因一 如果消息过大，IP层会自动进行分块处理， 二 不需要二次进行消息的修改 。 综上所述 目前不太需要 使用  MemoryStream增加额外的一层 缓冲
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="command"></param>
    public virtual async Task SendAsync(Stream stream, Command command)
    {
        using (var encode1 = $"{RespMessage.ArrayString}{command.Length}".ToEncodePool())
        {
            await stream.WriteAsync(encode1.Bytes.AsMemory(0, encode1.Length));
        }

        await stream.WriteAsync(NewLine);
        //写入命令
        using (var encode1 = command.Cmd.ToEncodePool())
        {
            await stream.WriteAsync($"{RespMessage.BulkStrings}{encode1.Length}".ToEncode());
            await stream.WriteAsync(NewLine);
            await stream.WriteAsync(encode1.Bytes.AsMemory(0, encode1.Length));
        }

        await stream.WriteAsync(NewLine);
        if (command.Length > 1)
        {
            //判断参数长度
            foreach (var item in command.Args)
            {
                //处理null
                if (item is null)
                {
                    await stream.WriteAsync(Nil);
                    await stream.WriteAsync(NewLine);
                    await stream.WriteAsync(NewLine);
                    continue;
                }

                if (item is not byte[] argBytes)
                {
                    using (var encode = await Serializer.SerializeAsync(item))
                    {
                        await stream.WriteAsync($"{RespMessage.BulkStrings}{encode.Length}".ToEncode());
                        await stream.WriteAsync(NewLine);
                        await stream.WriteAsync(encode.Bytes.AsMemory(0, encode.Length));
                    }

                    await stream.WriteAsync(NewLine);
                    continue;
                }

                using (var encode1 = $"{RespMessage.BulkStrings}{argBytes.Length}".ToEncodePool())
                {
                    await stream.WriteAsync(encode1.Bytes.AsMemory(0, encode1.Length));
                }

                await stream.WriteAsync(NewLine);
                await stream.WriteAsync(argBytes);
                await stream.WriteAsync(NewLine);
            }
        }
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
                return ReadLine(stream,RespMessageTypeEnum.Number);
            }
            //数组
            case RespMessage.ArrayString:
            {
                var result = await ReadMLineAsync(stream, ReadLine(stream,RespMessageTypeEnum.ArrayString));
                return result;
            }
            case RespMessage.BulkStrings:
            {
                int offset = ReadLine(stream,RespMessageTypeEnum.Number);
                //如果为null
                if (offset == -1)
                {
                    return RedisValue.Null();
                }

                var result = await ReadBlobLineAsync(stream, offset);

                return new RedisValue(result,RespMessageTypeEnum.BulkStrings);
            }
            case RespMessage.Maps:
            {
                var maps = await ReadMapsAsync(stream);
                return new RedisValue(maps, RespMessageTypeEnum.Maps);
            }
            default:
            {
                //错误
                throw new RedisExecException(ReadLine(stream,RespMessageTypeEnum.Error).ToString());
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
                return ReadLine(stream,RespMessageTypeEnum.Number);
            }
            //数组
            case RespMessage.ArrayString:
            {
                var length = ReadLine(stream,RespMessageTypeEnum.Number);
                if (length <=0)
                {
                    return RedisValue.Null();
                }

                throw new NotSupportedException(nameof(RespMessage.ArrayString));
            }
            case RespMessage.BulkStrings:
            {
                int offset = ReadLine(stream,RespMessageTypeEnum.Number);
                //如果为null
                if (offset == -1)
                {
                    return RedisValue.Null();
                }

                var result = await ReadBlobLineAsync(stream, offset);

                return new RedisValue(result,RespMessageTypeEnum.BulkStrings);
            }
            case RespMessage.Maps:
            {
                var maps = await ReadMapsAsync(stream);
                return new RedisValue(maps, RespMessageTypeEnum.Maps);
            }
            // case RespMessage.ArrayString:
            // {
            //     
            // }
            default:
            {
                //错误
                throw new RedisExecException(ReadLine(stream,RespMessageTypeEnum.Error).ToString());
            }
        }
    }

    /// <summary>
    /// 多行读取
    /// </summary>
    /// <param name="stream"></param>
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
                    resultList.Add(ReadLine(stream,RespMessageTypeEnum.Number));
                    break;
                }
                case RespMessage.BulkStrings:
                {
                    //获取字符串的长度
                    int offset = ReadLine(stream,RespMessageTypeEnum.Number);
                    //如果为null
                    if (offset == -1)
                    {
                        resultList.Add(RedisValue.Null());
                        break;
                    }

                    //读取结果
                    var result = await ReadBlobLineAsync(stream, offset);
                    resultList.Add(new RedisValue(result,RespMessageTypeEnum.BulkStrings));
                    break;
                }
                //数组
                case RespMessage.ArrayString:
                {
                    //读取剩下的消息
                    var result = await ReadMLineAsync(stream, ReadLine(stream,RespMessageTypeEnum.ArrayString));
                    resultList.Add(result);
                    break;
                }
                case RespMessage.Error:
                {
                    //todo 错误消息
                    resultList.Add(ReadLine(stream,RespMessageTypeEnum.Error));
                    break;
                }
            }
        }

        return resultList;
    }

    #region RESP3

    /// <summary>
    /// 读取key value 键值对结构
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private async Task<Dictionary<string,object>> ReadMapsAsync(Stream stream)
    {
        //读取长度
        var length = ReadLine(stream,RespMessageTypeEnum.Number);
        var maps = new Dictionary<string, object>(length);
        for (var i = 0; i < length; i++)
        {
            var key =await ReceiveSimpleMessageAsync(stream);
            var value=await ReceiveMessageAsync(stream);
            maps.Add(key,value);
        }

        return maps;
    }

    #endregion

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
            using (var memoryOwner = MemoryPool<byte>.Shared.Rent(length))
            {
                var mem = await stream.ReadAsync(memoryOwner.Memory[..(length - totalLength)]);
                await ms.WriteAsync(memoryOwner.Memory[..mem]);
                totalLength += mem;
                //读取 
                //这里用大于等于 是因为访问其它情况导致 陷入死循环
                if (totalLength >= length)
                {
                    break;
                }
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
        using var memoryOwner = MemoryPool<byte>.Shared.Rent(2);
        _ = await stream.ReadAsync(memoryOwner.Memory[..2]);
    }

    /// <summary>
    /// 读取第一行的字节信息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private static char ReadFirstChar(Stream stream)
    {
        var es = stream.ReadByte();
        //如果返回的-1 网络存在问题
        if (es == -1)
        {
            throw new IOException();
        }

        return (char) es;
    }

    /// <summary>
    /// 读取行数据
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private static RedisValue ReadLine(Stream stream,RespMessageTypeEnum respMessageType)
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

        return new RedisValue(ms.ToArray(),respMessageType);
    }
}