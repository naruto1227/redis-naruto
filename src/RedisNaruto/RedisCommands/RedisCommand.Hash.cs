using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// hash操作
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// hash删除
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> HDelAsync(string key, string[] fields, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return
            await client.ExecuteAsync<long>(new Command(RedisCommandName.HDel,
                new object[] {key}.Union(fields).ToArray()));
    }

    /// <summary>
    /// hash存储
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> HSetAsync(string key, Dictionary<string, object> fields,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new object[fields.Count * 2 + 1];
        argv[0] = key;

        var idx = 1;
        foreach (var (itemKey, value) in fields)
        {
            argv[idx * 2 - 1] = itemKey;
            argv[idx * 2] = value;
            idx++;
        }

        return
            await client.ExecuteAsync<long>(new Command(RedisCommandName.HSet,
                argv));
    }

    /// <summary>
    /// hash存在
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> HExistsAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return
            await client.ExecuteAsync<int>(new Command(RedisCommandName.HExists,
                new object[]
                {
                    key, field
                })) == 1;
    }

    /// <summary>
    /// hash获取
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> HGetAsync(string key, string field, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return
            await client.ExecuteAsync<string>(new Command(RedisCommandName.HGet,
                new object[]
                {
                    key, field
                }));
    }
}