using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest_Hash : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest_Hash(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task HSet()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.HSetAsync("testhash", new Dictionary<string, object>()
            {
                {"a1", "v1"},
                {"a2", "v2"},
                {"a3", "v3"},
                {"a4", "v4"},
            }
        );
    }

    [Fact]
    public async Task HDel()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.HDelAsync("testhash", new[] {"a1", "a2"}
        );
    }

    [Fact]
    public async Task HExistsAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.HExistsAsync("testhash", "a3"
        );
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task HGetAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.HGetAsync("testhash", "a3"
        );
        _testOutputHelper.WriteLine(res);
    }
}