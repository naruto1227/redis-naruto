namespace RedisNaruto.EventDatas;

public class ClientSideCachingRemoveEventData : EventData
{
    public ClientSideCachingRemoveEventData(string key) : base(nameof(ClientSideCachingRemoveEventData))
    {
        this.Key = key;
    }

    public string Key { get;}
}