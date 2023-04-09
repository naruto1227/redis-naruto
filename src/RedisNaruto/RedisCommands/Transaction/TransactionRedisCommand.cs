using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.RedisCommands.Transaction;

/// <summary>
/// 事务命令
/// </summary>
public class TransactionRedisCommand : RedisCommand, ITransactionRedisCommand
{
    internal TransactionRedisCommand(IRedisClientPool redisClientPool)
    {
        RedisClientPool = redisClientPool;
    }

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<object>> ExecAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var client = await GetRedisClient(cancellationToken);
        return (await client.ExecuteMoreResultAsync(new Command(RedisCommandName.Exec, default)).ToListAsync());
    }

    /// <summary>
    /// 取消事务
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task DiscardAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var client = await GetRedisClient(cancellationToken);
        _ = await client.ExecuteAsync(new Command(RedisCommandName.DisCard, default));
    }
}