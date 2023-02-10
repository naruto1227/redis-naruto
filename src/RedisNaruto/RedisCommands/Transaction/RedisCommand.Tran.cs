using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;
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
        var client = await GetRedisClient(cancellationToken);
        await client.ExecuteAsync<string>(new Command(RedisCommandName.Multi, default));
        var transactionRedisCommand = new TransactionRedisCommand(_redisClientPool);
        transactionRedisCommand.ChangeRedisClient(client);
        return transactionRedisCommand;
    }
    public async Task UnWatchAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        _ = await client.ExecuteAsync<string>(new Command(RedisCommandName.UnWatch, default));
    }
    public async Task WatchAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        _ = await client.ExecuteAsync<string>(new Command(RedisCommandName.Watch, keys));
    }
}