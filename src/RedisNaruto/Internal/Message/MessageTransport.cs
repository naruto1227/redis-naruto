using Microsoft.IO;
using RedisNaruto.Internal.Serialization;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.Internal.Message;

/// <summary>
/// 消息传输
/// </summary>
internal class MessageTransport 
{
    /// <summary>
    /// 池
    /// </summary>
    protected static readonly RecyclableMemoryStreamManager MemoryStreamManager = new();

    /// <summary>
    /// 换行
    /// </summary>
    protected static readonly byte[] NewLine = "\r\n".ToEncode();

    /// <summary>
    /// 序列化
    /// </summary>
    protected readonly ISerializer Serializer = new Serializer();

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="args"></param>
    public virtual async Task SendAsync(Stream stream, object[] args)
    {
        if (args is not {Length: > 0})
        {
            return;
        }

        await using var ms = MemoryStreamManager.GetStream();
        ms.Position = 0;
        await ms.WriteAsync(await Serializer.SerializeAsync($"{RespMessage.ArrayString}{args.Length}"));
        await ms.WriteAsync(NewLine);
        //判断参数长度
        foreach (var item in args)
        {
            //处理null
            if (item is null)
            {
                await ms.WriteAsync(await Serializer.SerializeAsync($"{RespMessage.BulkStrings}0"));
                await ms.WriteAsync(NewLine);
                await ms.WriteAsync(NewLine);
                continue;
            }

            if (item is byte[] argBytes)
            {
            }
            else
            {
                argBytes = await Serializer.SerializeAsync(item);
            }

            await ms.WriteAsync(await Serializer.SerializeAsync($"{RespMessage.BulkStrings}{argBytes.Length}"));
            await ms.WriteAsync(NewLine);
            await ms.WriteAsync(argBytes);
            await ms.WriteAsync(NewLine);
        }

        ms.Position = 0;
        await ms.CopyToAsync(stream);
    }

    // /// <summary>
    // /// 接收消息
    // /// </summary>
    // /// <param name="stream"></param>
    // /// <returns></returns>
    // public virtual async Task<object> ReceiveAsync(Stream stream)
    // {
    //     //获取首位的 符号 判断消息回复类型
    //     var bytes = new byte[1];
    //     _ = await stream.ReadAsync(bytes);
    //     var head = (char) bytes[0];
    //     switch (head)
    //     {
    //         case RespMessage.SimpleString:
    //         case RespMessage.Number:
    //         {
    //             var result = ReadLine(stream);
    //             return result;
    //         }
    //         //数组
    //         case RespMessage.ArrayString:
    //         {
    //             var result = await ReadMLineAsync(stream);
    //             return result;
    //         }
    //         case RespMessage.BulkStrings:
    //         {
    //             var strlen = ReadLine(stream);
    //             //如果为null
    //             if (strlen == -1)
    //             {
    //                 return RedisValue.Null();
    //             }
    //
    //             var result = ReadMLine(stream, strlen);
    //             return result;
    //         }
    //         default:
    //         {
    //             //错误
    //             var result = ReadLine(stream);
    //             throw new RedisExecException(result.ToString());
    //         }
    //     }
    // }
    //
    // /// <summary>
    // /// 接收消息
    // /// </summary>
    // /// <param name="stream"></param>
    // /// <param name="pipeCount"></param>
    // /// <returns></returns>
    // public virtual async Task<object[]> PipeReceiveAsync(Stream stream, int pipeCount)
    // {
    //     var result = new object[pipeCount];
    //     for (var i = 0; i < pipeCount; i++)
    //     {
    //         result[i] = await ReceiveAsync(stream);
    //     }
    //
    //     return result;
    // }
    //

    /// <summary>
    /// 读取行数据
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private static RedisValue ReadLine(Stream stream)
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
                bytes.Add((byte) msg);
                bytes.Add((byte) msg2);
            }
            else
                bytes.Add((byte) msg);
        }

        return new RedisValue(bytes.ToArray());
    }

    /// <summary>
    /// 读取行数据
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private static RedisValue ReadMLine(Stream stream, int offset)
    {
        Span<byte> bytes = new byte[offset + 2];
        _ = stream.Read(bytes);
        return new RedisValue(bytes.Slice(0, offset).ToArray());
    }

    /// <summary>
    /// 多行读取
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private async Task<List<object>> ReadMLineAsync(Stream stream)
    {
        //读取数组的长度
        var length = ReadLine(stream).ToInt();
        if (length == -1)
        {
            return default;
        }

        List<object> resultList = new();
        for (var i = 0; i < length; i++)
        {
            //获取 符号 判断消息类型 是字符串还是 数字 
            var bytes = new byte[1];
            _ = await stream.ReadAsync(bytes);
            var head = (char) bytes[0];
            switch (head)
            {
                case RespMessage.SimpleString:
                case RespMessage.Number:
                {
                    var result = ReadLine(stream);
                    resultList.Add(result);
                    break;
                }
                case RespMessage.BulkStrings:
                {
                    //去除第一位的长度
                    var strlen = ReadLine(stream);
                    //如果为null
                    if (strlen == -1)
                    {
                        resultList.Add(RedisValue.Null());
                        break;
                    }

                    //读取结果
                    var result = ReadMLine(stream, strlen);
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