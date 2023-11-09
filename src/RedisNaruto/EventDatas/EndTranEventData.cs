namespace RedisNaruto.EventDatas;

/// <summary>
/// 结束事务消息
/// </summary>
public class EndTranEventData: EventData
{
    public EndTranEventData(string host, int port, object result) : base(nameof(EndTranEventData))
    {
        Host = host;
        Port = port;
        Result = result;
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
    /// 结果
    /// </summary>
    public object Result { get; init; }
}