namespace RedisNaruto.EventDatas;

/// <summary>
/// 缓存写入前操作
/// </summary>
public sealed class WriteRedisNarutoMessageBeforeEventData : EventData
{
    public WriteRedisNarutoMessageBeforeEventData(string cmd,object[] args):base (nameof(WriteRedisNarutoMessageBeforeEventData))
    {
        this.Cmd = cmd;
        this.Args = args;
    }
    
    /// <summary>
    /// 命令
    /// </summary>
    public string Cmd { get;  init; }

    /// <summary>
    /// 参数
    /// </summary>
    public object[] Args { get; init; }
}