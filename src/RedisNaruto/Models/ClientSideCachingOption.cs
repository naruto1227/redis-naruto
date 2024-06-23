using RedisNaruto.Enums;

namespace RedisNaruto.Models;

public class ClientSideCachingOption
{
    public ClientSideCachingOption()
    {
        TimeOut = TimeSpan.FromMinutes(5);
    }

    /// <summary>
    /// 客户端缓存的模式
    /// </summary>
    public ClientSideCachingModeEnum Mode { get; set; }

    /// <summary>
    /// key的匹配方式
    /// </summary>
    public string[] KeyPrefix { get; set; }

    /// <summary>
    /// 缓存的过期时间
    /// </summary>
    public TimeSpan TimeOut { get; set; }

    /// <summary>
    /// 缓存中的容量限制，缓存的集合超过此大小将按照时间剔除老的数据
    /// </summary>
    public int Capacity { get; set; }
}