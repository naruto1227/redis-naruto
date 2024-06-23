using RedisNaruto.Models;

namespace RedisNaruto.Internal.Models;

/// <summary>
/// 命令
/// </summary>
public sealed class Command
{
    private Command()
    {
    }

    public Command(string cmd, object[] args,string key=default)
    {
        Cmd = cmd;
        Args = args;
        Key = key;
        Length = ((Args?.Length) ?? 0) + 1;
    }

    /// <summary>
    /// 命令
    /// </summary>
    public string Cmd { get; }

    /// <summary>
    /// 操作的key
    /// </summary>
    public string Key { get; }
    /// <summary>
    /// 参数
    /// </summary>
    public object[] Args { get; }

    /// <summary>
    /// 长度
    /// </summary>
    public int Length { get; }
}