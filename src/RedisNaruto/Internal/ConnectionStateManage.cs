using System.Buffers;
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net.Sockets;
using RedisNaruto.Exceptions;
using RedisNaruto.Internal.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.Internal;

/// <summary>
/// todo 移除
/// 连接状态管理
/// 所有的连接信息从此对像中获取
/// </summary>
internal static class ConnectionStateManage
{
    private static readonly ConcurrentDictionary<Guid, ConnectionState> ConnectionStates = new();

    private static readonly ConcurrentDictionary<Guid, TcpClient> TcpClients = new();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="connections"></param>
    public static async Task InitAsync(IEnumerable<string> connections)
    {
        foreach (var item in connections)
        {
            var hostString = item.Split(":");
            if (!int.TryParse(hostString[1], out var port))
            {
                throw new ArgumentException(nameof(port));
            }

            ConnectionStates.TryAdd(Guid.NewGuid(), new ConnectionState(hostString[0], port));
        }

        //初始的时候 进行一次地址有效的判断
        foreach (var connectionState in ConnectionStates)
        {
            await HealCheckCoreAsync(connectionState);
        }

        //开启后台服务
        new Thread(HealCheckAsync).Start();
        new Thread(InValidHealCheckAsync).Start();
    }

    /// <summary>
    /// ping消息
    /// </summary>
    private static readonly byte[] Ping =
        $"{RespMessage.ArrayString}1\r\n{RespMessage.BulkStrings}{4}\r\nPing\r\n".ToEncode();

    /// <summary>
    /// 健康检查
    /// </summary>
    private static async void HealCheckAsync()
    {
        //30s 检查一次
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
        while (await timer.WaitForNextTickAsync())
        {
            //遍历有效的
            foreach (var connectionState in ConnectionStates.Where(a => a.Value.State == ConnectionStateEnum.Valid))
            {
                await HealCheckCoreAsync(connectionState);
            }
        }
    }

    /// <summary>
    /// 失效连接检查
    /// </summary>
    private static async void InValidHealCheckAsync()
    {
        //3分钟 检查一次 todo 配置化时间
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(2));
        while (await timer.WaitForNextTickAsync())
        {
            //检查失效的
            foreach (var connectionState in ConnectionStates.Where(a => a.Value.State == ConnectionStateEnum.InValid))
            {
                //存在连接没有释放的需要先释放  SetInValid 会造成需要此操作
                if (TcpClients.TryRemove(connectionState.Key, out var tcp))
                {
                    //资源释放
                    tcp.Dispose();
                    tcp = null;
                }

                await HealCheckCoreAsync(connectionState);
            }
        }
    }

    /// <summary>
    /// 检查连接是否有效
    /// </summary>
    /// <param name="connectionState"></param>
    private static async Task HealCheckCoreAsync(KeyValuePair<Guid, ConnectionState> connectionState)
    {
        try
        {
            //获取tcp客户端
            if (!TcpClients.TryGetValue(connectionState.Key, out var tcp))
            {
                //初始化
                tcp = new TcpClient()
                {
                    NoDelay = true,
                    ReceiveTimeout = 3000
                };
                await tcp.ConnectAsync(connectionState.Value.Host, connectionState.Value.Port);
                TcpClients.TryAdd(connectionState.Key, tcp);
            }

            //模拟发送ping消息 如果有回复代表连接正常 没有断开
            var stream = tcp.GetStream();
            //写入消息
            await stream.WriteAsync(Ping);
            //读取回复
            using (var memory = MemoryPool<byte>.Shared.Rent(512))
            {
                var es = await stream.ReadAsync(memory.Memory);
                //没有任何消息代表失效
                if (es == 0)
                {
                    connectionState.Value.SetInValid("es=0");
                    return;
                }
            }

            connectionState.Value.SetValid();
        }
        catch (SocketException socketError)
        {
            connectionState.Value.SetInValid(socketError.GetBaseException().Message);
        }
        catch (IOException ioException)
        {
            connectionState.Value.SetInValid(ioException.GetBaseException().Message);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"连接失败,host={connectionState.Value.Host},port={connectionState.Value.Port}");
            connectionState.Value.SetInValid(e.GetBaseException().Message);
        }
        finally
        {
            //判断是否为失效
            if (connectionState.Value.State == ConnectionStateEnum.InValid &&
                TcpClients.TryRemove(connectionState.Key, out var tcp))
            {
                //资源释放
                tcp.Dispose();
                tcp = null;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static (Guid connectionId, HostPort hostPort) Get()
    {
        var info = ConnectionStates.Where(a => a.Value.State == ConnectionStateEnum.Valid).OrderBy(a => Guid.NewGuid())
            .Select(a => new
            {
                a.Key,
                a.Value.Host,
                a.Value.Port
            }).FirstOrDefault();
        if (info == null)
        {
            throw new NotConnectionException();
        }

        return (info.Key, new HostPort(info.Host, info.Port));
    }

    /// <summary>
    /// 设置无效
    /// </summary>
    /// <param name="connectionId"></param>
    public static void SetInValid(Guid connectionId)
    {
        if (!ConnectionStates.TryGetValue(connectionId, out var connectionState)) return;
        connectionState.SetInValid("conn error");
    }
}