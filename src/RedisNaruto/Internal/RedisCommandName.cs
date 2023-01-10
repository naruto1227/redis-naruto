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

    #endregion
}