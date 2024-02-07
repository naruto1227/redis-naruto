using RedisNaruto.Internal.Models;

namespace RedisNaruto.EventDatas;

/// <summary>
/// 执行后
/// </summary>
public class InterceptorCommandAfterEventArgs:EventArgs
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="value"></param>
    /// <param name="ex"></param>
    internal InterceptorCommandAfterEventArgs(Command command, object value,Exception ex=null)
    {
        Command = command;
        Value = value;
        Ex = ex;
    }

    /// <summary>
    /// 指令
    /// </summary>
    public Command Command { get; init; }

    /// <summary>
    /// 返回值
    /// </summary>
    public object Value { get; init; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public Exception? Ex { get; init; }
}