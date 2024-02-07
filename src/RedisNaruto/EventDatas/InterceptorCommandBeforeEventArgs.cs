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
}