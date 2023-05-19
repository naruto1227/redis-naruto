using System.Buffers;
using System.Buffers.Binary;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkTest;
using FreeRedis;
using NewLife.Caching;
using RedisNaruto;
using StackExchange.Redis;
using RedisClient = FreeRedis.RedisClient;

[MemoryDiagnoser()]
public class StringGetTest : BaseTest
{
    [Benchmark]
    public async Task Test_Get_RedisNaruto()
    {
        for (int i = 0; i < N; i++)
        {
            var r = await RedisCommand.GetAsync("sr");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Benchmark]
    public async Task Test_Get_FreeRedis()
    {
        for (int i = 0; i < N; i++)
        {
            var s = await _redisClient.GetAsync("sr");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Benchmark]
    public void Test_Get_NewLifeRedis()
    {
        for (int i = 0; i < N; i++)
        {
            var s = FullRedis.Get<string>("sr");
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [Benchmark]
    public async Task Test_Get_StackExchange()
    {
        for (int i = 0; i < N; i++)
        {
            var s = await RedisConn.StringGetAsync("sr");
        }
    }
}