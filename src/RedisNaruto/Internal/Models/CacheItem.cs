namespace RedisNaruto.Internal.Models;

internal class CacheItem
{
    /// <summary>
    /// 缓存值
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// 缓存过期时间
    /// </summary>
    public DateTime ExpireTime { get; set; }

    /// <summary>
    /// 最后一次访问时间
    /// </summary>
    public DateTime? LastAccessed { get; set; }
}