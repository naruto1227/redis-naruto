namespace RedisNaruto.EventDatas;

/// <summary>
/// 缓存写入后
/// </summary>
public class WriteRedisNarutoMessageAfterEventData:EventData
{
    public WriteRedisNarutoMessageAfterEventData(string cmd,object[] args, object result):base (nameof(WriteRedisNarutoMessageAfterEventData))
    {
        this.Cmd = cmd;
        this.Args = args;
        Result = result;
    }  
    
    /// <summary>
    /// 命令
    /// </summary>
    public string Cmd { get;  init; }

    /// <summary>
    /// 参数
    /// </summary>
    public object[] Args { get; init; }

    /// <summary>
    /// 结果
    /// </summary>
    public object Result { get; init; }
}