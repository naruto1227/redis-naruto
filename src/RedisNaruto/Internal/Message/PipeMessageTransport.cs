// using System.IO.Pipelines;
// using System.Net.Sockets;
// using RedisNaruto.Internal.Models;
// using RedisNaruto.Utils;
//
// namespace RedisNaruto.Internal.Message;
//
// /// <summary>
// /// 消息传输
// /// </summary>
// internal sealed class PipeMessageTransport : MessageTransport, IMessageTransport
// {
//     // /// <summary>
//     // /// 发送消息
//     // /// </summary>
//     // /// <param name="stream"></param>
//     // /// <param name="command"></param>
//     // public override async Task SendAsync(Stream stream, Command command)
//     // {
//     //     var pipeWriter = PipeWriter.Create(stream);
//     //     await pipeWriter.WriteAsync(await Serializer.SerializeAsync($"{RespMessage.ArrayString}{command.Length}"));
//     //     await pipeWriter.WriteAsync(NewLine);
//     //
//     //     //写入命令
//     //     var cmdBytes = command.Cmd.ToEncode();
//     //     await pipeWriter.WriteAsync(await Serializer.SerializeAsync($"{RespMessage.BulkStrings}{cmdBytes.Length}"));
//     //     await pipeWriter.WriteAsync(NewLine);
//     //     await pipeWriter.WriteAsync(cmdBytes);
//     //     await pipeWriter.WriteAsync(NewLine);
//     //     if (command.Length > 1)
//     //     {
//     //         //判断参数长度
//     //         foreach (var item in command.Args)
//     //         {
//     //             //处理null
//     //             if (item is null)
//     //             {
//     //                 await pipeWriter.WriteAsync(await Serializer.SerializeAsync($"{RespMessage.BulkStrings}0"));
//     //                 await pipeWriter.WriteAsync(NewLine);
//     //                 await pipeWriter.WriteAsync(NewLine);
//     //                 continue;
//     //             }
//     //
//     //             if (item is not byte[] argBytes)
//     //             {
//     //                 argBytes = await Serializer.SerializeAsync(item);
//     //             }
//     //
//     //             await pipeWriter.WriteAsync(
//     //                 await Serializer.SerializeAsync($"{RespMessage.BulkStrings}{argBytes.Length}"));
//     //             await pipeWriter.WriteAsync(NewLine);
//     //             await pipeWriter.WriteAsync(argBytes);
//     //             await pipeWriter.WriteAsync(NewLine);
//     //         }
//     //     }
//     // }
//
//     /// <summary>
//     /// 接收消息
//     /// </summary>
//     /// <param name="stream"></param>
//     /// <returns></returns>
//     public async Task<PipeReader> ReceiveAsync(Stream stream)
//     {
//         var pipe = new Pipe(new PipeOptions(null, null, null, -1L, -1L, 213));
//         //读取消息 将消息写入到 pipe˙中
//         //todo 消息读取完成 应该释放
//         await PipeWrite(pipe.Writer, stream);
//         // await using var dispose = new AsyncDisposeAction(() => pipe.Reader.CompleteAsync().AsTask());
//
//         return pipe.Reader;
//     }
//
//     private static async ValueTask PipeWrite(PipeWriter writer, Stream stream)
//     {
//         while (true)
//         {
//             var mem = writer.GetMemory(1);
//             var es = await stream.ReadAsync(mem);
//             //告诉PipeWriter从Socket中读取了多少。
//             writer.Advance(es);
//             // 使数据对piperreader可用。
//             await writer.FlushAsync();
//             //判断消息是否读取完毕 如果消息长度小于实际长度 或者 结尾是换行的话
//             if (es < mem.Length || mem.Span.LastIndexOf(NewLine) == mem.Length - 2)
//             {
//                 break;
//             }
//         }
//
//         // 通过完成PipeWriter，告诉piperreader没有更多的数据了。
//         await writer.CompleteAsync();
//     }
// }