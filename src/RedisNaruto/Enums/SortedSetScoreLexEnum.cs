namespace RedisNaruto.Enums;

public enum SortedSetScoreLexEnum
{
    /// <summary>
    /// 不做处理
    /// </summary>
    Defaut,
    /// <summary>
    /// 当BYSCORE提供该选项时，该命令的行为类似于并返回排序集中具有等于或介于和ZRANGEBYSCORE之间的元素范围。<start><stop>
    /// </summary>
    ByScore,
    /// <summary>
    /// BYLEX使用该选项时，该命令的行为类似于并返回和字典序封闭范围ZRANGEBYLEX区间之间的排序集中的元素范围。<start><stop>
    /// </summary>
    ByLex
}