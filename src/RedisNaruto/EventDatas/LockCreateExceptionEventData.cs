namespace RedisNaruto.EventDatas;

public class LockCreateExceptionEventData : EventData
{
    public LockCreateExceptionEventData(string resourceName, string lockId, Exception exception) : base(nameof(LockCreateExceptionEventData))
    {
        ResourceName = resourceName;
        LockId = lockId;
        Exception = exception;
    }


    /// <summary>
    /// 资源名称
    /// </summary>
    public string ResourceName { get; }

    /// <summary>
    /// 锁的唯值
    /// </summary>
    public string LockId { get; }

    /// <summary>
    /// 
    /// </summary>
    public Exception Exception { get; }
}