namespace RedisNaruto.Internal;

internal static class RedisCommandName
{
    #region 字符串

    /// <summary>
    /// 字符串 set
    /// </summary>
    public const string Set = "SET";

    /// <summary>
    /// 批量获取字符串
    /// </summary>
    public const string MGET = "MGET";

    /// <summary>
    /// 获取字符串
    /// </summary>
    public const string Get = "GET";

    /// <summary>
    /// 如果key已经存在并且是一个字符串，此命令将value在字符串末尾附加。如果key不存在，则创建它并将其设置为空字符串，因此与这种特殊情况APPEND 类似SET。
    /// </summary>
    public const string Append = "APPEND";

    /// <summary>
    /// 将存储在的数字减一。如果键不存在，则在0的基础上减1。如果键包含错误类型的值或包含不能表示为整数的字符串，则返回错误。此操作仅限于64 位有符号整数。
    /// </summary>
    public const string Decr = "DECR";

    /// <summary>
    /// 按照指定的数字递减
    /// key的数字递减decrement. 如果键不存在，则在0的基础上减。如果键包含错误类型的值或包含不能表示为整数的字符串，则返回错误。此操作仅限于 64 位有符号整数。
    /// </summary>
    public const string DecrBy = "DECRBY";

    /// <summary>
    ///获取键的值key并删除键。此命令类似于GET，除了它还会在成功时删除键（当且仅当键的值类型是字符串时）。
    ///  6.2.0
    /// </summary>
    public const string GetDel = "GETDEL";

    /// <summary>
    /// 获取key的值 并设置key的过期时间
    /// 6.2.0
    /// </summary>
    public const string GetEx = "GETEX";

    /// <summary>
    /// 获取字符串 指定区间的值
    /// </summary>
    public const string GetRange = "GETRANGE";

    /// <summary>
    /// 递增
    /// </summary>
    public const string IncrBy = "INCRBY";

    /// <summary>
    ///匹配两个字符串的 相似程度
    /// 7.0.0
    /// </summary>
    public const string Lcs = "LCS";

    /// <summary>
    /// 批量设置值
    /// </summary>
    public const string MSet = "MSET";

    /// <summary>
    /// 如果key不存在才添加
    /// </summary>
    public const string SetNx = "SETNX";

    /// <summary>
    /// 字符串的长度
    /// </summary>
    public const string StrLen = "STRLEN";

    /// <summary>
    /// 覆盖字符串的值 从指定的偏移量开始
    /// </summary>
    public const string SetRange = "SETRANGE";

    #endregion


    #region 发布订阅

    /// <summary>
    /// 发送消息 到频道
    /// </summary>
    public const string Pub = "PUBLISH";


    /// <summary>
    /// 订阅频道
    /// </summary>
    public const string Sub = "SUBSCRIBE";


    /// <summary>
    /// 取消订阅
    /// </summary>
    public const string UnSub = "UNSUBSCRIBE";

    #endregion

    #region client

    /// <summary>
    /// 客户端信息
    /// </summary>
    public const string Client = "CLIENT";

    /// <summary>
    /// 登陆
    /// </summary>
    public const string Auth = "AUTH";

    /// <summary>
    /// 退出
    /// </summary>
    public const string Quit = "QUIT";

    /// <summary>
    /// 选择 db
    /// </summary>
    public const string Select = "SELECT";

    /// <summary>
    /// Ping
    /// </summary>
    public const string Ping = "PING";

    #endregion

    #region server

    /// <summary>
    /// 返回选择数据库的 key数量
    /// </summary>
    public const string DbSize = "DBSIZE";

    #endregion

    #region script

    /// <summary>
    /// 执行lua脚本
    /// </summary>
    public const string Eval = "EVAL";

    /// <summary>
    /// 执行指定的 sha值的 lua缓存脚本 调用 SCRIPT LOAD 脚本返回的 sha值
    /// </summary>
    public const string EvalSha = "EVALSHA";

    /// <summary>
    /// 将script 存储到redis lua缓存中 返回 存储的sha值
    /// </summary>
    public const string Script = "SCRIPT";

