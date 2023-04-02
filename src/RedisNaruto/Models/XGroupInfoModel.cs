using System.ComponentModel;

namespace RedisNaruto.Models;

public readonly struct XGroupInfoModel
{
    public XGroupInfoModel(string name,int consumerCount, int pendingCount, string lastDeliveredId, string entriesRead, int? lag)
    {
        Name = name;
        ConsumerCount = consumerCount;
        PendingCount = pendingCount;
        LastDeliveredId = lastDeliveredId;
        EntriesRead = entriesRead;
        Lag = lag;
    }

    /// <summary>
    /// 消费者组名称
    /// </summary>
    [Description("name")]
    public string Name { get; init; }

    /// <summary>
    /// 消费者数量
    /// </summary>
    [Description("consumers")]
    public int ConsumerCount { get;init; }

    /// <summary>
    /// 组内的未确定的消息数
    /// </summary>
    [Description("pending")]
    public int PendingCount { get; init; }

    /// <summary>
    /// 最后一个条目的 ID 
    /// </summary>
    [Description("last-delivered-id")]
    public string LastDeliveredId { get;init; }

    /// <summary>
    /// 传递给组消费者的最后一个条目的逻辑“读取计数器”
    /// </summary>
    [Description("entries-read")]
    public string EntriesRead { get; init; }

    /// <summary>
    /// 流中仍在等待传递给组消费者的条目数，或者当无法确定该数字时为 NULL。
    /// </summary>
    [Description("lag")]
    public int? Lag { get;init; }
}