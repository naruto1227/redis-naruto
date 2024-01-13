namespace RedisNaruto.Internal.Enums;

/// <summary>
/// 消息类型
/// </summary>
public enum RespMessageTypeEnum
{
    /// <summary>
    /// 
    /// </summary>
    Default,
    /// <summary>
    /// 简单的字符串返回
    /// </summary>
    SimpleString,

    /// <summary>
    /// 错误
    /// </summary>
    Error,

    /// <summary>
    /// 数字
    /// </summary>
    Number,

    /// <summary>
    /// 批量字符串
    /// </summary>
    BulkStrings,

    /// <summary>
    /// 数组
    /// </summary>
    ArrayString,


    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#nulls
    /// </summary>
    Nulls,

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#booleans
    /// </summary>
    Bool,

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#doubles
    /// </summary>
    Double,

    /// <summary>
    /// Big numbers
    /// https://redis.io/docs/reference/protocol-spec/#big-numbers
    /// </summary>
    BigNumber,

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#bulk-errors
    /// </summary>
    BuckError,

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#verbatim-strings
    /// </summary>
    VerString,

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#maps
    /// </summary>
    Maps,

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#sets
    /// </summary>
    Sets,

    /// <summary>
    /// https://redis.io/docs/reference/protocol-spec/#pushes
    /// </summary>
    Pushs,
    
}