using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;

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
}