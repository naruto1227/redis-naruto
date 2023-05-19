using System.Buffers;
using System.Buffers.Binary;
using System.Text;
using BenchmarkDotNet.Attributes;
using FreeRedis;
using NewLife.Caching;
using RedisNaruto;
using StackExchange.Redis;
using RedisClient = FreeRedis.RedisClient;

namespace BenchmarkTest;

public class BaseTest
{
    [Params(1)] public int N;
    protected static IRedisCommand RedisCommand;
    //
    protected static IDatabase RedisConn;

    protected static RedisClient? _redisClient;

    protected static byte[] testmes = Encoding.Default.GetBytes("1");

    protected static string TestMsg = "";

    protected static FullRedis FullRedis;

    /// <summary>
    /// 
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        RedisCommand = RedisConnection.CreateAsync(new ConnectionBuilder()
        {
            Connection = new string[]
            {
                "127.0.0.1:55000"
            },
            UserName = "default",
            DataBase = 4,
            Password = "redispw",
            PoolCount = 10
        }).GetAwaiter().GetResult();

        ConfigurationOptions configurationOptions = ConfigurationOptions.Parse(
            "127.0.0.1:55000,password=redispw,connectTimeout=2000,defaultDatabase=1");
        RedisConn = (ConnectionMultiplexer.Connect(configurationOptions)).GetDatabase();

        _redisClient = new RedisClient(new ConnectionStringBuilder
        {
            Host = "127.0.0.1:55000",
            Protocol = RedisProtocol.RESP2,
            User = "default",
            Password = "redispw",
            Database = 2,
        });

        FullRedis = new FullRedis("127.0.0.1:55000", "redispw", 3);

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < 10000; i++)
        {
            stringBuilder.Append("测试");
        }


        TestMsg = stringBuilder.ToString();
    }
}