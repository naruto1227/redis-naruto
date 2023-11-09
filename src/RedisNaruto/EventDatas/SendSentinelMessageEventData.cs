namespace RedisNaruto.EventDatas;

/// <summary>
/// 发送哨兵消息
/// </summary>
public class SendSentinelMessageEventData : EventData
{
    public SendSentinelMessageEventData(string host, int port, string cmd, object[] argv) : base(nameof(SendSentinelMessageEventData))
    {
        Host = host;
        Port = port;
        Cmd = cmd;
        Argv = argv;
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
    /// 命令
    /// </summary>
    public string Cmd { get; init; }

    /// <summary>
    /// 参数
    /// </summary>
    public object[] Argv { get; init; }
}