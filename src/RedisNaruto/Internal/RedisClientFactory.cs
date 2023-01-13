using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Sentinels;

namespace RedisNaruto.Internal;

/// <summary>
/// 
/// </summary>
internal class RedisClientFactory : IRedisClientFactory
{
    private static readonly Random Random = new Random();

    /// <summary>
    /// 
    /// </summary>
    private readonly ISentinelConnection _sentinelConnection;

    private readonly ConnectionModel _connectionModel;

    public RedisClientFactory(ConnectionModel connectionModel)
    {
        _connectionModel = connectionModel;
        if (connectionModel.IsEnableSentinel)
        {
            _sentinelConnection = new SentinelConnection(connectionModel.Connection, _connectionModel.MasterName);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposeTask"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IRedisClient> GetAsync(
        Func<IRedisClient, Task> disposeTask, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_connectionModel.IsEnableSentinel)
        {
            return await CreateSentinelClient(disposeTask, cancellationToken);
        }

        return await CreateSimpleClient(disposeTask, cancellationToken);
    }

    /// <summary>
    /// 创建普通客户端
    /// </summary>
    /// <returns></returns>
    private async Task<IRedisClient> CreateSimpleClient(Func<IRedisClient, Task> disposeTask,
        CancellationToken cancellationToken)
    {
        var current = _connectionModel.Connection[Random.Next(_connectionModel.Connection.Length)];
        var hostString = current.Split(":");
        if (!int.TryParse(hostString[1], out var port))
        {
            port = 6349;
        }

        //初始化tcp客户端
        var tcpClient = new TcpClient();
        //获取ip地址
        var ips = await Dns.GetHostAddressesAsync(hostString[0], cancellationToken);
        await tcpClient.ConnectAsync(ips, port,
            cancellationToken);
        var currentHost = string.Join(',',
            ips.OrderBy(a => a.ToString()).Select(x => x.MapToIPv4().ToString()).ToArray());
        return new RedisClient(tcpClient, _connectionModel, currentHost, port, disposeTask);
    }

    /// <summary>
    /// 创建哨兵客户端
    /// </summary>
    /// <returns></returns>
    private async Task<IRedisClient> CreateSentinelClient(Func<IRedisClient, Task> disposeTask,
        CancellationToken cancellationToken)
    {
        var hostPort = await _sentinelConnection.GetMaserHostPort(cancellationToken);
        //初始化tcp客户端
        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(hostPort.Host, hostPort.Port,
            cancellationToken);
        var sentinelClient =
            new SentinelRedisClient(tcpClient, _connectionModel, hostPort.Host, hostPort.Port, disposeTask);
        //当不是master节点的时候重新走流程
        if (await sentinelClient.IsMaterAsync()) return sentinelClient;
        //等待100ms
        await Task.Delay(100,cancellationToken).ConfigureAwait(false);
        return await CreateSentinelClient(disposeTask, cancellationToken).ConfigureAwait(false);
    }
}