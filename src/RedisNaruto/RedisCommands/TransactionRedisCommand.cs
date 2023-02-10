using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// 事务命令
/// </summary>
public class TransactionRedisCommand : RedisCommand, ITransactionRedisCommand
{
    internal TransactionRedisCommand(IRedisClientPool redisClientPool)
    {
        _redisClientPool = redisClientPool;
    }

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<object>> ExecAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync<List<object>>(new Command(RedisCommandName.Exec, default));
    }

    protected override async ValueTask DisposeCoreAsync(bool isDispose)
    {
        await base.DisposeCoreAsync(isDispose);
        await _redisClientPool.ReturnAsync(_redisClient);
    }
}