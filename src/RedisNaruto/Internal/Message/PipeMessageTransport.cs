using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
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
internal sealed class PipeMessageTransport : MessageTransport
{
    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public override async Task<object> ReceiveAsync(Stream stream)
    {
        var pipe = new Pipe(new PipeOptions(null, null, null, -1L, -1L, 513));
        //读取消息 将消息写入到 pipe˙中
        await PipeWrite(pipe.Writer, stream);

        await using var dispose = new AsyncDisposeAction(() => pipe.Reader.CompleteAsync().AsTask());

        return await ReceiveCoreAsync(pipe.Reader);
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="pipeCount"></param>
    /// <returns></returns>
    public override async Task<object[]> PipeReceiveAsync(Stream stream, int pipeCount)
    {
        var pipe = new Pipe();
        //读取消息 将消息写入到 pipe˙中
        await PipeWrite(pipe.Writer, stream);
        await using var dispose = new AsyncDisposeAction(() => pipe.Reader.CompleteAsync().AsTask());
        var result = new object[pipeCount];
        for (var i = 0; i < pipeCount; i++)
        {
            result[i] = await ReceiveCoreAsync(pipe.Reader);
        }

        return result;
    }

    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="pipeReader"></param>
    /// <returns></returns>
    private static async Task<object> ReceiveCoreAsync(PipeReader pipeReader)
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

    private static async Task PipeWrite(PipeWriter writer, Stream stream)
    {
        while (true)
        {
            var mem = writer.GetMemory(513);
            var es = await stream.ReadAsync(mem);
            //告诉PipeWriter从Socket中读取了多少。
            writer.Advance(es);
            // 使数据对piperreader可用。
            await writer.FlushAsync();
            //判断消息是否读取完毕 如果消息长度小于实际长度 或者 结尾是换行的话
            if (es < mem.Length || mem.TrimEnd(NewLine).Length == mem.Length - 2)
            {
                break;
            }
        }

        // 通过完成PipeWriter，告诉piperreader没有更多的数据了。
        await writer.CompleteAsync();
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
        // var line = buffer.Slice(0, length);
        // // //如果消息已经读取完毕就不拆分了
        // // if (length + 2 != buffer.Length)
        // // {
        // //     buffer = buffer.Slice(length + 2);
        // // }
        // buffer = buffer.Slice(length + 2);
        // return line;

        #region 方案1

        // var reader = new SequenceReader<byte>(buffer);
        // reader.TryReadTo(out ReadOnlySpan<byte> span, NewLine);
        // buffer = buffer.Slice(span.Length + 2);
        // // 
        // return new ReadOnlySequence<byte>(span.ToArray());

        #endregion

        #region 方案2

        // //找到 \r 最近的一个偏移数据
        // var position = buffer.PositionOf((byte) '\r');
        // if (position == null)
        // {
        //     return default;
        // }
        //
        // // Skip the line + the \n.
        // var line = buffer.Slice(0, position.Value);
        // if (line.PositionOf((byte) '\n') != null)
        // {
        //     line = line.Slice(1, line.Length - 1);
        // }
        // else if (line.Length + 2 != buffer.Length)
        // {
        //     //过滤掉\r\n
        //     buffer = buffer.Slice(buffer.GetPosition(2, position.Value));
        //     return line;
        // }
        //
        // //过滤掉\n
        // buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
        // return line;

        #endregion
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
        // //如果已经读取到最后的就不进行Slice拆分了
        // if (offset + 2 == buffer.Length)
        // {
        //     return line;
        // }

        //过滤掉后面的\r\n
        buffer = buffer.Slice(offset + 2, buffer.Length - (offset + 2));
        return line;
    }
}