using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;
using RedisNaruto.Internal.RedisResolvers;
using RedisNaruto.Models;
using RedisNaruto.RedisCommands.Transaction;

namespace RedisNaruto.RedisCommands;

public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 开启事务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ITransactionRedisCommand> MultiAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var tranRedisResolver = new TranRedisResolver(_redisClientPool);
        await tranRedisResolver.InitClientAsync();
        await tranRedisResolver.BeginTranAsync();
        return new TransactionRedisCommand(_redisClientPool, tranRedisResolver);
    }

    public async Task UnWatchAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _ = await RedisResolver.InvokeAsync<RedisValue>(new Command(RedisCommandName.UnWatch, default));
    }

    public async Task WatchAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _ = await RedisResolver.InvokeAsync<RedisValue>(new Command(RedisCommandName.Watch, keys));
    }
}