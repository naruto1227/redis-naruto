namespace RedisNaruto.EventDatas;

/// <summary>
/// 接收哨兵消息
/// </summary>
public class ReceiveSentinelMessageEventData : EventData
{
    public ReceiveSentinelMessageEventData(string host, int port, string cmd, object[] argv, object result) : base(nameof(ReceiveSentinelMessageEventData))
    {
        Host = host;
        Port = port;
        Cmd = cmd;
        Argv = argv;
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
    /// 命令
    /// </summary>
    public string Cmd { get; init; }

    /// <summary>
    /// 参数
    /// </summary>
    public object[] Argv { get; init; }

    /// <summary>
    /// 结果
    /// </summary>
    public object Result { get; init; }
}