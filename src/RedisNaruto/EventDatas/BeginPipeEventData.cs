namespace RedisNaruto.EventDatas;

public class BeginPipeEventData : EventData
{
    public BeginPipeEventData(string host, int port) : base(nameof(BeginPipeEventData))
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