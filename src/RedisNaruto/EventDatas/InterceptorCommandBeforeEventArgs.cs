using RedisNaruto.Internal.Models;

namespace RedisNaruto.EventDatas;

/// <summary>
/// 执行前
/// </summary>
public class InterceptorCommandBeforeEventArgs: EventArgs
{
    internal InterceptorCommandBeforeEventArgs(Command command)
    {
        Command = command;
    }

    /// <summary>
    /// 
    /// </summary>
    public Command Command { get; init; }

    /// <summary>
    /// 是否缓存
    /// </summary>
    public bool IsCache { get; set; }
    
    /// <summary>
    /// 返回值
    /// </summary>
    public object Value { get; init; }
}