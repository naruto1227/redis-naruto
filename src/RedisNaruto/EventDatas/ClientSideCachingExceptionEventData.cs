namespace RedisNaruto.EventDatas;

public class ClientSideCachingExceptionEventData: EventData
{
    public ClientSideCachingExceptionEventData(Exception exception) : base(nameof(ClientSideCachingExceptionEventData))
    {
        Exception = exception;
    }

    /// <summary>
    /// 异常信息
    /// </summary>
    public Exception Exception { get; }
}