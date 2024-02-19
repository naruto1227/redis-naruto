using RedisNaruto.Enums;
using RedisNaruto.Models;
using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest_ZSet : BaseUnit
{

    public UnitTest_ZSet(ITestOutputHelper testOutputHelper):base(testOutputHelper)
    {
    }

    [Fact]
    public async Task Test_ZAddAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZAddAsync("zsettest", new[]
        {
            new SortedSetAddModel(2, "test")
        }, SortedSetAddEnum.LessThan);
        _testOutputHelper.WriteLine(res.ToString());
    }


    [Fact]
    public async Task ZScanAsync()
    {
        var redisCommand = await GetRedisAsync();
        await foreach (var item in redisCommand.ZScanAsync("zsettest"
                       ))
        {
            foreach (var itemDic in item)
            {
                _testOutputHelper.WriteLine($"key={itemDic.Key},value={itemDic.Value}");
            }
        }
    }

    [Fact]
    public async Task ZCardAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZCardAsync("zsettest");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task ZCountAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZCountAsync("zsettest");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ZDiffAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZDiffAsync(new[]
        {
            "zsettest",
            "zsettest2"
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
    public async Task Test_ZDiffWithScoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZDiffWithScoreAsync(new[]
        {
            "zsettest",
            "zsettest2"
        });
        if (res is {Count: > 0})
        {
            foreach (var itemDic in res)
            {
                _testOutputHelper.WriteLine($"key={itemDic.Key},value={itemDic.Value}");
            }
        }
    }

    [Fact]
    public async Task Test_ZDiffStoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZDiffStoreAsync("testzset43", new[]
        {
            "zsettest",
            "zsettest2"
        });
    }

    [Fact]
    public async Task Test_ZIncrByAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZIncrByAsync("testzset43", "test", 5);
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ZScoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZScoreAsync("testzset43", "test");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ZMScoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZMScoreAsync("testzset43", new[]
        {
            "test"
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
    public async Task Test_ZUnionStoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZUnionStoreAsync("zsetdest", new[]
        {
            "zsettest",
            "testzset43"
        }, new long[]
        {
            3, 4
        }, SortedSetAggregateEnum.Sum);
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ZInterCardAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZInterCardAsync(new[] {"zsetdest"});
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ZInterStoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZInterStoreAsync("ZInterStoreAsync", new[] {"zsetdest"});
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ZLexCountAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZLexCountAsync("ZInterStoreAsync");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ZInterAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZInterAsync(new[]
        {
            "zsettest",
            "zsettest2"
        });
        if (res is {Count: > 0})
        {
            foreach (var itemDic in res)
            {
                _testOutputHelper.WriteLine($"value={itemDic}");
            }
        }

        var res2 = await redisCommand.ZInterWithScoreAsync(new[]
        {
            "zsettest",
            "zsettest2"
        });
        if (res2 is {Count: > 0})
        {
            foreach (var itemDic in res2)
            {
                _testOutputHelper.WriteLine($"key={itemDic.Key},value={itemDic.Value}");
            }
        }
    }

    [Fact]
    public async Task Test_ZUnionAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZUnionAsync(new[]
        {
            "zsettest",
            "zsettest2"
        });
        if (res is {Count: > 0})
        {
            foreach (var itemDic in res)
            {
                _testOutputHelper.WriteLine($"value={itemDic}");
            }
        }

        var res2 = await redisCommand.ZUnionWithScoreAsync(new[]
        {
            "zsettest",
            "zsettest2"
        });
        if (res2 is {Count: > 0})
        {
            foreach (var itemDic in res2)
            {
                _testOutputHelper.WriteLine($"key={itemDic.Key},value={itemDic.Value}");
            }
        }
    }

    [Fact]
    public async Task Test_ZMpopAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZMpopAsync(new[]
        {
            "zsettest",
            "zsettest2"
        }, SortedSetMinMaxEnum.Max, 7);
        if (res.key != default)
        {
            _testOutputHelper.WriteLine(res.key);
            foreach (var itemDic in res.data)
            {
                _testOutputHelper.WriteLine($"key={itemDic.Key},value={itemDic.Value}");
            }
        }
    }

    [Fact]
    public async Task Test_ZPopMaxAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZPopMaxAsync("zsetdest", 2);
        if (res is {Count: > 0})
        {
            foreach (var itemDic in res)
            {
                _testOutputHelper.WriteLine($"key={itemDic.Key},value={itemDic.Value}");
            }
        }
    }

    [Fact]
    public async Task Test_ZPopMinAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZPopMinAsync("zsetdest", 2);
        if (res is {Count: > 0})
        {
            foreach (var itemDic in res)
            {
                _testOutputHelper.WriteLine($"key={itemDic.Key},value={itemDic.Value}");
            }
        }
    }

    [Fact]
    public async Task Test_ZRandMemberWithScoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZRandMemberWithScoreAsync("zsetdest", 2);
        if (res is {Count: > 0})
        {
            foreach (var itemDic in res)
            {
                _testOutputHelper.WriteLine($"key={itemDic.Member},value={itemDic.Score}");
            }
        }
    }

    [Fact]
    public async Task Test_ZRandMemberAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZRandMemberAsync("zsetdest", -22);
        if (res is {Count: > 0})
        {
            foreach (var itemDic in res)
            {
                _testOutputHelper.WriteLine($"key={itemDic}");
            }
        }
    }

    [Fact]
    public async Task Test_ZRangeAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZRangeAsync("zsetdest");
        if (res is {Count: > 0})
        {
            foreach (var itemDic in res)
            {
                _testOutputHelper.WriteLine($"key={itemDic}");
            }
        }

        var res2 = await redisCommand.ZRangeAsync("zsetdest", "-inf", "+inf", false, 0, 0,
            SortedSetScoreLexEnum.ByScore);
        if (res2 is {Count: > 0})
        {
            foreach (var itemDic in res2)
            {
                _testOutputHelper.WriteLine($"key={itemDic}");
            }
        }
    }

    [Fact]
    public async Task Test_ZRangeWithScoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ZRangeWithScoreAsync("zsetdest");
        if (res is {Count: > 0})
        {
            foreach (var itemDic in res)
            {
                _testOutputHelper.WriteLine($"key={itemDic.Member},score={itemDic.Score}");
            }
        }

        var res2 = await redisCommand.ZRangeWithScoreAsync("zsetdest", "-inf", "+inf", false, 0, 0,
            SortedSetScoreLexEnum.ByScore);
        if (res2 is {Count: > 0})
        {
            foreach (var itemDic in res2)
            {
                _testOutputHelper.WriteLine($"key={itemDic.Member},score={itemDic.Score}");
            }
        }
    }

    [Fact]
    public async Task Test_BZMpopAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.BZMpopAsync(new[]
        {
            "zsettest",
            "zsettest2"
        }, TimeSpan.FromSeconds(10), SortedSetMinMaxEnum.Max, 7);
        if (res.key != default)
        {
            _testOutputHelper.WriteLine(res.key);
            foreach (var itemDic in res.data)
            {
                _testOutputHelper.WriteLine($"key={itemDic.Key},value={itemDic.Value}");
            }
        }
    }

    [Fact]
    public async Task Test_ZRangeStoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res2 = await redisCommand.ZRangeStoreAsync("Test_ZRangeStoreAsync", "zsetdest", "-inf", "+inf", false, 0, 0,
            SortedSetScoreLexEnum.ByScore);
        _testOutputHelper.WriteLine(res2.ToString());
    }

    [Fact]
    public async Task Test_ZRankAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res2 = await redisCommand.ZRankAsync("Test_ZRangeStoreAsync", "4444zsetdest");
        _testOutputHelper.WriteLine(res2.ToString());
    }

    [Fact]
    public async Task Test_ZRevRankAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res2 = await redisCommand.ZRevRankAsync("Test_ZRangeStoreAsync", "4444zsetdest");
        _testOutputHelper.WriteLine(res2.ToString());
    }

    [Fact]
    public async Task Test_ZRemAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res2 = await redisCommand.ZRemAsync("Test_ZRangeStoreAsync", new[] {"4444zsetdest"});
        _testOutputHelper.WriteLine(res2.ToString());
    }

    [Fact]
    public async Task Test_ZRemRangeByLexAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res2 = await redisCommand.ZRemRangeByLexAsync("Test_ZRangeStoreAsync", "[a", "[zhang");
        _testOutputHelper.WriteLine(res2.ToString());
    }

    [Fact]
    public async Task Test_ZRemRangeByRankAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res2 = await redisCommand.ZRemRangeByRankAsync("Test_ZRangeStoreAsync", 1, 1);
        _testOutputHelper.WriteLine(res2.ToString());
    }

    [Fact]
    public async Task Test_ZRemRangeByScoreAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res2 = await redisCommand.ZRemRangeByScoreAsync("Test_ZRangeStoreAsync", "-inf", "(2");
        _testOutputHelper.WriteLine(res2.ToString());
    }

    [Fact]
    public async Task Test_BzPopMinAsync()
    {
        var redisCommand = await GetRedisAsync();
        await redisCommand.ZAddAsync("bzpop", new[]
        {
            new SortedSetAddModel
            {
                Member = "1231",
                Score = 12
            },
            new SortedSetAddModel
            {
                Member = "456",
                Score = 14
            },
        });
        var res2 = await redisCommand.BzPopMinAsync(new[]
        {
            "bzpop"
        }, TimeSpan.FromSeconds(5));
        if (res2.key == default)
        {
            return;
        }

        _testOutputHelper.WriteLine(res2.data.Member.ToString());

        _testOutputHelper.WriteLine(res2.data.Score.ToString());
    }

    [Fact]
    public async Task Test_BzPopMaxAsync()
    {
        var redisCommand = await GetRedisAsync();
        using (FileStream fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, "未命名.docx"),
                   FileMode.Open, FileAccess.Read))
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                await redisCommand.ZAddAsync("bzpopmax", new[]
                {
                    new SortedSetAddModel
                    {
                        Member = memoryStream.ToArray(),
                        Score = 1233
                    },
                });
            }
        }
        var res2 = await redisCommand.BzPopMaxAsync(new[]
        {
            "bzpopmax"
        }, TimeSpan.FromSeconds(5));
        if (res2.key == default)
        {
            return;
        }

        _testOutputHelper.WriteLine(res2.data.Member.ToString());

        _testOutputHelper.WriteLine(res2.data.Score.ToString());
    }
}