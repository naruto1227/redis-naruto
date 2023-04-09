namespace RedisNaruto.Models;

public readonly struct StreamEntityModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="messageId"></param>
    internal StreamEntityModel(string messageId, Dictionary<string, RedisValue> entitys)
    {
        MessageId = messageId;
        Entitys = entitys;
    }

    /// <summary>
    /// 消息id
    /// </summary>
    public string MessageId { get; init; }

    /// <summary>
    /// 实体信息
    /// </summary>
    public Dictionary<string, RedisValue> Entitys { get; init; }
}