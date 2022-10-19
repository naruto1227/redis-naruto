namespace RedisNaruto.Internal;

/// <summary>
/// 消息协议
/// https://redis.io/docs/reference/protocol-spec
/// </summary>
public static class RespMessage
{
    /// <summary>
    /// 简单的字符串返回
    /// </summary>
    public const char SimpleString = '+';

    /// <summary>
    /// 错误
    /// </summary>
    public const char Error = '-';

    /// <summary>
    /// 数字
    /// </summary>
    public const char Number = ':';

    /// <summary>
    /// 批量字符串
    /// </summary>
    public const char BulkStrings = '$';

    /// <summary>
    /// 数组
    /// </summary>
    public const char ArrayString = '*';
}