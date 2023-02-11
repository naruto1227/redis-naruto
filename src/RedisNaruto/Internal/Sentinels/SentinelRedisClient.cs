using System.Net.Sockets;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;

namespace RedisNaruto.Internal.Sentinels;

internal class SentinelRedisClient : RedisClient
{
    private readonly string _masterName;

    /// <summary>
    /// 
    /// </summary>
    public SentinelRedisClient(Guid connectionId, TcpClient tcpClient, ConnectionModel connectionModel,
        string currentHost,
        int currentPort,
        Func<IRedisClient, Task> disposeTask) : base(connectionId, tcpClient, connectionModel, currentHost, currentPort,
        disposeTask)
    {
        _masterName = connectionModel.MasterName;
    }

    /// <summary>
    /// 是否主库
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsMaterAsync()
    {
        var resultList = ExecuteMoreResultAsync<string>(new Command(RedisCommandName.Role, default));
        await foreach (var item in resultList)
        {
            if (string.Compare(item, "master", StringComparison.OrdinalIgnoreCase) > 0)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 重置
    /// </summary>
    /// <returns></returns>
    public override async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        //判断master库 是否切换
        var hostPort = await SentinelConnection.GetMaserAddressAsync(_masterName, cancellationToken);
        if (this.CurrentHost == hostPort.Host && this.CurrentPort == hostPort.Port)
        {
            return;
        }

        IsAuth = false;
        //释放连接
        this.TcpClient.Dispose();
        this.TcpClient = null;
        //重新打开一个新的连接
        var localClient = new TcpClient();
        await localClient.ConnectAsync(hostPort.Host, hostPort.Port, cancellationToken);
        this.TcpClient = localClient;
    }
}