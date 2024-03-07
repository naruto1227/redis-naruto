namespace RedisNaruto.Internal.Models;

/// <summary>
/// 缓存的值
/// </summary>
public class CachingValue
{
    public CachingValue(object value)
    {
        this.Value = value;
        CreateTime = DateTime.Now;
    }

    /// <summary>
    /// 值
    /// </summary>
    public object Value { get; init; }

    public DateTime CreateTime { get; }

    /// <summary>
    /// 使用次数 每次获取完毕后更新此字段信息 可用于 后续的缓存淘汰策略
    /// </summary>
    public int UseCount => _useCount;

    /// <summary>
    /// 
    /// </summary>
    private int _useCount;

    /// <summary>
    /// 递增 使用次数
    /// </summary>
    public void IncrementUseCount()
    {
        Interlocked.Increment(ref _useCount);
    }
}