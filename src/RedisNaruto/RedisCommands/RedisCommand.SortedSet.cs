using System.Runtime.CompilerServices;
using RedisNaruto.Enums;
using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// sorted set 数据类型
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 添加zset 集合
    /// </summary>
    /// <param name="key"></param>
    /// <param name="values">存储的数据</param>
    /// <param name="type">操作类型</param>
    /// <param name="isIncr">指定此选项时的ZADD行为类似于ZINCRBY。在此模式下只能指定一个分数元素对。当指定了此元素 返回的值为更新完成的新的score</param>
    /// <param name="isCh">修改返回值，默认返回新添加的元素个数，修改为返回 总的变化的元素个数</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ZAddAsync(string key, SortedSetAddModel[] values,
        SortedSetAddEnum type = SortedSetAddEnum.No, bool isIncr = false, bool isCh = false,
        CancellationToken cancellationToken = default)
    {
        if (values is not {Length: > 0})
        {
            throw new ArgumentNullException(nameof(values));
        }

        if (isIncr && values is {Length: > 1})
        {
            throw new InvalidOperationException($"当{nameof(isIncr)}为true，values只允许插入一条");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var datas = new List<object>()
        {
            key
        };
        datas.IfAdd(type != SortedSetAddEnum.No, type switch
        {
            SortedSetAddEnum.Exists => "XX",
            SortedSetAddEnum.GreaterThan => "GT",
            SortedSetAddEnum.LessThan => "LT",
            _ => "NX"
        });

        datas.IfAdd(isCh, "CH");
        datas.IfAdd(isIncr, "INCR");
        foreach (var item in values)
        {
            datas.Add(item.Score);
            datas.Add(item.Member);
        }

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZAdd, datas.ToArray()));
        return result;
    }

    /// <summary>
    /// 扫描zset的数据
    /// https://redis.io/commands/scan/
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">条数</param>
    /// <param name="cancellationToken"></param>
    /// <param name="matchPattern">匹配条件</param>
    /// <returns></returns>
    public async IAsyncEnumerable<Dictionary<string, RedisValue>> ZScanAsync(string key,
        string matchPattern = "*", int count = 10,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (key.IsNullOrWhiteSpace())
        {
            yield break;
        }

        cancellationToken.ThrowIfCancellationRequested();

        //游标位置
        var cursor = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var resultList = await RedisResolver.InvokeAsync<object>(new Command(RedisCommandName.ZScan,
                new object[]
                {
                    key,
                    cursor,
                    "MATCH",
                    matchPattern,
                    "COUNT",
                    count
                }));
            if (resultList is not List<object> {Count: >= 2} list)
            {
                yield break;
            }

            //更新游标
            cursor = list[0].ToString().ToInt();
            if (list[1] is List<object> datas)
            {
                yield return datas.ToDic();
            }
            else
            {
                yield break;
            }

            if (cursor == 0)
            {
                yield break;
            }
        }
    }

    /// <summary>
    /// 返回zset中集合的长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> ZCardAsync(string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZCard, new[]
            {
                key,
            }));
        return result;
    }

    /// <summary>
    /// 返回区间范围中的元素数量
    /// </summary>
    /// <param name="key"></param>
    /// <param name="max"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="min"></param>
    /// <returns></returns>
    public async Task<long> ZCountAsync(string key, string min = "-inf", string max = "+inf",
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZCount, new[]
            {
                key,
                min,
                max
            }));
        return result;
    }

    /// <summary>
    /// 返回由第一个集合和所有后续集合之间的差异产生的集合成员。
    /// 类似 except
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<RedisValue>> ZDiffAsync(string[] key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.ZDiff, new object[]
            {
                key.Length
            }.Concat(key).ToArray())).ToRedisValueListAsync();
        return result;
    }

    /// <summary>
    /// 返回由第一个集合和所有后续集合之间的差异产生的集合成员。
    /// 类似 except
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Dictionary<RedisValue, long>> ZDiffWithScoreAsync(string[] key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.ZDiff, new object[]
        {
            key.Length
        }.Concat(key).Concat(new[] {"WITHSCORES"}).ToArray()));
        return await result.ToZSetDicAsync();
    }

    /// <summary>
    /// 此命令等于ZDIFF，但不是返回结果集，而是存储在destination.
    /// 将except的差异值存到目标destination
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ZDiffStoreAsync(string destination, string[] keys,
        CancellationToken cancellationToken = default)
    {
        if (destination.IsNullOrWhiteSpace() || keys is not {Length: > 0})
        {
            return default;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZDiffStore,
                new object[] {destination, keys.Length}
                    .Concat(keys).ToArray()));
        return result;
    }


    /// <summary>
    /// 递增zset元素中的score值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="increment"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public async Task<int> ZIncrByAsync(string key, object member, int increment = 1,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(member);
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZIncrBy, new object[]
            {
                key,
                increment,
                member
            }));
        return result;
    }


    /// <summary>
    /// 返回由所有给定集的交集产生的集的成员。 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="aggregate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="weights"></param>
    /// <returns></returns>
    public async Task<List<RedisValue>> ZInterAsync(string[] keys, long[] weights = null,
        SortedSetAggregateEnum aggregate = SortedSetAggregateEnum.Sum,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();
        var args = new List<object>()
        {
            keys.Length
        };
        args.AddRange(keys);
        if (weights is {Length: > 0})
        {
            args.Add("WEIGHTS");
            args.AddRange(weights.Cast<object>());
        }

        args.Add("AGGREGATE");
        args.Add(aggregate.ToString());

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.ZInter, args.ToArray()))
                .ToRedisValueListAsync();
        return result;
    }

    /// <summary>
    /// 返回由所有给定集的交集产生的集的成员。
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="aggregate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="weights"></param>
    /// <returns></returns>
    public async Task<Dictionary<RedisValue, long>> ZInterWithScoreAsync(string[] keys, long[] weights = null,
        SortedSetAggregateEnum aggregate = SortedSetAggregateEnum.Sum,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();
        var args = new List<object>()
        {
            keys.Length
        };
        args.AddRange(keys);
        if (weights is {Length: > 0})
        {
            args.Add("WEIGHTS");
            args.AddRange(weights.Cast<object>());
        }

        args.Add("AGGREGATE");
        args.Add(aggregate.ToString());
        args.Add("WITHSCORES");

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.ZInter, args.ToArray()))
                .ToZSetDicAsync();
        return result;
    }

    /// <summary>
    /// 计算并集 将结果存储到 dest
    /// </summary>
    /// <code>
    ///  这将对 zset1 的成员的分数乘以 2，对 zset2 的成员的分数乘以 3，然后计算它们的并集并将结果存储到 dest 中。
    /// ZUNIONSTORE result 2 zset1 zset2 WEIGHTS 2 3
    /// </code>
    /// <param name="dest">目标元素</param>
    /// <param name="keys">参与操作的key集合</param>
    /// <param name="weights">用于指定每个输入有序集合的权重。默认情况下，每个输入有序集合的权重为 1。</param>
    /// <param name="aggregate">用于指定对于具有相同成员的元素，如何计算它们的分数。默认情况下使用 SUM 计算。其他可选项为 MIN 和 MAX。</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ZUnionStoreAsync(string dest, string[] keys, long[] weights = null,
        SortedSetAggregateEnum aggregate = SortedSetAggregateEnum.Sum,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dest);
        ArgumentNullException.ThrowIfNull(keys);
        if (weights is {Length: > 0} && weights.Length != keys.Length)
        {
            throw new InvalidOperationException("weights需要和keys一一对应");
        }

        cancellationToken.ThrowIfCancellationRequested();
        var args = new List<object>()
        {
            dest,
            keys.Length
        };
        args.AddRange(keys);
        if (weights is {Length: > 0})
        {
            args.Add("WEIGHTS");
            args.AddRange(weights.Cast<object>());
        }

        args.Add("AGGREGATE");
        args.Add(aggregate.ToString());

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZUnionStore, args.ToArray()));
        return result;
    }

    /// <summary>
    /// 返回由所有给定集的交集产生的集的成员。 
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="aggregate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="weights"></param>
    /// <returns></returns>
    public async Task<List<RedisValue>> ZUnionAsync(string[] keys, long[] weights = null,
        SortedSetAggregateEnum aggregate = SortedSetAggregateEnum.Sum,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();
        var args = new List<object>()
        {
            keys.Length
        };
        args.AddRange(keys);
        if (weights is {Length: > 0})
        {
            args.Add("WEIGHTS");
            args.AddRange(weights.Cast<object>());
        }

        args.Add("AGGREGATE");
        args.Add(aggregate.ToString());

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.ZUnion, args.ToArray()))
                .ToRedisValueListAsync();
        return result;
    }

    /// <summary>
    /// 返回由所有给定集的交集产生的集的成员。
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="aggregate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="weights"></param>
    /// <returns></returns>
    public async Task<Dictionary<RedisValue, long>> ZUnionWithScoreAsync(string[] keys, long[] weights = null,
        SortedSetAggregateEnum aggregate = SortedSetAggregateEnum.Sum,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();
        var args = new List<object>()
        {
            keys.Length
        };
        args.AddRange(keys);
        if (weights is {Length: > 0})
        {
            args.Add("WEIGHTS");
            args.AddRange(weights.Cast<object>());
        }

        args.Add("AGGREGATE");
        args.Add(aggregate.ToString());
        args.Add("WITHSCORES");

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.ZUnion, args.ToArray()))
                .ToZSetDicAsync();
        return result;
    }

    ///  <summary>
    /// 获取元素的分数信息
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="member">元素</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    public async Task<long> ZScoreAsync(string key, object member,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZScore, new object[]
        {
            key,
            member
        }));
    }

    /// <summary>
    /// 返回交集的值数量 此命令类似于ZINTER，但它不返回结果集，而是仅返回结果的基数。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="limit">默认情况下，该命令计算所有给定集的交集的基数。当提供可选LIMIT参数（默认为 0，表示无限制）时，如果交集基数在计算中途达到 limit，则算法将退出并产生 limit 作为基数。这样的实现确保了限制低于实际交集基数的查询的显着加速。</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> ZInterCardAsync(string[] key, int limit = 0,
        CancellationToken cancellationToken = default)
    {
        if (key is not {Length: > 0})
        {
            return default;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZInterCard,
                new object[]
                {
                    key.Length
                }.Concat(
                    key).Concat(new[] {"LIMIT", limit.ToString()}).ToArray()));
        return result;
    }

    /// <summary>
    /// 此命令等于ZInter，但不是返回结果集，而是存储在destination.
    /// 将except的差异值存到目标destination
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="keys"></param>
    /// <param name="aggregate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="weights"></param>
    /// <returns></returns>
    public async Task<int> ZInterStoreAsync(string destination, string[] keys, long[] weights = null,
        SortedSetAggregateEnum aggregate = SortedSetAggregateEnum.Sum,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();
        var args = new List<object>()
        {
            destination,
            keys.Length
        };
        args.AddRange(keys);
        if (weights is {Length: > 0})
        {
            args.Add("WEIGHTS");
            args.AddRange(weights.Cast<object>());
        }

        args.Add("AGGREGATE");
        args.Add(aggregate.ToString());

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZInterStore,
                args.ToArray()));
        return result;
    }

    ///  <summary>
    /// 当排序集中的所有元素都以相同的分数插入时，为了强制按字典顺序排序，此命令返回排序集中的元素数，其值介于key和min之间max。
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="min"></param>
    ///  <param name="max"></param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    public async Task<long> ZLexCountAsync(string key, string min = "-", string max = "+",
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZLexCount, new object[]
        {
            key,
            min,
            max
        }));
    }

    ///  <summary>
    /// 从提供的键名列表中的第一个非空排序集中弹出一个或多个元素，即成员分数对。
    ///  </summary>
    ///  <param name="keys"></param>
    ///  <param name="minMax">标记从最小的分数 还是最大的分数弹出元素的数量</param>
    ///  <param name="count">弹出的数量</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    public async Task<(string key, Dictionary<RedisValue, long> data)> ZMpopAsync(string[] keys,
        SortedSetMinMaxEnum minMax = SortedSetMinMaxEnum.Min,
        long count = 1,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        var args = new List<object>()
        {
            keys.Length
        };
        args.AddRange(keys);
        args.Add(minMax.ToString());
        args.Add("COUNT");
        args.Add(count);
        cancellationToken.ThrowIfCancellationRequested();

        var result = await RedisResolver.InvokeAsync<object>(new Command(RedisCommandName.ZMpop, args.ToArray()));
        //判断是否有数据
        if (result is not List<object> {Count: >= 2} resultList || resultList[1] is not List<object> valueList)
            return default;

        var dic = new Dictionary<RedisValue, long>();
        foreach (var item in valueList)
        {
            if (item is List<object> {Count: >= 2} datas)
            {
                dic.Add((RedisValue) datas[0], datas[1].ToString().ToLong());
            }
        }

        return (resultList[0].ToString(), dic);
    }

    ///  <summary>
    /// 从提供的键名列表中的第一个非空排序集中弹出一个或多个元素，即成员分数对。
    /// 阻塞
    ///  </summary>
    ///  <param name="keys"></param>
    ///  <param name="timeout">超时时间</param>
    ///  <param name="minMax">标记从最小的分数 还是最大的分数弹出元素的数量</param>
    ///  <param name="count">弹出的数量</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    public async Task<(string key, Dictionary<RedisValue, long> data)> BZMpopAsync(string[] keys, TimeSpan timeout,
        SortedSetMinMaxEnum minMax = SortedSetMinMaxEnum.Min,
        long count = 1,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        var args = new List<object>()
        {
            timeout.TotalSeconds,
            keys.Length
        };
        args.AddRange(keys);
        args.Add(minMax.ToString());
        args.Add("COUNT");
        args.Add(count);
        cancellationToken.ThrowIfCancellationRequested();

        var result = await RedisResolver.InvokeAsync<object>(new Command(RedisCommandName.BZMpop, args.ToArray()));
        //判断是否有数据
        if (result is not List<object> {Count: >= 2} resultList || resultList[1] is not List<object> valueList)
            return default;

        var dic = new Dictionary<RedisValue, long>();
        foreach (var item in valueList)
        {
            if (item is List<object> {Count: >= 2} datas)
            {
                dic.Add((RedisValue) datas[0], datas[1].ToString().ToLong());
            }
        }

        return (resultList[0].ToString(), dic);
    }

    ///  <summary>
    /// 获取元素的分数信息
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="members">元素</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    public async Task<List<RedisValue>> ZMScoreAsync(string key, object[] members,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.ZMScore, new object[]
        {
            key
        }.Concat(members).ToArray())).ToRedisValueListAsync();
    }

    /// <summary>
    /// 删除并返回count存储在 的排序集中得分最高的成员key。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<Dictionary<RedisValue, long>> ZPopMaxAsync(string key, int count = 1,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.ZPopMax, new object[]
                {
                    key,
                    count
                }))
                .ToZSetDicAsync();
        return result;
    }

    /// <summary>
    /// 删除并返回count存储在 的排序集中得分最低的成员key。
    /// 当返回多个元素时，得分最低的将是第一个，其次是得分较大的元素。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<Dictionary<RedisValue, long>> ZPopMinAsync(string key, int count = 1,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.ZPopMin, new object[]
                {
                    key,
                    count
                }))
                .ToZSetDicAsync();
        return result;
    }

    /// <summary>
    /// 移除最大的元素
    /// 阻塞版本
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async Task<(string key, SortedSetModel data)> BzPopMaxAsync(string[] key, TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.BZPopMax, key.Concat(new string[]
            {
                timeout.TotalSeconds.ToString()
            }).ToArray())).ToListAsync();
        if (result is not {Count: > 2})
        {
            return default;
        }


        return (result[0].ToString(), new SortedSetModel((RedisValue) result[1])
        {
            Score = result[2].ToString().ToLong()
        });
    }

    /// <summary>
    /// 移除最小的元素
    /// 阻塞版本
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public async Task<(string key, SortedSetModel data)> BzPopMinAsync(string[] key, TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.BZPopMin, key.Concat(new string[]
            {
                timeout.TotalSeconds.ToString()
            }).ToArray())).ToListAsync();

        if (result is not {Count: > 2})
        {
            return default;
        }


        return (result[0].ToString(), new SortedSetModel((RedisValue) result[1])
        {
            Score = result[2].ToString().ToLong()
        });
    }

    /// <summary>
    /// 当仅使用key参数调用时，从存储在 的已排序集合值中返回一个随机元素key。
    /// 如果提供的参数为正，则返回不同元素count的数组。数组的长度是排序集的基数 ( ) 之一，以较小者为准。
    ///如果用否定调用count，行为会改变，命令可以多次返回相同的元素。在这种情况下，返回的元素数是指定的绝对值count
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<RedisValue>> ZRandMemberAsync(string key, int count = 1,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.ZRandMember, new object[]
            {
                key, count
            })).ToRedisValueListAsync();
        return result;
    }

    /// <summary>
    /// 当仅使用key参数调用时，从存储在 的已排序集合值中返回一个随机元素key。
    /// 如果提供的参数为正，则返回不同元素count的数组。数组的长度是排序集的基数 ( ) 之一，以较小者为准。
    ///如果用否定调用count，行为会改变，命令可以多次返回相同的元素。在这种情况下，返回的元素数是指定的绝对值count
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<SortedSetModel>> ZRandMemberWithScoreAsync(string key, int count = 1,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.ZRandMember, new object[]
            {
                key, count,
                "WITHSCORES"
            })).ToZSetAsync();
        return result;
    }

    /// <summary>
    /// 查询zset中数据的信息 默认是从低到高
    /// </summary>
    /// <code>
    /// ZRANGE zset (1 5 BYSCORE
    /// while将返回所有元素1 < score <= 5：
    /// 
    /// ZRANGE zset (5 (10 BYSCORE
    /// 将返回所有元素5 < score < 10（排除 5 和 10）
    /// </code>
    /// <param name="key"></param>
    /// <param name="start">开始位置 (-inf正无穷负无穷 需要搭配ByScore使用）, 在分数前加上 ( 符号 代表排除当前值</param>
    /// <param name="stop">结束位置 (-inf正无穷负无穷 需要搭配ByScore使用）, 在分数前加上 ( 符号 代表排除当前值</param>
    /// <param name="isLimit">6.2.0 开启limit</param>
    /// <param name="count">需要开启isLimit </param>
    /// <param name="offset">需要开启isLimit</param>
    /// <param name="scoreLex">6.2.0 </param>
    /// <param name="rev">6.2.0
    /// 默认的返回值顺序的从低到高，true 代表反转数据，从高到低
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<RedisValue>> ZRangeAsync(string key, string start = "0", string stop = "1",
        bool isLimit = false, int offset = 0, int count = 0,
        SortedSetScoreLexEnum scoreLex = SortedSetScoreLexEnum.Defaut, bool rev = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        var argv = new List<object>()
        {
            key,
            start, stop,
        };
        if (rev)
        {
            argv.Add("REV");
        }

        if (scoreLex != SortedSetScoreLexEnum.Defaut)
        {
            argv.Add(scoreLex.ToString());
        }

        if (isLimit)
        {
            argv.Add(offset);
            argv.Add(count);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.ZRange,
                argv.ToArray())).ToRedisValueListAsync();
        return result;
    }


    /// <summary>
    /// 查询zset中数据的信息 默认是从低到高
    /// </summary>
    /// <code>
    /// ZRANGE zset (1 5 BYSCORE
    /// while将返回所有元素1 < score <= 5：
    /// 
    /// ZRANGE zset (5 (10 BYSCORE
    /// 将返回所有元素5 < score < 10（排除 5 和 10）
    /// </code>
    /// <param name="key"></param>
    /// <param name="start">开始位置 (-inf正无穷负无穷 需要搭配ByScore使用）, 在分数前加上 ( 符号 代表排除当前值</param>
    /// <param name="stop">结束位置 (-inf正无穷负无穷 需要搭配ByScore使用）, 在分数前加上 ( 符号 代表排除当前值</param>
    /// <param name="isLimit">6.2.0 开启limit</param>
    /// <param name="count">需要开启isLimit </param>
    /// <param name="offset">需要开启isLimit</param>
    /// <param name="scoreLex">6.2.0 </param>
    /// <param name="rev">6.2.0
    /// 默认的返回值顺序的从低到高，true 代表反转数据，从高到低
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<SortedSetModel>> ZRangeWithScoreAsync(string key, string start = "0", string stop = "1",
        bool isLimit = false, int offset = 0, int count = 0,
        SortedSetScoreLexEnum scoreLex = SortedSetScoreLexEnum.Defaut, bool rev = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        var argv = new List<object>()
        {
            key,
            start, stop,
        };
        if (rev)
        {
            argv.Add("REV");
        }

        if (scoreLex != SortedSetScoreLexEnum.Defaut)
        {
            argv.Add(scoreLex.ToString());
        }

        if (isLimit)
        {
            argv.Add(offset);
            argv.Add(count);
        }

        argv.Add("WITHSCORES");
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.ZRange,
                argv.ToArray())).ToZSetAsync();
        return result;
    }

    /// <summary>
    /// 此命令类似于ZRANGE，但将结果存储在<dest>目标键中
    /// </summary>
    /// <code>
    /// </code>
    /// <param name="dest">目标元素</param>
    /// <param name="key"></param>
    /// <param name="start">开始位置 (-inf正无穷负无穷 需要搭配ByScore使用）, 在分数前加上 ( 符号 代表排除当前值</param>
    /// <param name="stop">结束位置 (-inf正无穷负无穷 需要搭配ByScore使用）, 在分数前加上 ( 符号 代表排除当前值</param>
    /// <param name="isLimit">6.2.0 开启limit</param>
    /// <param name="count">需要开启isLimit </param>
    /// <param name="offset">需要开启isLimit</param>
    /// <param name="scoreLex">6.2.0 </param>
    /// <param name="rev">6.2.0
    /// 默认的返回值顺序的从低到高，true 代表反转数据，从高到低
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ZRangeStoreAsync(string dest, string key, string start = "0", string stop = "1",
        bool isLimit = false, int offset = 0, int count = 0,
        SortedSetScoreLexEnum scoreLex = SortedSetScoreLexEnum.Defaut, bool rev = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dest);
        ArgumentNullException.ThrowIfNull(key);
        var argv = new List<object>()
        {
            dest,
            key,
            start, stop,
        };
        if (rev)
        {
            argv.Add("REV");
        }

        if (scoreLex != SortedSetScoreLexEnum.Defaut)
        {
            argv.Add(scoreLex.ToString());
        }

        if (isLimit)
        {
            argv.Add(offset);
            argv.Add(count);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZRangeStore,
                argv.ToArray()));
        return result;
    }

    /// <summary>
    /// 从低到高返回member对应的名次信息 默认从0 开始
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ZRankAsync(string key, object member,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(member);
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZRank, new object[]
            {
                key, member
            }));
        return result;
    }

    /// <summary>
    /// member返回存储在 的已排序集合中的排名key，分数从高到低排序。排名（或索引）从 0 开始，这意味着得分最高的成员具有排名0。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ZRevRankAsync(string key, object member,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(member);
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZRevRank, new object[]
            {
                key, member
            }));
        return result;
    }

    /// <summary>
    /// 从存储在 的排序集中删除指定成员key。不存在的成员将被忽略。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ZRemAsync(string key, object[] member,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(member);
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZRem, new object[]
            {
                key
            }.Concat(member).ToArray()));
        return result;
    }

    /// <summary>
    /// 此命令删除存储在 和 指定的字典序范围之间的有序集合中的所有key元素。
    /// 按照member删除
    /// </summary>
    /// <param name="key"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ZRemRangeByLexAsync(string key, string min, string max,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZRemRangeByLex, new object[]
            {
                key,
                min,
                max
            }));
        return result;
    }

    /// <summary>
    /// 按照排行从低到高删除
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start">排行开始</param>
    /// <param name="stop">排行结束</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ZRemRangeByRankAsync(string key, int start, int stop,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZRemRangeByRank, new object[]
            {
                key,
                start,
                stop
            }));
        return result;
    }

    /// <summary>
    /// 删除score 分数之间的所有元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ZRemRangeByScoreAsync(string key, string min, string max,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.ZRemRangeByScore, new object[]
            {
                key,
                min,
                max
            }));
        return result;
    }
}