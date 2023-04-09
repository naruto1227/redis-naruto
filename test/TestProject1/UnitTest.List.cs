using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest_List : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest_List(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Test_LSetAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.LSetAsync("listtest", 0, "1");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_LInsertAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.LInsertAsync("listtest", "1", "2", true);
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_RPushAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.RPushAsync("listtest", new[]
        {
            "3",
            "4"
        });
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_LPushAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.LPushAsync("listtest", new[]
        {
            "3",
            "4"
        });
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_LTrimAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.LTrimAsync("listtest", 0, -2);
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_RPushxAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.RPushxAsync("listtestx", new[]
        {
            "3",
            "4"
        });
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_LPushxAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.LPushxAsync("listtest", new[]
        {
            "3",
            "4"
        });
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_RPopAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = redisCommand.RPopAsync("listtest", 4);
        await foreach (var item in res)
        {
            _testOutputHelper.WriteLine(item.ToString());
        }
    }

    [Fact]
    public async Task Test_BRPopAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.BRPopAsync(new[]
        {
            "listtest3",
            "listtest"
        }, TimeSpan.FromSeconds(5));
        if (res.key != default)
        {
            _testOutputHelper.WriteLine(res.key);
        }

        if (!res.value.IsEmpty())
        {
            _testOutputHelper.WriteLine(res.value.ToString());
        }
    }

    [Fact]
    public async Task Test_BLPopAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.BLPopAsync(new[]
        {
            "listtest3",
            "listtest"
        }, TimeSpan.FromSeconds(3));
        if (res.key != default)
        {
            _testOutputHelper.WriteLine(res.key);
        }

        if (!res.value.IsEmpty())
        {
            _testOutputHelper.WriteLine(res.value.ToString());
        }
    }

    [Fact]
    public async Task Test_LPopAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = redisCommand.LPopAsync("listtest", 2);
        await foreach (var item in res)
        {
            _testOutputHelper.WriteLine(item.ToString());
        }
    }

    [Fact]
    public async Task Test_LRemAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.LRemAsync("listtest", 2, "2");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_LRangeAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = redisCommand.LRangeAsync("listtest", 0, 5);
        await foreach (var item in res)
        {
            _testOutputHelper.WriteLine(item.ToString());
        }
    }

    [Fact]
    public async Task Test_LLenAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.LLenAsync("listtest");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_LIndexAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.LIndexAsync("listtest", 1);
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_LPosAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.LPosAsync("listtest", 2);
        foreach (var item in res)
        {
            _testOutputHelper.WriteLine(item.ToString());
        }
    }

    [Fact]
    public async Task Test_LmPopAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.LmPopAsync(new[] {"listtest", "listtest2", "listtest3"});
        if (res != default)
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(item.Key);
            }
        }
    }

    [Fact]
    public async Task Test_BLmPopAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.BlMPopAsync(new[] {"listtest", "listtest2", "listtest3"},TimeSpan.FromSeconds(1));
        if (res != default)
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(item.Key);
            }
        }
    }
    [Fact]
    public async Task Test_LMoveAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.LMoveAsync("listtest", "listtest3");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_BLMoveAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.BLMoveAsync("listtest", "listtest3", TimeSpan.FromSeconds(1));
        _testOutputHelper.WriteLine(res.ToString());
    }
}