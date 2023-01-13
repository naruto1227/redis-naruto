using RedisNaruto.Internal.Models;

namespace RedisNaruto.Internal.Sentinels;

/// <summary>
/// 哨兵连接
/// </summary>
internal interface ISentinelConnection
{
    /// <summary>
    /// 获取master节点信息
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<HostPort> GetMaserHostPort(
        CancellationToken cancellationToken = default);
}