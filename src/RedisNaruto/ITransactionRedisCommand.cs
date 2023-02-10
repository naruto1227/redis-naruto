namespace RedisNaruto;

public interface ITransactionRedisCommand : IRedisCommand
{
    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<object>> ExecAsync(CancellationToken cancellationToken = default);
}