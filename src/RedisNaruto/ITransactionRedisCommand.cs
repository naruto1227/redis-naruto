namespace RedisNaruto;

public interface ITransactionRedisCommand : IRedisCommand
{
    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<object>> ExecAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消事务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DiscardAsync(CancellationToken cancellationToken = default);
}