namespace RedisNaruto.Internal.Interfaces;

/// <summary>
/// redis 客户端工厂
/// </summary>
internal interface IRedisClientFactory
{
    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="disposeTask"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IRedisClient> GetAsync(
        Action<IRedisClient> disposeTask, CancellationToken cancellationToken = default);
    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="disposeTask"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IRedisClient> GetClientSideCacheAsync(
        Action<IRedisClient> disposeTask, CancellationToken cancellationToken = default);
}