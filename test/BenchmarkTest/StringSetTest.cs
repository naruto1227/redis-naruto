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
        for (int i = 0; i < 1_000; i++)
        {
            await _redisCommand.StringSet("testobj",new TestModel
            {
                Id = Guid.NewGuid(),
                Name = "123",
                Time = DateTime.Now
            });
        }
       
    }
}