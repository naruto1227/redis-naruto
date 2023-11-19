namespace RedisNaruto.EventDatas;

public class LockCreateFailEventData:EventData
{
    public LockCreateFailEventData(string resourceName, string lockId) : base(nameof(LockCreateFailEventData))
    {
        ResourceName = resourceName;
        LockId = lockId;
    }
    /// <summary>
    /// 资源名称
    /// </summary>
    public string ResourceName { get; }

    /// <summary>
    /// 锁的唯值
    /// </summary>
    public string LockId { get; }
}