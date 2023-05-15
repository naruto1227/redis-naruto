using System.Buffers;
using System.IO.Pipelines;
using RedisNaruto.Exceptions;
using RedisNaruto.Internal.Serialization;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.Internal.Message.MessageParses;

internal sealed class MessageParse : IMessageParse
{
    /// <summary>
    /// 换行
    /// </summary>
    private static readonly byte[] NewLine = "\r\n".ToEncode();

    /// <summary>
    /// 转换消息
    /// </summary>
    /// <param name="pipeReader"></param>
    /// <returns></returns>
    public async Task<object> ParseMessageAsync(PipeReader pipeReader)
    {
        //获取首位的 符号 判断消息回复类型
        var firstByteSequence = await ReadLineAsync(pipeReader);
        var head = (char) firstByteSequence.Slice(0, 1).ToArray().First();
        //读取剩下的消息
        var remindMessage = new RedisValue(firstByteSequence.Slice(1, firstByteSequence.Length - 1));
        switch (head)
        {
            case RespMessage.SimpleString:
            case RespMessage.Number:
            {
                return remindMessage;
            }
            //数组
            case RespMessage.ArrayString:
            {
                var result = await ReadMLineAsync(pipeReader, remindMessage);
                return result;
            }
            case RespMessage.BulkStrings:
            {
                int offset = remindMessage;
                //如果为null
                if (offset == -1)
                {
                    return RedisValue.Null();
                }

                var result = await ReadLineAsync(pipeReader, offset);

                return new RedisValue(result);
            }
            default:
            {
                //错误
                throw new RedisExecException(remindMessage);
            }
        }
    }

    /// <summary>
    /// 多行读取
    /// </summary>
    /// <param name="pipeReader"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    private static async Task<List<object>> ReadMLineAsync(PipeReader pipeReader, int length)
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
            var firstByteSequence = await ReadLineAsync(pipeReader);
            var head = (char) firstByteSequence.Slice(0, 1).ToArray().First();
            switch (head)
            {
                case RespMessage.SimpleString:
                case RespMessage.Number:
                {
                    //读取剩下的消息
                    var remindMessage = new RedisValue(firstByteSequence.Slice(1, firstByteSequence.Length - 1));
                    resultList.Add(remindMessage);
                    break;
                }
                case RespMessage.BulkStrings:
                {
                    //获取字符串的长度
                    int offset = new RedisValue(firstByteSequence.Slice(1, firstByteSequence.Length - 1));
                    //如果为null
                    if (offset == -1)
                    {
                        resultList.Add(RedisValue.Null());
                        break;
                    }

                    //读取结果
                    var result = await ReadLineAsync(pipeReader, offset);
                    resultList.Add(new RedisValue(result));
                    break;
                }
                //数组
                case RespMessage.ArrayString:
                {
                    //读取剩下的消息
                    var remindMessage = new RedisValue(firstByteSequence.Slice(1, firstByteSequence.Length - 1));
                    var result = await ReadMLineAsync(pipeReader, remindMessage);
                    resultList.Add(result);
                    break;
                }
                case RespMessage.Error:
                {
                    resultList.Add(RedisValue.Error(firstByteSequence.Slice(1, firstByteSequence.Length - 1)));
                    break;
                }
            }
        }

        return resultList;
    }


    private static async Task<ReadOnlyMemory<byte>> ReadLineAsync(PipeReader reader, int offset = 0)
    {
        var result = await reader.ReadAsync();
        var buffer = result.Buffer;
        var line = offset == 0 ? ReadLine(ref buffer) : ReadLineByOffset(ref buffer, offset);
        //转换结果
        var memory = new ReadOnlyMemory<byte>(line.ToArray());
        // 告诉piperreader数据已经读取到哪里了
        reader.AdvanceTo(buffer.Start);
        return memory;
    }

    /// <summary>
    /// 读取行消息
    /// 简单消息的获取
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    private static ReadOnlySequence<byte> ReadLine(ref ReadOnlySequence<byte> buffer)
    {
        //因为是简单消息的处理 所以直接获取第一个span 来判断 行消息
        var length = buffer.FirstSpan.IndexOf(NewLine);
        return ReadLineByOffset(ref buffer, length);
    }

    /// <summary>
    /// 读取行消息
    /// 获取批量字符串消息格式的
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="offset">偏移的长度</param>
    /// <returns></returns>
    private static ReadOnlySequence<byte> ReadLineByOffset(ref ReadOnlySequence<byte> buffer,
        int offset)
    {
        //读取指定的数据
        var line = buffer.Slice(0, offset);
        //过滤掉后面的\r\n
        buffer = buffer.Slice(offset + 2, buffer.Length - (offset + 2));
        return line;
    }
}