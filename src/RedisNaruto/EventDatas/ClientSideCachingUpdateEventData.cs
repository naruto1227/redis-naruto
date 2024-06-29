namespace RedisNaruto.EventDatas;

public class ClientSideCachingUpdateEventData : EventData
{
    public ClientSideCachingUpdateEventData(string key, object value) : base(nameof(ClientSideCachingUpdateEventData))
    {
        this.Key = key;
        Value = value;
    }

    /// <summary>
    /// key
    /// </summary>
    public string Key { get;  }
    /// <summary>
    /// 值
    /// </summary>
    public object Value { get;}
}