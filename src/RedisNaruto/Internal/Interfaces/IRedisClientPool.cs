using System.Diagnostics.CodeAnalysis;

namespace RedisNaruto.Internal.Interfaces;

/// <summary>
/// redis池化
/// </summary>
internal interface IRedisClientPool : IDisposable
{
    /// <summary>
    /// 租用
    /// </summary>
    /// <returns></returns>
    Task<IRedisClient> RentAsync(CancellationToken cancellationToken = default);

    // /// <summary>
    // /// 归还
    // /// </summary>
    // /// <returns></returns>
    // ValueTask ReturnAsync([NotNull] IRedisClient redisClient);
}