using System.Net.Sockets;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.RedisClients;

namespace RedisNaruto.Internal.Cluster;

/// <summary>
/// 集群客户端
/// </summary>
internal class ClusterRedisClient : RedisClient
{
    /// <summary>
    /// 
    /// </summary>
    public ClusterRedisClient(Guid connectionId, TcpClient tcpClient, ConnectionBuilder connectionBuilder,
        string currentHost,
        int currentPort,
        Action<IRedisClient> disposeTask) : base(connectionId, tcpClient, connectionBuilder, currentHost, currentPort,
        disposeTask)
    {
    }

    public override Task<bool> SelectDb(int db)
    {
        //集群只有一个db 不允许切换数据库
        return Task.FromResult(true);
    }

    public override Task InitClientIdAsync()
    {
        return Task.CompletedTask;
    }
}