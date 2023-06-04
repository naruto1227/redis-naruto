using RedisNaruto.Utils;

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
        //默认5分钟
        Idle = 1000 * 60 * 5;
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
    /// 最大
    /// </summary>
    private int _maxPoolCount;

    /// <summary>
    /// 最大连接池
    /// </summary>
    public int MaxPoolCount
    {
        get
        {
            if (_maxPoolCount == 0) return PoolCount * 2;
            return _maxPoolCount;
        }
        set => _maxPoolCount = value;
    }


    /// <summary>
    /// 超时时间 ms 默认3000
    /// </summary>
    public int TimeOut { get; set; }

    /// <summary>
    /// 0 不开启
    /// 闲置时间 闲置多久进行释放 ，当池中的数量低于 最小不做处理
    /// </summary>
    public int Idle { get; set; }

    /// <summary>
    /// 拆分的分隔符
    /// </summary>
    private const char SplitChar = ',';

    /// <summary>
    /// 相等
    /// </summary>
    private const char EqualChar = '=';

    /// <summary>
    /// 配置
    /// </summary>
    /// <param name="config"></param>
    internal ConnectionBuilder Parse(string config)
    {
        var configs = config.Split(SplitChar);
        if (configs is not {Length: > 0})
        {
            throw new ArgumentNullException(nameof(config));
        }

        //连接地址
        var connection = new HashSet<string>();
        //遍历
        foreach (var itemEle in configs.Select(a => a.Trim()))
        {
            //判断是否是 主机地址
            if (itemEle.IndexOf(EqualChar) > 0)
            {
                var ele = itemEle.Split(EqualChar);
                switch (ele[0])
                {
                    case "username":
                        this.UserName = ele[1];
                        break;
                    case "password":
                        this.Password = ele[1];
                        break;
                    case "servertype":
                        this.ServerType = (ServerType) ele[1].ToInt();
                        break;
                    case "mastername":
                        this.MasterName = ele[1];
                        break;
                    case "database":
                        this.DataBase = ele[1].ToInt();
                        break;
                    case "poolcount":
                        this.PoolCount = ele[1].ToInt();
                        break;
                    case "maxpoolcount":
                        this.MaxPoolCount = ele[1].ToInt();
                        break;
                    case "timeout":
                        this.TimeOut = ele[1].ToInt();
                        break;
                    case "idle":
                        this.Idle = ele[1].ToInt();
                        break;
                }
            }
            else
            {
                connection.Add(itemEle);
            }
        }

        this.Connection = connection.ToArray();
        return this;
    }
}