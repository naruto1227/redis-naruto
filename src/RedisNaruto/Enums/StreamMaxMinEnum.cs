namespace RedisNaruto.Enums;

public enum StreamMaxMinEnum
{
    /// <summary>
    /// 默认
    /// </summary>
    Default,

    /// <summary>
    /// 流最大长度限制 精确修建
    /// </summary>
    MaxPrecision,

    /// <summary>
    /// 流最大长度限制 几乎精确修建
    /// </summary>
    MaxNearly,

    /// <summary>
    /// 最小的id限制 精确修建
    /// 6.2.0
    /// </summary>
    MinIdPrecision,

    /// <summary>
    /// 最小的id限制 几乎精确修建
    /// 6.2.0
    /// </summary>
    MinIdNearly,
}