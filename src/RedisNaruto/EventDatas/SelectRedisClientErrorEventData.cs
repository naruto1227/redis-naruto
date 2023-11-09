namespace RedisNaruto.EventDatas;

/// <summary>
/// 选择redis客户端错误
/// </summary>
public class SelectRedisClientErrorEventData : EventData
{
    public SelectRedisClientErrorEventData(string host, int port, Exception exception,string eventName=nameof(SelectRedisClientErrorEventData)) : base(eventName)
    {
        Host = host;
        Port = port;
        Exception = exception;
    }

    /// <summary>
    /// 主机
    /// </summary>
    public string Host { get; init; }

    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; init; }
    
    /// <summary>
    /// 异常
    /// </summary>
    public Exception Exception { get; init; }
}