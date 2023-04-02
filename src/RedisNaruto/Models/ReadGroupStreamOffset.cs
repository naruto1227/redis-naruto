namespace RedisNaruto.Models;

public readonly struct ReadGroupStreamOffset
{
    /// <summary>
    /// 从消费者的pending队列开头读取
    /// </summary>
    public const string BeginMessage = "0";

    /// <summary>
    /// 新消息
    /// </summary>
    public const string NewMessage = ">";

    public ReadGroupStreamOffset(string key, string offset)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(offset);
        Key = key;
        Offset = offset;
    }

    /// <summary>
    /// 流信息
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// 指定的流读取偏移量
    /// </summary>
    public string Offset { get; }
}