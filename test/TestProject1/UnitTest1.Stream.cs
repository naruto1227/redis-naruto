using Newtonsoft.Json;
using RedisNaruto.Enums;
using RedisNaruto.Models;
using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest1_Stream : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest1_Stream(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Test_XAddAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XAddAsync("stream", new Dictionary<string, object>()
        {
            {"a", "helloword"},
            {"b", "helloword2"},
        }, "*", StreamMaxMinEnum.MaxNearly, "100", null, true);
        if (res != null)
        {
            _testOutputHelper.WriteLine(res.ToString());
        }
    }

    [Fact]
    public async Task Test_XGroupCreateAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XGroupCreateAsync("stream", "testgroup2");
    }

    [Fact]
    public async Task Test_XGroupCreateConsumerAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XGroupCreateConsumerAsync("stream", "testgroup", "testconsumer");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_XGroupInfoAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XGroupInfoAsync("stream");
        if (res != null)
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(JsonConvert.SerializeObject(item));
            }
        }
    }

    [Fact]
    public async Task Test_XConsumerInfoAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XConsumerInfoAsync("stream", "testgroup");
        if (res != null)
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(JsonConvert.SerializeObject(item));
            }
        }
    }

    [Fact]
    public async Task Test_XReadAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XReadAsync(new[]
        {
            new ReadStreamOffset("mystream3", ReadStreamOffset.BeginMessage)
        }, 10);
        if (res != null)
        {
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(JsonConvert.SerializeObject(item));
            }
        }
    }

    [Fact]
    public async Task Test_XClaimAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XClaimAsync("stream", "testgroup", "testconsumer2", new[]
        {
            "1680175374752-0"
        }, TimeSpan.FromSeconds(1));
        if (res != null)
        {
            // foreach (var item in res)
            // {
            //     _testOutputHelper.WriteLine(JsonConvert.SerializeObject(item));
            // }
        }
    }

    [Fact]
    public async Task Test_XClaimWithIdAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XClaimWithIdAsync("stream", "testgroup", "testconsumer2", new[]
        {
            "1680175383112-0"
        }, TimeSpan.FromSeconds(1));
        if (res != null)
        {
            // foreach (var item in res)
            // {
            //     _testOutputHelper.WriteLine(JsonConvert.SerializeObject(item));
            // }
        }
    }

    [Fact]
    public async Task Test_XAutoClaimAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XAutoClaimAsync("stream", "testgroup", "testconsumer2", TimeSpan.FromSeconds(1),
            "0-0");
        
        var res2 = await redisCommand.XAutoClaimWithIdAsync("stream", "testgroup", "testconsumer", TimeSpan.FromSeconds(1),
            "0-0");
        
    }
    [Fact]
    public async Task Test_XGroupSetIdAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XGroupSetIdAsync("stream", "testgroup",
            "0-0");
    }

    [Fact]
    public async Task Test_XReadGroupAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XReadGroupAsync(new[
        ]
        {
            new ReadGroupStreamOffset("stream", ReadGroupStreamOffset.NewMessage)
        }, "testgroup", "testconsumer2", 10);
        if (res != null)
        {
            // foreach (var item in res)
            // {
            //     _testOutputHelper.WriteLine(JsonConvert.SerializeObject(item));
            // }
        }
    }

    [Fact]
    public async Task Test_XPendIngAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XPendingAsync("stream", "testgroup", 10, null,
            null, null, TimeSpan.FromSeconds(180));
        if (res != null)
        {
            // foreach (var item in res)
            // {
            //     _testOutputHelper.WriteLine(JsonConvert.SerializeObject(item));
            // }
        }
    }

    [Fact]
    public async Task Test_XRangeAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XRangeAsync("stream", "-", "+", 10);
        if (res != null)
        {
            // foreach (var item in res)
            // {
            //     _testOutputHelper.WriteLine(JsonConvert.SerializeObject(item));
            // }
        }
    }

    [Fact]
    public async Task Test_XRevRangeAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XRevRangeAsync("stream", "+", "-", 10);
        if (res != null)
        {
            // foreach (var item in res)
            // {
            //     _testOutputHelper.WriteLine(JsonConvert.SerializeObject(item));
            // }
        }
    }

    [Fact]
    public async Task Test_XStreamInfoAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XStreamInfoAsync("stream");
        // if (res!=default)
        // {
        //     _testOutputHelper.WriteLine(JsonConvert.SerializeObject(res));
        // }
    }

    [Fact]
    public async Task Test_XGroupDeleteConsumerAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XGroupDeleteConsumerAsync("stream", "testgroup", "testconsumer2");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_XGroupDestroyAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XGroupDestroyAsync("stream", "testgroup");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_XTrimAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XTrimAsync("stream", StreamMaxMinEnum.MaxPrecision, "1", null);
        if (res != null)
        {
            _testOutputHelper.WriteLine(res.ToString());
        }
    }

    [Fact]
    public async Task Test_XDelAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XDelAsync("stream", new[]
        {
            "1679911496233-1"
        });
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_XLenAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.XLenAsync("stream");
        _testOutputHelper.WriteLine(res.ToString());
    }
}