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
        if (type != SortedSetAddEnum.No)
        {
            datas.Add(type switch
            {
                SortedSetAddEnum.Exists => "XX",
                SortedSetAddEnum.GreaterThan => "GT",
                SortedSetAddEnum.LessThan => "LT",
                _ => "NX"
            });
        }

        if (isCh)
            datas.Add("CH");

        if (isIncr)
            datas.Add("INCR");
        foreach (var item in values)
        {
            datas.Add(item.Score);
            datas.Add(item.Member);
        }

        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<int>(new Command(RedisCommandName.ZAdd, datas.ToArray()));
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
    public async IAsyncEnumerable<Dictionary<string, string>> ZScanAsync(string key,
        string matchPattern = "*", int count = 10,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (key.IsNullOrWhiteSpace())
        {
            yield break;
        }

        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        //游标位置
        var cursor = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var resultList = await client.ExecuteAsync<object>(new Command(RedisCommandName.ZScan,
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
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<long>(new Command(RedisCommandName.ZCard, new[]
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
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<long>(new Command(RedisCommandName.ZCount, new[]
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
    public async Task<List<object>> ZDiffAsync(string[] key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<List<object>>(new Command(RedisCommandName.ZDiff, new object[]
            {
                key.Length
            }.Concat(key).ToArray()));
        return result;
    }

    /// <summary>
    /// 返回由第一个集合和所有后续集合之间的差异产生的集合成员。
    /// 类似 except
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Dictionary<object, long>> ZDiffWithScoreAsync(string[] key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result = client.ExecuteMoreResultAsync<object>(new Command(RedisCommandName.ZDiff, new object[]
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
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<int>(new Command(RedisCommandName.ZDiffStore,
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
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<int>(new Command(RedisCommandName.ZIncrBy, new object[]
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
    public async Task<List<object>> ZInterAsync(string[] keys, long[] weights = null,
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
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<List<object>>(new Command(RedisCommandName.ZInter, args.ToArray()));
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
    public async Task<Dictionary<object, long>> ZInterWithScoreAsync(string[] keys, long[] weights = null,
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
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteMoreResultAsync<object>(new Command(RedisCommandName.ZInter, args.ToArray()))
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
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<int>(new Command(RedisCommandName.ZUnionStore, args.ToArray()));
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
    public async Task<List<object>> ZUnionAsync(string[] keys, long[] weights = null,
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
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<List<object>>(new Command(RedisCommandName.ZUnion, args.ToArray()));
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
    public async Task<Dictionary<object, long>> ZUnionWithScoreAsync(string[] keys, long[] weights = null,
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
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteMoreResultAsync<object>(new Command(RedisCommandName.ZUnion, args.ToArray()))
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
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync<long>(new Command(RedisCommandName.ZScore, new object[]
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
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<long>(new Command(RedisCommandName.ZInterCard,
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
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<int>(new Command(RedisCommandName.ZInterStore,
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
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync<long>(new Command(RedisCommandName.ZLexCount, new object[]
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
    public async Task<(string key, Dictionary<object, long> data)> ZMpopAsync(string[] keys,
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
        await using var client = await GetRedisClient(cancellationToken);
        var result = await client.ExecuteAsync<object>(new Command(RedisCommandName.ZMpop, args.ToArray()));
        //判断是否有数据
        if (result is not List<object> {Count: >= 2} resultList || resultList[1] is not List<object> valueList)
            return default;

        var dic = new Dictionary<object, long>();
        foreach (var item in valueList)
        {
            if (item is List<object> {Count: >= 2} datas)
            {
                dic.Add(datas[0], datas[1].ToString().ToLong());
            }
        }

        return (resultList[0].ToString(), dic);
    }
}