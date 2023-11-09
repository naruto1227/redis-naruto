namespace RedisNaruto.EventDatas;

/// <summary>
/// 开启事务消息
/// </summary>
public class BeginTranEventData: EventData
{
    public BeginTranEventData(string host, int port) : base(nameof(BeginTranEventData))
    {
        Host = host;
        Port = port;
    }
    /// <summary>
    /// 主机
    /// </summary>
    public string Host { get; init; }

    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; init; }
}