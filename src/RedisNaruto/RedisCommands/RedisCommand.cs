using System.Diagnostics.CodeAnalysis;
using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;

namespace RedisNaruto;

public partial class RedisCommand : IRedisCommand
{
    private IRedisClientPool _redisClientPool;

    private RedisCommand(IRedisClientPool redisClientPool)
    {
        _redisClientPool = redisClientPool;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal static async Task<RedisCommand> ConnectionAsync(ConnectionModel config)
    {
        var redisClient = await RedisClient.ConnectionAsync(config.Connection, config.UserName, config.Password,config.DataBase);
        var redisCommand = new RedisCommand(redisClient);
        //连接配置
        return redisCommand;
    }

    public ValueTask DisposeAsync()
    {
        // return _redisClient.DisposeAsync();
    }
}