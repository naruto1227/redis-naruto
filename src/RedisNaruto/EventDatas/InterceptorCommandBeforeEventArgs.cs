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
    public bool IsCache { get; private set; }
    
    /// <summary>
    /// 返回值
    /// </summary>
    public object Value { get; private set; }

    /// <summary>
    /// 设置返回值
    /// </summary>
    /// <param name="value"></param>
    public void SetRetValue(object value)
    {
        this.Value = value;
        this.IsCache = true;
    }
}