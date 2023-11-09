namespace RedisNaruto.EventDatas;

public class ReceiveSubMessageEventData: EventData
{
    public ReceiveSubMessageEventData(object result) : base(nameof(ReceiveSubMessageEventData))
    {
        Result = result;
    }

    public object Result { get; init; }
}