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
}