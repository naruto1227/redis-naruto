namespace RedisNaruto.Models;

public class ClientSideCachingOption
{
    /// <summary>
    /// key的匹配方式
    /// </summary>
    public Func<string, bool> KeyPrefixMatch { get; set; }

    /// <summary>
    /// 缓存的过期时间
    /// </summary>
    public TimeSpan TimeOut { get; set; }
}