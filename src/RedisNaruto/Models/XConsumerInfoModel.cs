using System.ComponentModel;

namespace RedisNaruto.Models;

/// <summary>
/// 消费者信息
/// </summary>
public struct XConsumerInfoModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pendingCount"></param>
    /// <param name="idle"></param>
    public XConsumerInfoModel(string name, int pendingCount, long idle)
    {
        Name = name;
        PendingCount = pendingCount;
        Idle = idle;
    }

    /// <summary>
    /// 消费者名称
    /// </summary>
    [Description("name")]
    public string Name { get; init; }
    
    /// <summary>
    ///未确定的消息数
    /// </summary>
    
    [Description("pending")]
    public int PendingCount { get; init; }

    /// <summary>
    /// 自消费者上次尝试交互以来经过的毫秒数（示例：XREADGROUP, XCLAIM, XAUTOCLAIM）
    /// </summary>
    
    [Description("idle")]
    public long Idle { get; init; }
}