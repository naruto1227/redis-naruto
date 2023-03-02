using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;

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
    public async Task<bool> SAddAsync(string key, object value,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<int>(new Command(RedisCommandName.SAdd, new[]
            {
                key,
                value
            }));
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
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<long>(new Command(RedisCommandName.SCard, new[]
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
    public async Task<List<object>> SDiffAsync(string[] key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<List<object>>(new Command(RedisCommandName.SDiff, key));
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
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<int>(new Command(RedisCommandName.SDiffStore,
                new object[] {destination}.Union(keys).ToArray()));
        return result;
    }
    /// <summary>
    /// 返回由所有给定集的交集产生的集的成员。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<object>> SInterAsync(string[] key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var result =
            await client.ExecuteAsync<List<object>>(new Command(RedisCommandName.SInter, key));
        return result;
    }
}