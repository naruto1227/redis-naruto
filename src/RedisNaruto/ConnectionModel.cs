namespace RedisNaruto;

/// <summary>
/// 连接模型
/// </summary>
public class ConnectionModel
{
    /// <summary>
    /// 链接地址
    /// </summary>
    public string[] Connection { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }
}