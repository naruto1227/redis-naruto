using System.Collections.Concurrent;

namespace RedisNaruto.Internal;

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

/// <summary>
/// 客户端缓存字典
/// </summary>
internal static class ClientSideCachingDic
{
    /// <summary>
    /// 缓存字典
    /// </summary>
    private static ConcurrentDictionary<string, CacheItem> _entries = new ConcurrentDictionary<string, CacheItem>();

    //todo  增加清理操作
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static object Get(string key)
    {
        if (_entries.TryGetValue(key, out var res))
        {
            if (res.ExpireTime < DateTime.Now)
            {
                _entries.TryRemove(key, out _);
                return default;
            }

            res.LastAccessed = DateTime.Now;
            return res.Value;
        }

        return default;
    }

    /// <summary>
    /// 存储
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void Set(string key, object value, TimeSpan expireTime)
    {
        if (_entries.TryGetValue(key, out var old))
        {
            _entries.TryUpdate(key, new CacheItem
            {
                Value = value,
                ExpireTime = DateTime.Now.AddSeconds(expireTime.TotalSeconds),
                LastAccessed = null
            }, old);
        }
        else
        {
            _entries.TryAdd(key, new CacheItem
            {
                Value = value,
                ExpireTime = DateTime.Now.AddSeconds(expireTime.TotalSeconds),
                LastAccessed = null
            });
        }
    }

    /// <summary>
    /// 移除缓存
    /// </summary>
    /// <param name="key"></param>
    public static void Remove(string key)
    {
        _entries.TryRemove(key, out _);
    }
}