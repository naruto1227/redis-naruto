namespace RedisNaruto.Internal;

internal static class RedisCommandName
{
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

    #endregion

}