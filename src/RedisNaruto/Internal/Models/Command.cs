namespace RedisNaruto.Internal.Models;

/// <summary>
/// 命令
/// </summary>
internal class Command
{
    private Command()
    {
    }

    public Command(string cmd, object[] args)
    {
        Cmd = cmd;
        Args = args;
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
    /// 组合参数
    /// </summary>
    /// <returns></returns>

    internal object[] CombinArgs()
    {
        var res = new object[(Args?.Length??0) + 1];
        res[0] = Cmd;
        if (Args!=null && Args.Length>0)
        {
            for (int i = 0; i < Args.Length; i++)
            {
                res[i + 1] = Args[i];
            }
        }

        return res;
    }
}