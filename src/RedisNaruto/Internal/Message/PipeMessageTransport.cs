using System.Buffers;
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
public sealed class PipeMessageTransport : IMessageTransport
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
            if (item is not byte[] argBytes)
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
    /// 接收消息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public async Task<object> ReceiveAsync(Stream stream)
    {
        var pipe = new Pipe();
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
    public async Task<object[]> PipeReceiveAsync(Stream stream, int pipeCount)
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
        var remindMessage = new RedisValue(firstByteSequence.Slice(1, firstByteSequence.Length - 1).First);
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
                //如果为null
                if (remindMessage == "-1")
                {
                    return RedisValue.Null();
                }

                var result = await ReadLineAsync(pipeReader);
                return new RedisValue(result.First);
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
    private static async Task<List<object>> ReadMLineAsync(PipeReader pipeReader, RedisValue length)
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
            //读取剩下的消息
            var remindMessage = new RedisValue(firstByteSequence.Slice(1, firstByteSequence.Length - 1).First);
            switch (head)
            {
                case RespMessage.SimpleString:
                case RespMessage.Number:
                {
                    resultList.Add(remindMessage);
                    break;
                }
                case RespMessage.BulkStrings:
                {
                    //如果为null
                    if (remindMessage == "-1")
                    {
                        resultList.Add(RedisValue.Null());
                        break;
                    }

                    //读取结果
                    var result = await ReadLineAsync(pipeReader);
                    resultList.Add(new RedisValue(result.First));
                    break;
                }
                //数组
                case RespMessage.ArrayString:
                {
                    var result = await ReadMLineAsync(pipeReader, remindMessage);
                    resultList.Add(result);
                    break;
                }
                case RespMessage.Error:
                {
                    resultList.Add(remindMessage);
                    break;
                }
            }
        }

        return resultList;
    }

    private static async Task PipeWrite(PipeWriter writer, Stream stream)
    {
        var mem = writer.GetMemory(512);
        var es = await stream.ReadAsync(mem);
        //告诉PipeWriter从Socket中读取了多少。
        writer.Advance(es);
        // 使数据对piperreader可用。
        await writer.FlushAsync();
        // 通过完成PipeWriter，告诉piperreader没有更多的数据了。
        await writer.CompleteAsync();
    }


    private static async IAsyncEnumerable<ReadOnlySequence<byte>> PipeRead(PipeReader reader)
    {
        while (true)
        {
            var result = await reader.ReadAsync();
            var buffer = result.Buffer;

            while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
            {
                yield return line;
            }

            // 告诉piperreader已经占用了多少缓冲区。
            reader.AdvanceTo(buffer.Start, buffer.End);

            // Stop reading if there's no more data coming.
            if (result.IsCompleted)
            {
                break;
            }
        }

        // Mark the PipeReader as complete.
        await reader.CompleteAsync();
    }

    private static async Task<ReadOnlySequence<byte>> ReadLineAsync(PipeReader reader)
    {
        var result = await reader.ReadAsync();
        var buffer = result.Buffer;
        TryReadLine(ref buffer, out ReadOnlySequence<byte> line);
        // 告诉piperreader已经占用了多少缓冲区。
        reader.AdvanceTo(buffer.Start, buffer.End);
        return line;
    }

    /// <summary>
    /// 读取行消息
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    private static bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
    {
        // 根据\n 进行拆分
        SequencePosition? position = buffer.PositionOf((byte) '\r');

        if (position == null)
        {
            line = default;
            return false;
        }

        // Skip the line + the \n.
        line = buffer.Slice(0, position.Value);
        if (line.PositionOf((byte) '\n') != null)
        {
            line = line.Slice(1, line.Length - 1);
        }

        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
        return true;
    }
}