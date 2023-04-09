using RedisNaruto.Consts;
using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// 基础命令
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 拷贝key
    /// </summary>
    /// <param name="source">原始key</param>
    /// <param name="dest">目标key</param>
    /// <param name="db">需要转移的目标的db</param>
    /// <param name="isReplace">如果true 代表如果目标key存在就先删除key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> CopyAsync(string source, string dest, int? db = null, bool isReplace = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(dest);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        var argv = new List<object>()
        {
            source,
            dest
        };
        if (db != null)
        {
            argv.Add(StreamConst.Db);
            argv.Add(db.Value);
        }

        argv.IfAdd(isReplace, StreamConst.Replace);

        return await client.ExecuteAsync(new Command(RedisCommandName.Copy, argv.ToArray())) == 1;
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="keys">需要删除的key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> DelAsync(string[] keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.Del, keys));
    }

    /// <summary>
    ///  以 Redis 特定格式序列化存储在 key 中的值，并将其返回给用户。可以使用命令将返回值合成回 Redis 键RESTORE 。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<RedisValue> DumpAsync(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.Dump, new[] {key}));
    }

    /// <summary>
    ///  将dump序列化的值，反序列化到指定的key中
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expire">过期时间</param>
    /// <param name="isReplace">存储的话 先删除后添加</param>
    /// <param name="cancellationToken"></param>
    /// <param name="serializedValue">序列化值</param>
    /// <returns></returns>
    public async Task<bool> ReStoreAsync(string key, object serializedValue, TimeSpan? expire = null,
        bool isReplace = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(serializedValue);
        cancellationToken.ThrowIfCancellationRequested();
        var argv = new List<object>()
        {
            key,
            expire?.TotalMilliseconds ?? 0,
            serializedValue
        };
        argv.IfAdd(isReplace, StreamConst.Replace);
        
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.ReStore, argv.ToArray()))=="OK";
    }
    
    /// <summary>
    /// 校验是否存在
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ExistsAsync(string[] keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync(new Command(RedisCommandName.Exists, keys));
    }
}