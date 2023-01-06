using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;

namespace RedisNaruto;

/// <summary>
/// 字符串操作
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    public async Task<bool> StringSet(string key, object value, TimeSpan timeSpan = default)
    {
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

    public async Task<string> StringGet(string key)
    {
        return await StringGet<string>(key);
    }

    /// <summary>
    /// 查询字符串
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<TResult> StringGet<TResult>(string key)
    {
        await using var client = await _redisClientPool.RentAsync();
        return await client.ExecuteAsync<TResult>(new Command(RedisCommandName.Get,
            new object[] {key}));
    }

    public async Task<List<TResult>> StringMGet<TResult>(string[] key)
    {
        await using var client = await _redisClientPool.RentAsync();
        return await client.ExecuteMoreResultAsync<TResult>(
            new Command(RedisCommandName.MGET, key));
    }
}