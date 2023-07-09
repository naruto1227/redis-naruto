using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;
using RedisNaruto.Internal;
using RedisNaruto.Internal.Message;
using RedisNaruto.Internal.Serialization;
using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest1_Client : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest1_Client(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Test_ClientId()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ClientIdAsync();
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_Ping()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.PingAsync();
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_DbSizeAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.DbSizeAsync();
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_SlowLogAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SlowLogAsync(2);
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_SlowLogLenAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SlowLogLenAsync();
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task testTcp()
    {
        var pipe = new Pipe();
        var reader = pipe.Reader;
        var write = pipe.Writer;
        // write.WriteAsync()
        // TcpClient tcpClient = new TcpClient();
        // await tcpClient.ConnectAsync("127.0.0.1", 55002);
        // await tcpClient.GetStream().WriteAsync(Encoding.Default.GetBytes("ping"));
        // var s = new byte[1024];
        // await tcpClient.GetStream().ReadAsync(s);
        TcpClient tcpClient2 = new TcpClient();
        await tcpClient2.ConnectAsync("127.0.0.1", 55000);

        var s1 = "HELLO"u8.ToArray();
        await SendAsync(tcpClient2.GetStream(), new object[]
        {
            "HELLO"u8.ToArray(),
            "3"u8.ToArray(),
            "AUTH"u8.ToArray(),
            "default"u8.ToArray(),
            "redispw"u8.ToArray()
        });
        // var ps = PipeReader.Create(tcpClient2.GetStream());
        //
        // var ss = await ps.ReadAsync();
        // var str = Encoding.UTF8.GetString(ss.Buffer);
        // _testOutputHelper.WriteLine(str);
        // await messageTransport.SendAsync(tcpClient2.GetStream(), new object[]
        // {
        //     "ping"
        // });
        // var s = await messageTransport.ReceiveAsync(tcpClient2.GetStream());
        await PipeWrite(pipe.Writer, tcpClient2.GetStream());
        await PipeRead(pipe.Reader);


        await SendAsync(tcpClient2.GetStream(), new object[]
        {
            "Set"u8.ToArray(),
            "3"u8.ToArray(),
            "2"u8.ToArray()
        });
        pipe = new Pipe();
        await PipeWrite(pipe.Writer, tcpClient2.GetStream());
        await PipeRead(pipe.Reader);


        await SendAsync(tcpClient2.GetStream(), new object[]
        {
            "GEt"u8.ToArray(),
            "3"u8.ToArray(),
        });
        pipe = new Pipe();
        await PipeWrite(pipe.Writer, tcpClient2.GetStream());
        await PipeRead(pipe.Reader);


        await SendAsync(tcpClient2.GetStream(), new object[]
        {
            "XRead"u8.ToArray(),
            "STREAMS"u8.ToArray(),
            "mystream3"u8.ToArray(),
            "0"u8.ToArray(),
        });
        pipe = new Pipe();
        await PipeWrite(pipe.Writer, tcpClient2.GetStream());
        await PipeRead(pipe.Reader);
    }

    [Fact]
    public async Task Test_RESP_Ping()
    {
        TcpClient tcpClient2 = new TcpClient
        {
            NoDelay = true //关闭Nagle算法
        };
        tcpClient2.Client.Blocking = false;
        Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        socket.Blocking = false;

        await tcpClient2.ConnectAsync("127.0.0.1", 55003);
        await socket.ConnectAsync("127.0.0.1", 55003);
        byte[] Ping = Encoding.UTF8.GetBytes(
            $"1");
        await tcpClient2.Client.SendAsync(Ping);
        var memory = new byte[1024];
        var len = await tcpClient2.Client.ReceiveAsync(memory);
        var respon = Encoding.Default.GetString(memory.AsSpan()[..len].ToArray());
        _testOutputHelper.WriteLine(respon);
    }

    /// <summary>
    /// 换行
    /// </summary>
    protected static readonly byte[] NewLine = Encoding.Default.GetBytes("\r\n");

    public virtual async Task SendAsync(Stream stream, object[] args)
    {
        if (args is not {Length: > 0})
        {
            return;
        }

        // await using var ms = MemoryStreamManager.GetStream();
        // ms.Position = 0;
        await stream.WriteAsync(Encoding.Default.GetBytes($"{RespMessage.ArrayString}{args.Length}"));
        await stream.WriteAsync(NewLine);
        //判断参数长度
        foreach (var item in args)
        {
            if (item is byte[] argBytes)
            {
                await stream.WriteAsync(Encoding.Default.GetBytes($"{RespMessage.BulkStrings}{argBytes.Length}"));
                await stream.WriteAsync(NewLine);
                await stream.WriteAsync(argBytes);
                await stream.WriteAsync(NewLine);
            }
        }
    }

    private async Task PipeWrite(PipeWriter writer, Stream stream)
    {
        var mem = writer.GetMemory(512);
        var es = await stream.ReadAsync(mem);
        var str = Encoding.Default.GetString(mem.Slice(0, es).ToArray());
        //告诉PipeWriter从Socket中读取了多少。
        writer.Advance(es);
        // 使数据对piperreader可用。
        var result = await writer.FlushAsync();
        // 通过完成PipeWriter，告诉piperreader没有更多的数据了。
        await writer.CompleteAsync();
    }


    private async Task PipeRead(PipeReader reader)
    {
        while (true)
        {
            ReadResult result = await reader.ReadAsync();
            ReadOnlySequence<byte> buffer = result.Buffer;


            while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
            {
                var s = Encoding.Default.GetString(line);
                // Process the line.
                // ProcessLine(line);
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

    bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
    {
        if (buffer.Length <= 0)
        {
            line = default;
            return false;
        }

        //因为是简单消息的处理 所以直接获取第一个span 来判断 行消息
        var offset = buffer.FirstSpan.IndexOf(NewLine);
        //读取行消息
        line = buffer.Slice(0, offset);

        buffer = buffer.Slice(offset + 2, buffer.Length - (offset + 2));
        return true;
    }

    [Fact]
    public async Task Test_SelectDb()
    {
        var redis = await GetRedisAsync();
        await using (var command = await redis.SelectDbAsync(1))
        {
            await command.SetAsync("select_db1", "1");
        }

        await using (var command = await redis.SelectDbAsync(1))
        {
            await command.SetAsync("select_db2", "1");
        }

        await using (var command = await redis.SelectDbAsync(1))
        {
            await command.SetAsync("select_db3", "1");
        }

        await redis.SetAsync("db1", "1111");
    }
}