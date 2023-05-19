using FreeRedis;
using StackExchange.Redis;

namespace TestProject1;

public class TestRedisClient : BaseUnit
{
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
}