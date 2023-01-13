using System.Net.Sockets;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;

namespace RedisNaruto.Internal.Sentinels;

internal class SentinelRedisClient : RedisClient
{
    /// <summary>
    /// 
    /// </summary>
    public SentinelRedisClient(TcpClient tcpClient, ConnectionModel connectionModel, string currentHost,
        int currentPort,
        Func<IRedisClient, Task> disposeTask) : base(tcpClient,connectionModel,currentHost,currentPort,disposeTask)
    {
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
            if (string.Compare(item,"master",StringComparison.OrdinalIgnoreCase)>0)
            {
                return true;
            }
        }

        return false;
    }
}