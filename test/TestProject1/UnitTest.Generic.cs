using System.Net;
using RedisNaruto.Enums;
using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest_Generic : BaseUnit
{
  
    public UnitTest_Generic(ITestOutputHelper testOutputHelper):base(testOutputHelper)
    {
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

    [Fact]
    public async Task Test_ExpireAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ExpireAsync("key2", TimeSpan.FromMinutes(10));
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ExpireAtAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ExpireAtAsync("key2", 1681213694);
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ExpireTimeAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ExpireTimeAsync("key2");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_KeysAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.KeysAsync("key*");
        if (res is {Count: > 0})
        {
            foreach (var VARIABLE in res)
            {
                _testOutputHelper.WriteLine(VARIABLE);
            }
        }
    }

    [Fact]
    public async Task Test_MiGrateAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.MiGrateAsync(new[] {"key"}, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 55002),
            3,
            TimeSpan.FromMinutes(100), false, false, AuthEnum.Auth2, "redispw", "default");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_MoveAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.MoveAsync("key", 3);
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ObjectEncodingAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ObjectEncodingAsync("key1");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_PersistAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.PersistAsync("key2");
        _testOutputHelper.WriteLine(res.ToString());
    }


    [Fact]
    public async Task Test_PExpireAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.PExpireAsync("key2", TimeSpan.FromMinutes(10));
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_PExpireAtAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.PExpireAtAsync("key2", 1681386603000);
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_PExpireTimeAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.PExpireTimeAsync("key2");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_PTtlAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.PTtlAsync("key2");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_TtlAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.TtlAsync("key2");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_RandomKeyAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.RandomKeyAsync();
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ReNameAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ReNameAsync("key43", "key2");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ReNameNxAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ReNameNxAsync("key2", "key43");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_TypeAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.TypeAsync("key2");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_UnLinkAsync()
    {
        var redisCommand = await GetRedisAsync();
        await redisCommand.SetAsync("Test_UnLinkAsync", 1);
        await Task.Delay(1_000);
        var res = await redisCommand.UnLinkAsync(new[]
        {
            "Test_UnLinkAsync"
        });
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_WaitAsync()
    {
        var redisCommand = await GetRedisAsync();
        await redisCommand.SetAsync("Test_WaitAsync", 1);

        var res = await redisCommand.WaitAsync(1, TimeSpan.FromSeconds(1));
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_WaitAofAsync()
    {
        var redisCommand = await GetRedisAsync();
        await redisCommand.SetAsync("WaitAofAsync", 1);

        var res = await redisCommand.WaitAofAsync(0, 1, TimeSpan.FromSeconds(1));
        _testOutputHelper.WriteLine(res.ToString());
    }
    [Fact]
    public async Task Test_TouchAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.TouchAsync(new[]
        {
            "key1"
        });
        _testOutputHelper.WriteLine(res.ToString());
    }
}