namespace RedisNaruto;

/// <summary>
/// redis服务的类型
/// </summary>
public enum ServerType
{
    /// <summary>
    /// 单机版
    /// </summary>
    Standalone,

    /// <summary>
    /// 哨兵模式
    /// </summary>
    Sentinel,

    /// <summary>
    /// 集群模式
    /// </summary>
    Cluster
}