namespace RedisNaruto.EventDatas;

public class EndPipeEventData: EventData
{
    public EndPipeEventData(string host, int port, object result) : base(nameof(EndPipeEventData))
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