namespace RedisNaruto;

/// <summary>
/// 连接模型
/// </summary>
public sealed class ConnectionBuilder
{
    public ConnectionBuilder()
    {
        DataBase = 0;
        PoolCount = Environment.ProcessorCount * 2;
        ServerType = ServerType.Standalone;
        TimeOut = 3000;
    }

    /// <summary>
    /// 服务类型
    /// </summary>
    public ServerType ServerType { get; set; }

    /// <summary>
    /// 连接地址
    /// </summary>
    public string[] Connection { get; set; }

    /// <summary>
    /// 主节点名称 用于哨兵
    /// </summary>
    public string MasterName { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 数据存储库
    /// </summary>
    public int DataBase { get; set; }

    /// <summary>
    /// 连接池
    /// </summary>
    public int PoolCount { get; set; }

    /// <summary>
    /// 超时时间 ms 默认3000
    /// </summary>
    public int TimeOut { get; set; }
}