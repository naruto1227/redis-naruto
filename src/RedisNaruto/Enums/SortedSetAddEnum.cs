namespace RedisNaruto.Enums;

public enum SortedSetAddEnum
{
    /// <summary>
    /// 新的分数小于当前值 才更新 LT
    /// 6.2.0
    /// </summary>
    LessThan,

    /// <summary>
    /// 新的分数大与当前值 才更新 GT
    /// 6.2.0
    /// </summary>
    GreaterThan,

    /// <summary>
    /// 只更新不存在的元素 NX
    /// </summary>
    NotExists,

    /// <summary>
    /// 只更新存在的元素 XX
    /// </summary>
    Exists,

    /// <summary>
    /// 不进行任何操作
    /// </summary>
    No,
}