using RedisNaruto.Models;

namespace RedisNaruto.Internal.Models;

/// <summary>
/// 命令
/// </summary>
internal sealed class Command
{
    private Command()
    {
    }

    public Command(string cmd, object[] args)
    {
        Cmd = cmd;
        Args = args;
        Length = ((Args?.Length) ?? 0) + 1;
    }

    /// <summary>
    /// 命令
    /// </summary>
    public string Cmd { get; }

    /// <summary>
    /// 参数
    /// </summary>
    public object[] Args { get; }

    /// <summary>
    /// 长度
    /// </summary>
    public int Length { get; }
}