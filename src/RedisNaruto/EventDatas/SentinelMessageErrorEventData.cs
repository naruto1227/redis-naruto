namespace RedisNaruto.EventDatas;

/// <summary>
/// 哨兵错误
/// </summary>
public class SentinelMessageErrorEventData : EventData
{
    public SentinelMessageErrorEventData(string host, int port, Exception exception) : base(nameof(SentinelMessageErrorEventData))
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