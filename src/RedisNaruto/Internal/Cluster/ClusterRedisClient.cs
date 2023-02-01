using System.Net.Sockets;
using RedisNaruto.Internal.Interfaces;

namespace RedisNaruto.Internal.Cluster;

/// <summary>
/// 集群客户端
/// </summary>
internal class ClusterRedisClient : RedisClient
{
    /// <summary>
    /// 
    /// </summary>
    public ClusterRedisClient(Guid connectionId,TcpClient tcpClient, ConnectionModel connectionModel, string currentHost,
        int currentPort,
        Func<IRedisClient, Task> disposeTask) : base(connectionId,tcpClient, connectionModel, currentHost, currentPort, disposeTask)
    {
    }

    public override Task<bool> SelectDb(int db)
    {
        //集群只有一个db 不允许切换数据库
        return Task.FromResult(true);
    }
}