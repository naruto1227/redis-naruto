using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net.Sockets;
using RedisNaruto.Internal.Models;

namespace RedisNaruto.Internal;

/// <summary>
/// 连接状态管理
/// 所有的连接信息从此对像中获取
/// </summary>
internal static class ConnectionStateManage
{
    private static readonly ConcurrentDictionary<Guid, ConnectionState> _connectionStates = new();

    /// <summary>
    /// 锁
    /// </summary>
    private static object _lock = new();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="connections"></param>
    public static void Init(IEnumerable<string> connections)
    {
        if (_connectionStates is {Count: > 0})
        {
            return;
        }

        lock (_lock)
        {
            if (_connectionStates is {Count: > 0})
            {
                return;
            }

            //
            foreach (var item in connections)
            {
                var hostString = item.Split(":");
                if (!int.TryParse(hostString[1], out var port))
                {
                    port = 6349;
                }

                _connectionStates.TryAdd(Guid.NewGuid(), new ConnectionState(hostString[0], port));
            }

            //开启后台服务
            Task.Factory.StartNew(async () => await HealCheckAsync(), default, TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }
    }

    /// <summary>
    /// 健康检查
    /// </summary>
    private static async Task HealCheckAsync()
    {
        //15s 检查一次
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));
        while (await timer.WaitForNextTickAsync())
        {
            foreach (var connectionState in _connectionStates)
            {
                try
                {
                    using var tcp = new TcpClient();
                    await tcp.ConnectAsync(connectionState.Value.Host, connectionState.Value.Port);
                    connectionState.Value.SetVaild();
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"连接失败,host={connectionState.Value.Host},port={connectionState.Value.Port}");
                    connectionState.Value.SetInVaild();
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static (Guid connectionId, HostPort hostPort) Get()
    {
        var info = _connectionStates.Where(a => a.Value.State == ConnectionStateEnum.Vaild)
            .Select(a => new
            {
                a.Key,
                a.Value.Host,
                a.Value.Port
            }).FirstOrDefault();
        if (info == null)
        {
            throw new InvalidOperationException("没有可用的连接");
        }

        return (info.Key, new HostPort(info.Host, info.Port));
    }

    /// <summary>
    /// 设置无效
    /// </summary>
    /// <param name="connectionId"></param>
    public static void SetInVaild(Guid connectionId)
    {
        if (_connectionStates.TryGetValue(connectionId, out var connectionState))
        {
            connectionState.SetInVaild();
        }
    }
}

/// <summary>
/// 连接状态
/// </summary>
internal struct ConnectionState
{
    public ConnectionState(string host, int port)
    {
        this.Host = host;
        this.Port = port;
        State = ConnectionStateEnum.Vaild;
    }

    public string Host { get; init; }

    public int Port { get; init; }

    /// <summary>
    /// 状态
    /// </summary>
    public ConnectionStateEnum State { get; private set; }

    /// <summary>
    /// 设置有效
    /// </summary>
    public void SetVaild()
    {
        this.State = ConnectionStateEnum.Vaild;
    }

    /// <summary>
    /// 设置无效
    /// </summary>
    public void SetInVaild()
    {
        this.State = ConnectionStateEnum.InVaild;
    }
}

internal enum ConnectionStateEnum
{
    /// <summary>
    /// 有效
    /// </summary>
    Vaild,

    /// <summary>
    /// 失效
    /// </summary>
    InVaild
}