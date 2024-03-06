namespace RedisNaruto.Internal.Models;

/// <summary>
/// 缓存的值
/// </summary>
public class CachingValue
{

    
    public CachingValue(object value)
    {
        this.Value = value;
        CreateTime=DateTime.Now;
    }

    /// <summary>
    /// 值
    /// </summary>
    public object Value { get; init; }
    public DateTime CreateTime { get;}
}