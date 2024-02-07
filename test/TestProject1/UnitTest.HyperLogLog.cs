using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest_HyperLogLog : BaseUnit
{

    public UnitTest_HyperLogLog(ITestOutputHelper testOutputHelper):base(testOutputHelper)
    {
    }

    [Fact]
    public async Task Test_PfAddAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.PfAddAsync("hy", new[]
        {
           Guid.NewGuid().ToString(),
        });
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_PfCountAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.PfCountAsync(new[] {"HyperLogLog","hy"});
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_PfMergeAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.PfMergeAsync(new[] {"HyperLogLog"}, "hy");
        _testOutputHelper.WriteLine(res.ToString());
    }
}