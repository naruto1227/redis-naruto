using System.Runtime.Intrinsics.Arm;
using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;
using RedisNaruto.Internal.RedisResolvers;
using RedisNaruto.Models;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// 客户端命令
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 返回客户端id
    /// 1.它从不重复，所以如果CLIENT ID返回相同的数字，调用者可以确定底层客户端没有断开并重新连接连接，但它仍然是同一个连接。
    ///2.ID 是单调递增的。如果一个连接的 ID 大于另一个连接的 ID，则可以保证在稍后的时间与服务器建立第二个连接。
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> ClientIdAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.Client, new[] {"ID"}));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<bool> PingAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.Ping, null)) == "PONG";
    }

    /// <summary>
    /// 切换db
    /// </summary>
    /// <param name="db"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ISelectDbRedisCommand> SelectDbAsync(int db, CancellationToken cancellationToken = default)
    {
        if (db < 0)
        {
            throw new ArgumentException(nameof(db));
        }

        cancellationToken.ThrowIfCancellationRequested();
        var selectDbRedisResolver = new SelectDbRedisResolver(_redisClientPool);
        await selectDbRedisResolver.InitClientAsync(db);
        return new SelectDbRedisCommand(selectDbRedisResolver, _redisClientPool);
    }
}