namespace RedisNaruto.Enums;

/// <summary>
/// 过期命令
/// </summary>
public enum ExpireEnum
{
    /// <summary>
    /// 仅当新到期时间小于当前到期时间时才设置到期时间
    /// 7.0.0
    /// </summary>
    LT,

    /// <summary>
    ///  仅当新到期时间大于当前到期时间时才设置到期时间
    /// 7.0.0
    /// </summary>
    GT,

    /// <summary>
    /// 仅在密钥没有过期时设置过期 NX
    /// 7.0.0
    /// </summary>
    NX,

    /// <summary>
    /// 仅当密钥有一个现有的到期时才设置到期
    /// 7.0.0
    /// </summary>
    XX,

    /// <summary>
    /// 不进行任何操作
    /// </summary>
    No,
}