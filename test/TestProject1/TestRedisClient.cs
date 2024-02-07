using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FreeRedis;
using Microsoft.Extensions.ObjectPool;
using StackExchange.Redis;
using Xunit.Abstractions;
using RedisProtocol = FreeRedis.RedisProtocol;

namespace TestProject1;

public class TestRedisClient : BaseUnit
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// 
    /// </summary>
    [Fact]
    public async Task TestFreeRedis()
    {
        var cli = new RedisClient(new ConnectionStringBuilder
        {
            Host = "127.0.0.1:55000",
            Protocol = RedisProtocol.RESP3,
            User = "default",
            Password = "redispw",
            Database = 0,
        });
        await cli.SetAsync("tr", "1");
    }

    [Fact]
    public async Task Test_RedisNaruto()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SetAsync("Test_RedisNaruto", null);
        var res2 = await redisCommand.GetAsync("Test_RedisNaruto");
    }

    // 分别设置实例的连接地址、端口号和密码。
    private static ConfigurationOptions configurationOptions = ConfigurationOptions.Parse(
        "127.0.0.1:55000,password=redispw,connectTimeout=2000");

    //the lock for singleton
    private static readonly object Locker = new object();

    //singleton
    private static ConnectionMultiplexer redisConn;

    public TestRedisClient(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    //singleton
    public static ConnectionMultiplexer getRedisConn()
    {
        if (redisConn == null)
        {
            lock (Locker)
            {
                if (redisConn == null || !redisConn.IsConnected)
                {
                    redisConn = ConnectionMultiplexer.Connect(configurationOptions);
                }
            }
        }

        return redisConn;
    }

    /// <summary>
    /// 
    /// </summary>
    [Fact]
    public async Task TestSR()
    {
        var cli = getRedisConn().GetDatabase();
        await cli.StringSetAsync("sr", "1");
    }


    /// <summary>
    /// 
    /// </summary>
    [Fact]
    public async Task Dis()
    {
        var poolPolicy = new DefaultPooledObjectPolicy<TcpClient>();
        ObjectPool<TcpClient> _tcpClientPool = new DefaultObjectPool<TcpClient>(poolPolicy, 10);
        TcpClient tcpClient = _tcpClientPool.Get();
        await tcpClient.ConnectAsync(IPAddress.Parse("127.0.0.1"), 5000);

        await tcpClient.GetStream().WriteAsync(Encoding.Default.GetBytes("ping"));

        MemoryPool<byte> memoryPool = MemoryPool<byte>.Shared;
        using (var mem = memoryPool.Rent(1024))
        {
            var res = await tcpClient.GetStream().ReadAsync(mem.Memory);
            _testOutputHelper.WriteLine(Encoding.Default.GetString(mem.Memory.Slice(0, res).ToArray()));
        }

        await tcpClient.GetStream().WriteAsync(Encoding.Default.GetBytes("ping"));

        // await tcpClient.ConnectAsync(IPAddress.Parse("127.0.0.1"), 5000);
    }
}