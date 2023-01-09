using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;
using RedisNaruto.Utils;

namespace RedisNaruto;

/// <summary>
/// 字符串操作
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="timeSpan"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> StringSet(string key, object value, TimeSpan timeSpan = default,CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var argv = timeSpan == default
            ? new[]
            {
                key,
                value
            }
            : new[]
            {
                key,
                value,
                "PX",
                timeSpan == default ? 0 : timeSpan.TotalMilliseconds
            };
        await using var client = await _redisClientPool.RentAsync();

        var result =
            await client.ExecuteAsync<string>(new Command(RedisCommandName.Set, argv));
        return result == "OK";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> StringGet(string key,CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await StringGet<string>(key,cancellationToken);
    }

    /// <summary>
    /// 查询字符串
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResult> StringGet<TResult>(string key,CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await _redisClientPool.RentAsync();
        return await client.ExecuteAsync<TResult>(new Command(RedisCommandName.Get,
            new object[] {key}));
    }

    /// <summary>
    /// 批量获取
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public async Task<List<TResult>> StringMGet<TResult>(string[] key,CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await _redisClientPool.RentAsync();
        return await (client.ExecuteMoreResultAsync<TResult>(
            new Command(RedisCommandName.MGET, key))).ToListAsync();
    }
}