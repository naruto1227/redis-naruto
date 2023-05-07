using BenchmarkDotNet.Attributes;
using RedisNaruto;

namespace BenchmarkTest;

[MemoryDiagnoser()]
public class StringSetTest
{
    private IRedisCommand _redisCommand;

    /// <summary>
    /// 
    /// </summary>
    [GlobalSetup]
    public async Task Setup()
    {
        _redisCommand = await RedisConnection.ConnectionAsync(new ConnectionModel
        {
            Connection = new string[]
            {
                "127.0.0.1:55000"
            },
            UserName = null,
            Password = "redispw"
        });
    }

    [Benchmark()]
    public async Task StringSet()
    {
         var fileBytes = await _redisCommand.GetAsync("file");
    }
}