    #endregion

    #region Sentinel

    /// <summary>
    /// 哨兵命令
    /// </summary>
    public const string Sentinel = "SENTINEL";

    #endregion

    /// <summary>
    /// 获取实例的角色信息 
    /// </summary>
    public const string Role = "ROLE";


    #region tran

    /// <summary>
    /// 开启事务
    /// </summary>
    public const string Multi = "MULTI";

    /// <summary>
    /// 提交事务
    /// </summary>
    public const string Exec = "EXEC";

    /// <summary>
    /// 事务丢弃
    /// </summary>
    public const string DisCard = "DISCARD";

    /// <summary>
    /// 标记要监视的给定键以进行有条件的 交易执行。
    /// </summary>
    public const string Watch = "WATCH";

    /// <summary>
    /// 刷新所有先前监视的交易密钥。
    /// 如果您调用EXECor DISCARD，则无需手动调用UNWATCH.
    /// </summary>
    public const string UnWatch = "UNWATCH";

    #endregion

    #region hash

    /// <summary>
    /// 删除hash
    /// </summary>
    public const string HDel = "HDEL";

    /// <summary>
    /// hash存储
    /// </summary>
    public const string HSet = "HSET";

    /// <summary>
    /// hash是否存在
    /// </summary>
    public const string HExists = "HEXISTS";

    /// <summary>
    /// hash获取
    /// </summary>
    public const string HGet = "HGET";


    /// <summary>
    /// 获取所有的hash数据
    /// </summary>
    public const string HGetAll = "HGETALL";

    /// <summary>
    /// 递增
    /// </summary>
    public const string HIncrBy = "HINCRBYFLOAT";

    /// <summary>
    /// 获取hash的key信息
    /// </summary>
    public const string HKeys = "HKEYS";

    /// <summary>
    /// hash长度
    /// </summary>
    public const string HLen = "HLEN";


    /// <summary>
    /// 批量获取
    /// </summary>
    public const string HMGet = "HMGET";

    /// <summary>
    /// 返回hash的随机数据
    ///
    /// 当仅使用key参数调用时，从存储在 的哈希值中返回一个随机字段key。
    /// 如果提供的参数为正，则返回不同字段count的数组。数组的长度或者是散列的字段数 ( )，以较小者为准。countHLEN
    /// 如果用否定调用，行为会改变并且允许命令多次count返回相同的字段。在这种情况下，返回的字段数是指定的绝对值。count
    /// 可选WITHVALUES修饰符更改回复，使其包含随机选择的哈希字段的相应值。
    /// 6.2.0
    /// </summary>
    public const string HRandField = "HRANDFIELD";

    /// <summary>
    /// 获取hash的所有值
    /// </summary>
    public const string HVals = "HVALS";

    /// <summary>
    /// 获取hash 字段对应值的长度
    /// </summary>
    public const string HStrLen = "HSTRLEN";

    /// <summary>
    /// 设置hast的值 如果不存在的话 就添加
    /// </summary>
    public const string HSetNx = "HSETNX";

    /// <summary>
    /// 扫描
    /// </summary>
    public const string HScan = "HSCAN";

    #endregion

    #region set

    /// <summary>
    /// 添加
    /// 将指定的成员添加到存储在 的集合中key。已经是该集合成员的指定成员将被忽略。如果key不存在，则在添加指定成员之前创建一个新集。
    /// </summary>
    public const string SAdd = "SADD";

    /// <summary>
    /// 返回set里面集合的长度
    /// </summary>
    public const string SCard = "SCARD";

    /// <summary>
    /// 返回由第一个集合和所有后续集合之间的差异产生的集合成员。
    /// 类似 except
    /// </summary>
    public const string SDiff = "SDIFF";

    /// <summary>
    /// 此命令等于SDIFF，但不是返回结果集，而是存储在destination.
    /// 将except的差异值存到目标key
    /// </summary>
    public const string SDiffStore = "SDIFFSTORE";

    /// <summary>
    /// 返回由所有给定集的交集产生的集的成员。
    /// </summary>
    public const string SInter = "SINTER";

    #endregion
}