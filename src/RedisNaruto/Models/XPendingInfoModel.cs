namespace RedisNaruto.Models;

public struct XPendingInfoModel
{
    internal XPendingInfoModel(string messageId, string consumerName, long idleTimeInMs, int deliveryCount)
    {
        MessageId = messageId;
        ConsumerName = consumerName;
        IdleTimeInMilliseconds = idleTimeInMs;
        DeliveryCount = deliveryCount;
    }

    /// <summary>
    /// 消息id
    /// </summary>
    public string MessageId { get; }

    /// <summary>
    /// 消费者名称
    /// </summary>
    public string ConsumerName { get; }

    /// <summary>
    /// 自上次将此消息传递给此消费者以来经过的毫秒数。
    /// </summary>
    public long IdleTimeInMilliseconds { get; }

    /// <summary>
    ///消息传递次数
    /// </summary>
    public int DeliveryCount { get; }
}