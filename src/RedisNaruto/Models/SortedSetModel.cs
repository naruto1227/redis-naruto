namespace RedisNaruto.Models;

public class SortedSetModel
{
    public SortedSetModel(object member)
    {
        Member = member;
    }

    public object Member { get; init; }

    public long Score { get; internal set; }
}