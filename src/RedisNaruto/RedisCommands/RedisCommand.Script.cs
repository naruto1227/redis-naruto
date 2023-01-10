using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;

namespace RedisNaruto.RedisCommands;

public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 执行script脚本
    /// </summary>
    /// <param name="script"></param>
    /// <param name="keys"></param>
    /// <param name="argvs"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<object> EvalAsync(string script, object[] keys, object[] argvs,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        keys ??= Array.Empty<object>();
        argvs ??= Array.Empty<object>();
        var param = new object[2]
        {
            script,
            keys.Length
        }.Union(keys).Union(argvs).ToArray();
        await using var client = await _redisClientPool.RentAsync(cancellationToken);
        return await client.ExecuteAsync<object>(new Command(RedisCommandName.Eval, param));
    }

    /// <summary>
    /// 执行指定的 sha值的 lua缓存脚本 调用 SCRIPT LOAD 脚本返回的 sha值
    /// </summary>
    /// <param name="sha"></param>
    /// <param name="keys"></param>
    /// <param name="argvs"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<object> EvalShaAsync(string sha,object[] keys, object[] argvs,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        keys ??= Array.Empty<object>();
        argvs ??= Array.Empty<object>();
        var param = new object[2]
        {
            sha,
            keys.Length
        }.Union(keys).Union(argvs).ToArray();
        await using var client = await _redisClientPool.RentAsync(cancellationToken);
        return await client.ExecuteAsync<object>(new Command(RedisCommandName.EvalSha, param));
    }

    /// <summary>
    /// 将script 存储到redis lua缓存中
    /// </summary>
    /// <param name="script"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string> ScriptLoadAsync(string script,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await _redisClientPool.RentAsync(cancellationToken);
        return await client.ExecuteAsync<string>(new Command(RedisCommandName.Script, new object[]
        {
            "LOAD",
            script
        }));
    }
}