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

    #region resp3

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#nulls
    /// </summary>
    public const char Nulls = '_';

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#booleans
    /// </summary>
    public const char Bool = '#';

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#doubles
    /// </summary>
    public const char Double = ',';

    /// <summary>
    /// Big numbers
    /// https://redis.io/docs/reference/protocol-spec/#big-numbers
    /// </summary>
    public const char BigNumber = '(';

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#bulk-errors
    /// </summary>
    public const char BuckError = '!';

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#verbatim-strings
    /// </summary>
    public const char VerString = '=';

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#maps
    /// </summary>
    public const char Maps = '%';

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#sets
    /// </summary>
    public const char Sets = '~';

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#pushes
    /// </summary>
    public const char Pushs = '>';

    #endregion
}