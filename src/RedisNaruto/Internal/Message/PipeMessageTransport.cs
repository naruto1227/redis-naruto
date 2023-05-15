using System.IO.Pipelines;

namespace RedisNaruto.Internal.Message;

/// <summary>
/// 消息传输
/// </summary>
internal sealed class PipeMessageTransport : MessageTransport, IMessageTransport
{
    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public async Task<PipeReader> ReceiveAsync(Stream stream)
    {
        var pipe = new Pipe(new PipeOptions(null, null, null, -1L, -1L, 513));
        //读取消息 将消息写入到 pipe˙中
        //todo 消息读取完成 应该释放
        await PipeWrite(pipe.Writer, stream);
        // await using var dispose = new AsyncDisposeAction(() => pipe.Reader.CompleteAsync().AsTask());

        return pipe.Reader;
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
}