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

    [Fact]
    public async Task HGetAllAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.HGetAllAsync("testhash"
        );
        if (res != null)
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine($"key={item.Key},value={item.Value}");
            }
        }
    }

    [Fact]
    public async Task HIncrByAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.HIncrByAsync("testhash", "a7", 5.5
        );
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task HKeysAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.HKeysAsync("testhash"
        );
        if (res != null)
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine($"key={item}");
            }
        }
    }

    [Fact]
    public async Task HLenAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.HLenAsync("testhash"
        );
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task HMGetAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.HMGetAsync("testhash", new[]
            {
                "a3",
                "a7"
            }
        );
        if (res != null)
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(item);
            }
        }
    }

    [Fact]
    public async Task HRandFieldAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.HRandFieldAsync("testhash", -7
        );
        if (res != null)
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(item);
            }
        }
    }

    [Fact]
    public async Task HValsAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.HValsAsync("testhash"
        );
        if (res != null)
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(item);
            }
        }
    }

    [Fact]
    public async Task HStrLenAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.HStrLenAsync("testhash", "a7"
        );
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task HSetNxAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.HSetNxAsync("testhash", "a7", "v7"
        );
        _testOutputHelper.WriteLine(res.ToString());
        var res2 = await redisCommand.HSetNxAsync("testhash", "a8", "v8"
        );
        _testOutputHelper.WriteLine(res2.ToString());
    }

    [Fact]
    public async Task HScanAsync()
    {
        var redisCommand = await GetRedisAsync();
        await foreach (var item in redisCommand.HScanAsync("testhash"
                       ))
        {
            foreach (var itemDic in item)
            {
                _testOutputHelper.WriteLine($"key={itemDic.Key},value={itemDic.Value}");
            }
        }
    }
}