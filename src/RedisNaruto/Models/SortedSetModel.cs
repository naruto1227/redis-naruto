namespace RedisNaruto.Models;

public class SortedSetModel
{
    public SortedSetModel(object member)
    {
        Member = member;
        Score = 0;
    }

    public object Member { get; }

    public long Score { get; internal set; }
}