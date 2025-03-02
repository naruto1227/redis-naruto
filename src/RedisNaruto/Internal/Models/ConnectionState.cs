using System.Diagnostics;

namespace RedisNaruto.Internal.Models;

internal class ConnectionState
{
    public ConnectionState(string host, int port)
    {
        this.Host = host;
        this.Port = port;
        State = ConnectionStateEnum.Valid;
        //todo 考虑维护一个失败的次数，如果达到指定的次数 不再连接，直接废弃当前主机
    }

    public string Host { get; init; }

    public int Port { get; init; }

    /// <summary>
    /// 状态
    /// </summary>
    public ConnectionStateEnum State { get; private set; }


    /// <summary>
    /// 失效原因
    /// </summary>
    public string InValidReason { get; private set; }

    /// <summary>
    /// 设置有效
    /// </summary>
    public void SetValid()
    {
        this.State = ConnectionStateEnum.Valid;
        this.InValidReason = string.Empty;
        Console.WriteLine($"连接恢复:host={this.Host},port={this.Port}");
    }

    /// <summary>
    /// 设置无效
    /// </summary>
    public void SetInValid(string inValidReason)
    {
        this.State = ConnectionStateEnum.InValid;
        this.InValidReason = inValidReason;
        Console.WriteLine($"连接失效:host={this.Host},port={this.Port}");
    }
}

internal enum ConnectionStateEnum
{
    /// <summary>
    /// 有效
    /// </summary>
    Valid,

    /// <summary>
    /// 失效
    /// </summary>
    InValid
}