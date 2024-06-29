namespace RedisNaruto.EventDatas;

public class ClientSideCachingStartEventData : EventData
{
    /// <summary>
    /// 启用客户端缓存
    /// </summary>
    public ClientSideCachingStartEventData(string clientId, object result) : base(nameof(ClientSideCachingStartEventData))
    {
        ClientId = clientId;
        Result = result;
    }

    /// <summary>
    /// 客户端id
    /// </summary>
    public string ClientId { get;  }

    /// <summary>
    /// 返回结果
    /// </summary>
    public object Result { get; }
}