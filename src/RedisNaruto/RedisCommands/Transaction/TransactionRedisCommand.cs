using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Internal.RedisResolvers;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.RedisCommands.Transaction;

/// <summary>
/// 事务命令
/// </summary>
public class TransactionRedisCommand : RedisCommand, ITransactionRedisCommand
{
    private readonly TranRedisResolver _tranRedisResolver;

    internal TransactionRedisCommand(IRedisClientPool redisClientPool, TranRedisResolver tranRedisResolver) : base(
        tranRedisResolver,
        redisClientPool)
    {
        _tranRedisResolver = tranRedisResolver;
    }

    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<object>> ExecAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return (await RedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.Exec, default)).ToListAsync());
    }

    /// <summary>
    /// 取消事务
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task DiscardAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _ = await RedisResolver.InvokeAsync<RedisValue>(new Command(RedisCommandName.DisCard, default));
    }

    protected override async ValueTask DisposeCoreAsync(bool isDispose)
    {
        await _tranRedisResolver.DisposeAsync();
    }
}