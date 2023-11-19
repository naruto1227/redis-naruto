namespace RedisNaruto.EventDatas;

public class LockCreateSuccessEventData : EventData
{
    public LockCreateSuccessEventData(string resourceName, string lockId) : base(nameof(LockCreateSuccessEventData))
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