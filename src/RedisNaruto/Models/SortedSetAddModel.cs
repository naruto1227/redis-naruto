namespace RedisNaruto.Models;

/// <summary>
/// 
/// </summary>
public struct SortedSetAddModel
{
    public SortedSetAddModel(long score, object member)
    {
        this.Score = score;
        this.Member = member;
    }

    /// <summary>
    /// 元素信息
    /// </summary>
    public object Member { get; init; }

    /// <summary>
    /// 排行信息
    /// </summary>
    public long Score { get; init; }
}