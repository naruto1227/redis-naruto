using System.Net.Sockets;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Internal.RedisClients;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.Sentinels;

internal class SentinelRedisClient : RedisClient
{
    private readonly string _masterName;

    /// <summary>
    /// 
    /// </summary>
    public SentinelRedisClient(Guid connectionId, TcpClient tcpClient, ConnectionBuilder connectionBuilder,
        string currentHost,
        int currentPort,
        Func<IRedisClient, Task> disposeTask) : base(connectionId, tcpClient, connectionBuilder, currentHost,
        currentPort,
        disposeTask)
    {
        _masterName = connectionBuilder.MasterName;
    }

    /// <summary>
    /// 是否主库
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsMaterAsync()
    {
        var resultList = await ExecAsync(new Command(RedisCommandName.Role, default));
        foreach (var item in resultList)
        {
            if (item is RedisValue redisValue &&
                string.Compare(redisValue, "master", StringComparison.OrdinalIgnoreCase) > 0)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    private async Task<List<object>> ExecAsync(Command command)
    {
        return await ExecuteAsync<List<object>>(command);
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
            await base.ClearMessageAsync(cancellationToken);
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