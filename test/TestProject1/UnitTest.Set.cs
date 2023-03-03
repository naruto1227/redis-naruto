using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest_Set : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest_Set(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Test_SAddAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SAddAsync("testobjset", new object[]
        {
            1
        });
        var res2 = await redisCommand.SAddAsync("testobjset2", new object[]
        {
            1
        });
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_SCardAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SCardAsync("testobjset");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_SDiffAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SDiffAsync(new[]
        {
            "testobjset",
            "testobjset2"
        });
        if (res is {Count: > 0})
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
        }

        _testOutputHelper.WriteLine("======================");
        var res2 = await redisCommand.SDiffAsync(new[]
        {
            "testobjset2",
            "testobjset"
        });
        if (res2 is {Count: > 0})
        {
            foreach (var item in res2)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
        }
    }

    [Fact]
    public async Task Test_SDiffStoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SDiffStoreAsync("settestkey", new[] {"key1", "key2"});
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_SInterAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SInterAsync(new[] {"key1", "key2"});
        if (res is {Count: > 0})
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
        }
    }

    [Fact]
    public async Task Test_SInterCardAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SInterCardAsync(new[] {"key1", "key2"});
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_SInterStoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SInterStoreAsync("SInterStoreAsync", new[] {"key1", "key2"});
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_SisMemberAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SisMemberAsync("SInterStoreAsync", "c");
        _testOutputHelper.WriteLine(res.ToString());
        var res2 = await redisCommand.SisMemberAsync("SInterStoreAsync", "b");
        _testOutputHelper.WriteLine(res2.ToString());
    }

    [Fact]
    public async Task Test_SmemBersAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SMembersAsync("SInterStoreAsync");
        if (res is {Count: > 0})
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
        }
    }

    [Fact]
    public async Task Test_SmisMemberAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SmisMemberAsync("SInterStoreAsync", new string[]
        {
            "c", "d"
        });
        if (res is {Count: > 0})
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
        }
    }

    [Fact]
    public async Task Test_SMoveAsync()
    {
        var redisCommand = await GetRedisAsync();

        var res = await redisCommand.SMoveAsync("key1", "asdasd", "a");
        _testOutputHelper.WriteLine(res.ToString());

        var res2 = await redisCommand.SMembersAsync("asdasd");
        if (res2 is {Count: > 0})
        {
            foreach (var item in res2)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
        }
    }

    [Fact]
    public async Task Test_SPopAsync()
    {
        var redisCommand = await GetRedisAsync();

        var res2 = await redisCommand.SPopAsync("pop", 2);
        if (res2 is {Count: > 0})
        {
            foreach (var item in res2)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
        }
    }

    [Fact]
    public async Task Test_SRandMemberAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SRandMemberAsync("key1", -3);
        if (res is {Count: > 0})
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
        }
    }

    [Fact]
    public async Task Test_SRemAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SRemAsync("SInterStoreAsync", new[] {"c", "key2"});
        _testOutputHelper.WriteLine(res.ToString());
    }
    
    [Fact]
    public async Task Test_SUnionAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SUnionAsync(new []{"key1","key2"});
        if (res is {Count: > 0})
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
        }
    }
    [Fact]
    public async Task Test_SUnionStoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SUnionStoreAsync("SUnionStoreAsync",new []{"key1","key2"});
        _testOutputHelper.WriteLine(res.ToString());

    }
    [Fact]
    public async Task Test_SScanAsync()
    {
        var redisCommand = await GetRedisAsync();
        await foreach (var item in redisCommand.SScanAsync("key1"
                       ))
        {
            foreach (var itemDic in item)
            {
                _testOutputHelper.WriteLine($"value={itemDic}");
            }
        }
    }
}