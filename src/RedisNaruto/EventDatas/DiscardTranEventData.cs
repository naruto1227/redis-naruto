namespace RedisNaruto.EventDatas;

/// <summary>
/// 取消事务
/// </summary>
public class DiscardTranEventData: EventData
{
    public DiscardTranEventData(string host, int port) : base(nameof(DiscardTranEventData))
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