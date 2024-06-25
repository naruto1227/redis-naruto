using System.Net;
using System.Net.Sockets;
using RedisNaruto.Enums;
using RedisNaruto.Internal.Cluster;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Sentinels;

namespace RedisNaruto.Internal.RedisClients;

/// <summary>
/// 
/// </summary>
internal class RedisClientFactory : IRedisClientFactory
{
    private readonly ConnectionBuilder _connectionBuilder;
    

    private RedisClientFactory(ConnectionBuilder connectionBuilder)
    {
        _connectionBuilder = connectionBuilder;
    }

    public static async Task<RedisClientFactory> BuildAsync(ConnectionBuilder connectionBuilder)
    {
        var redisClientFactory = new RedisClientFactory(connectionBuilder);
        await ConnectionStateManage.InitAsync(connectionBuilder.Connection);
        return redisClientFactory;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposeTask"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IRedisClient> GetAsync(
        Action<IRedisClient> disposeTask, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var client = _connectionBuilder.ServerType switch
        {
            ServerType.Sentinel => await CreateSentinelClient(disposeTask, cancellationToken),
            ServerType.Cluster => await CreateClusterClient(disposeTask, cancellationToken),
            _ => await CreateSimpleClient(disposeTask, cancellationToken)
        };
        // await client.InitClientIdAsync();
        return client;
    }

    /// <summary>
    /// 创建普通客户端
    /// </summary>
    /// <returns></returns>
    private async Task<IRedisClient> CreateSimpleClient(Action<IRedisClient> disposeTask,
        CancellationToken cancellationToken)
    {
        var connectionInfo = await GetConnectionInfo(cancellationToken);
        return new RedisClient(connectionInfo.connectionId, connectionInfo.tcpClient, _connectionBuilder,
            connectionInfo.currentHost,
            connectionInfo.port,
            disposeTask);
    }

    /// <summary>
    /// 创建集群客户端
    /// </summary>
    /// <returns></returns>
    private async Task<IRedisClient> CreateClusterClient(Action<IRedisClient> disposeTask,
        CancellationToken cancellationToken)
    {
        var connectionInfo = await GetConnectionInfo(cancellationToken);
        return new ClusterRedisClient(connectionInfo.connectionId, connectionInfo.tcpClient, _connectionBuilder,
            connectionInfo.currentHost,
            connectionInfo.port,
            disposeTask);
    }

    /// <summary>
    /// 获取连接信息
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<(Guid connectionId, TcpClient tcpClient, string currentHost, int port)> GetConnectionInfo(
        CancellationToken cancellationToken)
    {
        var hostInfo = ConnectionStateManage.Get();
        //初始化tcp客户端
        var tcpClient = new TcpClient()
        {
            ReceiveTimeout = _connectionBuilder.TimeOut,
            SendTimeout = _connectionBuilder.TimeOut
        };
        //获取ip地址
        if (!IPAddress.TryParse(hostInfo.hostPort.Host, out var ips))
        {
            ips = (await Dns.GetHostAddressesAsync(hostInfo.hostPort.Host, cancellationToken)).FirstOrDefault();
        }

        //todo 增加连接拒绝的处理，拒绝后就直接 将当前主机信息填写进 失败数据中，然后后台服务检测 是否恢复
        await tcpClient.ConnectAsync(ips, hostInfo.hostPort.Port,
            cancellationToken);

        return (hostInfo.connectionId, tcpClient, ips.MapToIPv4().ToString(), hostInfo.hostPort.Port);
    }

    /// <summary>
    /// 创建哨兵客户端
    /// </summary>
    /// <returns></returns>
    private async Task<IRedisClient> CreateSentinelClient(Action<IRedisClient> disposeTask,
        CancellationToken cancellationToken)
    {
        var hostPort = await SentinelConnection.GetMaserAddressAsync(_connectionBuilder.MasterName, cancellationToken);
        //初始化tcp客户端
        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(hostPort.Host, hostPort.Port,
            cancellationToken);
        var sentinelClient =
            new SentinelRedisClient(default, tcpClient, _connectionBuilder, hostPort.Host, hostPort.Port, disposeTask);
        //当不是master节点的时候重新走流程
        if (await sentinelClient.IsMaterAsync()) return sentinelClient;
        //等待100ms
        await Task.Delay(100, cancellationToken).ConfigureAwait(false);
        return await CreateSentinelClient(disposeTask, cancellationToken).ConfigureAwait(false);
    }
}