using System.Runtime.CompilerServices;
using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// set 数据类型
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> SAddAsync(string key, object[] value,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.SAdd, new object[]
            {
                key
            }.Concat(value).ToArray()));
        return result == 1;
    }

    /// <summary>
    /// 返回set中集合的长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> SCardAsync(string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.SCard, new[]
            {
                key,
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
    public async Task<List<RedisValue>> SDiffAsync(string[] key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.SDiff, key)).ToRedisValueListAsync();
        return result;
    }

    /// <summary>
    /// 此命令等于SDIFF，但不是返回结果集，而是存储在destination.
    /// 将except的差异值存到目标destination
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> SDiffStoreAsync(string destination, string[] keys,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.SDiffStore,
                new object[] {destination}.Concat(keys).ToArray()));
        return result;
    }

    /// <summary>
    /// 返回由所有给定集的交集产生的集的成员。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<RedisValue>> SInterAsync(string[] key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.SInter, key))
                .ToRedisValueListAsync();
        return result;
    }

    /// <summary>
    /// 返回交集的值数量 类似与 SInter，SInter返回具体的交集数据，SInterCard返回数
    /// </summary>
    /// <param name="key"></param>
    /// <param name="limit">默认情况下，该命令计算所有给定集的交集的基数。当提供可选LIMIT参数（默认为 0，表示无限制）时，如果交集基数在计算中途达到 limit，则算法将退出并产生 limit 作为基数。这样的实现确保了限制低于实际交集基数的查询的显着加速。</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> SInterCardAsync(string[] key, int limit = 0,
        CancellationToken cancellationToken = default)
    {
        if (key is not {Length: > 0})
        {
            return default;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.SInterCard,
                new object[]
                {
                    key.Length
                }.Concat(
                    key).Concat(new[] {"LIMIT", limit.ToString()}).ToArray()));
        return result;
    }

    /// <summary>
    /// 此命令等于SInter，但不是返回结果集，而是存储在destination.
    /// 将except的差异值存到目标destination
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> SInterStoreAsync(string destination, string[] keys,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.SInterStore,
                new object[] {destination}.Union(keys).ToArray()));
        return result;
    }

    /// <summary>
    /// 判断值是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> SisMemberAsync(string key, object member,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.SisMember,
                new object[] {key, member}));
        return result == 1;
    }

    /// <summary>
    /// 获取set的数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<RedisValue>> SMembersAsync(string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result =
            await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.SMembers,
                new object[] {key})).ToRedisValueListAsync();
        return result;
    }

    /// <summary>
    /// 判断值是否存在set中
    /// </summary>
    /// <param name="key"></param>
    /// <param name="members"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<RedisValue>> SmisMemberAsync(string key, object[] members,
        CancellationToken cancellationToken = default)
    {
        if (members is not {Length: > 0})
        {
            return default;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var result = await
            RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.SmisMember,
                new object[] {key}.Concat(members).ToArray())).ToRedisValueListAsync();
        return result;
    }

    /// <summary>
    /// 将值从 source 移动到 destination
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="member"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> SMoveAsync(string source, string destination, object member,
        CancellationToken cancellationToken = default)
    {
        if (member is null)
        {
            return default;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var result = await
            RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.SMove, new[]
            {
                source, destination, member
            }));
        return result == 1;
    }


    /// <summary>
    /// 随机移除 指定数量的值 原子性操作
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<RedisValue>> SPopAsync(string key, int count = 1,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await
            RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.SPop, new object[]
            {
                key, count
            })).ToRedisValueListAsync();
        return result;
    }

    /// <summary>
    /// 返回随机对象
    /// 如果提供的参数为正，则返回不同元素count的数组。数组的长度是集合的基数 ( ) 之一，以较小者为准。
    /// 如果用否调整使用count，行为会改变，命可以多次返回相同的元素。在这种情况下，返回的元数是指定的绝对值count。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<RedisValue>> SRandMemberAsync(string key, int count = 1,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await
            RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.SRandMember, new object[]
            {
                key, count
            })).ToRedisValueListAsync();
        return result;
    }

    /// <summary>
    /// 移除成员信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="members">需要删除的成员</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> SRemAsync(string key, object[] members,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await
            RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.SRem, new object[]
            {
                key,
            }.Concat(members).ToArray()));
        return result;
    }

    /// <summary>
    ///返回多个set的并集
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<RedisValue>> SUnionAsync(string[] keys,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await
            RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.SUnion, keys)).ToRedisValueListAsync();
        return result;
    }

    ///  <summary>
    /// 将多个key的并集 存储到一个新的目标set中
    ///  </summary>
    ///  <param name="destination"></param>
    ///  <param name="keys"></param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    public async Task<int> SUnionStoreAsync(string destination, string[] keys,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await
            RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.SUnionStore,
                new object[]
                {
                    destination
                }.Concat(keys).ToArray()));
        return result;
    }

    /// <summary>
    /// 扫描set的数据
    /// https://redis.io/commands/scan/
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">条数</param>
    /// <param name="cancellationToken"></param>
    /// <param name="matchPattern">匹配条件</param>
    /// <returns></returns>
    public async IAsyncEnumerable<List<RedisValue>> SScanAsync(string key,
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
            var resultList = await RedisResolver.InvokeAsync<object>(new Command(RedisCommandName.SScan,
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
                yield return datas.ToRedisValueList();
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
}