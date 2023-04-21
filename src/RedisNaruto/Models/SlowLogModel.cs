namespace RedisNaruto.Models;

public struct SlowLogModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="createTime"></param>
    /// <param name="execTime"></param>
    /// <param name="commandDatas"></param>
    /// <param name="clientIpPort"></param>
    /// <param name="clientName"></param>
    internal SlowLogModel(int id, long createTime, long execTime, List<RedisValue> commandDatas, string clientIpPort,
        string clientName)
    {
        Id = id;
        CreateTime = createTime;
        ExecTime = execTime;
        CommandDatas = commandDatas;
        ClientIpPort = clientIpPort;
        ClientName = clientName;
    }

    /// <summary>
    /// 唯一id
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// 开始处理命令的时间戳 秒级
    /// </summary>
    public long CreateTime { get; init; }

    /// <summary>
    /// 执行时间 微秒
    /// </summary>
    public long ExecTime { get; init; }

    /// <summary>
    /// 执行命令参数的数组
    /// </summary>
    public List<RedisValue> CommandDatas { get; init; }

    /// <summary>
    /// 客户端ip和端口
    /// </summary>
    public string ClientIpPort { get; init; }

    /// <summary>
    /// 客户端名称
    /// </summary>
    public string ClientName { get; init; }
}