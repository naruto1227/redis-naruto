using System.Diagnostics;

namespace RedisNaruto.EventDatas;

public abstract class EventData
{
    protected EventData(string eventName)
    {
        Id = Guid.NewGuid();
        Time = Stopwatch.GetTimestamp();
        EventName = eventName;
    }

    /// <summary>
    /// 时间名称
    /// </summary>
    public string EventName { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public long Time { get; private set; }
    
}