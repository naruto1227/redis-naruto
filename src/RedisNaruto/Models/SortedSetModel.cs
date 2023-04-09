namespace RedisNaruto.Models;

public class SortedSetModel
{
    public SortedSetModel(RedisValue member)
    {
        Member = member;
        Score = 0;
    }

    public RedisValue Member { get; }

    public long Score { get; internal set; }
}