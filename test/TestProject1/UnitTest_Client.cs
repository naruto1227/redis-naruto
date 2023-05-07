// using System.Buffers;
// using System.IO.Pipelines;
// using System.Net.Sockets;
// using System.Text;
// using RedisNaruto.Internal.Message;
// using Xunit.Abstractions;
//
// namespace TestProject1;
//
// public class UnitTest1_Client : BaseUnit
// {
//     private ITestOutputHelper _testOutputHelper;
//
//     public UnitTest1_Client(ITestOutputHelper testOutputHelper)
//     {
//         _testOutputHelper = testOutputHelper;
//     }
//
//     [Fact]
//     public async Task Test_ClientId()
//     {
//         var redisCommand = await GetRedisAsync();
//         var res = await redisCommand.ClientIdAsync();
//         _testOutputHelper.WriteLine(res.ToString());
//     }
//
//     [Fact]
//     public async Task Test_Ping()
//     {
//         var redisCommand = await GetRedisAsync();
//         var res = await redisCommand.PingAsync();
//         _testOutputHelper.WriteLine(res.ToString());
//     }
//
//     [Fact]
//     public async Task Test_DbSizeAsync()
//     {
//         var redisCommand = await GetRedisAsync();
//         var res = await redisCommand.DbSizeAsync();
//         _testOutputHelper.WriteLine(res.ToString());
//     }
//
//     [Fact]
//     public async Task Test_SlowLogAsync()
//     {
//         var redisCommand = await GetRedisAsync();
//         var res = await redisCommand.SlowLogAsync(2);
//         _testOutputHelper.WriteLine(res.ToString());
//     }
//
//     [Fact]
//     public async Task Test_SlowLogLenAsync()
//     {
//         var redisCommand = await GetRedisAsync();
//         var res = await redisCommand.SlowLogLenAsync();
//         _testOutputHelper.WriteLine(res.ToString());
//     }
//
//     [Fact]
//     public async Task testTcp()
//     {
//         var pipe = new Pipe();
//         var reader = pipe.Reader;
//         var write = pipe.Writer;
//         // write.WriteAsync()
//         // TcpClient tcpClient = new TcpClient();
//         // await tcpClient.ConnectAsync("127.0.0.1", 55002);
//         // await tcpClient.GetStream().WriteAsync(Encoding.Default.GetBytes("ping"));
//         // var s = new byte[1024];
//         // await tcpClient.GetStream().ReadAsync(s);
//         TcpClient tcpClient2 = new TcpClient();
//         await tcpClient2.ConnectAsync("127.0.0.1", 55000);
//
//         IMessageTransport messageTransport = new MessageTransport();
//
//         await messageTransport.SendAsync(tcpClient2.GetStream(), new object[]
//         {
//             "auth",
//             "redispw"
//         });
//         // await messageTransport.SendAsync(tcpClient2.GetStream(), new object[]
//         // {
//         //     "ping"
//         // });
//         // var s = await messageTransport.ReceiveAsync(tcpClient2.GetStream());
//
//         await PipeWrite(write, tcpClient2.GetStream());
//         await PipeRead(reader);
//     }
//
//     private async Task PipeWrite(PipeWriter writer, Stream stream)
//     {
//         var mem = writer.GetMemory(512);
//         var es = await stream.ReadAsync(mem);
//         //告诉PipeWriter从Socket中读取了多少。
//         writer.Advance(es);
//         // 使数据对piperreader可用。
//         var result = await writer.FlushAsync();
//         // 通过完成PipeWriter，告诉piperreader没有更多的数据了。
//         await writer.CompleteAsync();
//     }
//
//
//     private async Task PipeRead(PipeReader reader)
//     {
//         while (true)
//         {
//             ReadResult result = await reader.ReadAsync();
//             ReadOnlySequence<byte> buffer = result.Buffer;
//
//             while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
//             {
//                 var s=Encoding.Default.GetString(line);
//                 // Process the line.
//                 // ProcessLine(line);
//             }
//
//             // 告诉piperreader已经占用了多少缓冲区。
//             reader.AdvanceTo(buffer.Start, buffer.End);
//
//             // Stop reading if there's no more data coming.
//             if (result.IsCompleted)
//             {
//                 break;
//             }
//         }
//
//         // Mark the PipeReader as complete.
//         await reader.CompleteAsync();
//     }
//     bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
//     {
//         // Look for a EOL in the buffer.
//         SequencePosition? position = buffer.PositionOf((byte)'\n');
//
//         if (position == null)
//         {
//             line = default;
//             return false;
//         }
//
//         // Skip the line + the \n.
//         line = buffer.Slice(0, position.Value);
//         buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
//         return true;
//     }
// }