using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest_Generic : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest_Generic(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Test_CopyAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.CopyAsync("testobjset", "testobjset3", 2, true);
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_DelAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.DelAsync(new[] {"testobjset"});
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ExistsAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ExistsAsync(new[] {"testobjset"});
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_DumpAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.DumpAsync("key2");
        _testOutputHelper.WriteLine(res);
    }

    [Fact]
    public async Task Test_ReStoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.DumpAsync("key2");
        _testOutputHelper.WriteLine(res);
        if (res != null)
        {
            await redisCommand.ReStoreAsync("rkey2", res.ToBytes);
        }
    }

    public void te()
    {
        var data2 = "".AsSpan().Slice(0, 1);
    }
}