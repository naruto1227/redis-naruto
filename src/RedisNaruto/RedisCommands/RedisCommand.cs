using System.Diagnostics.CodeAnalysis;
using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;

namespace RedisNaruto;

public partial class RedisCommand : IRedisCommand
{
    private IRedisClient _redisClient;

    private RedisCommand(IRedisClient redisClient)
    {
        _redisClient = redisClient;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal static async Task<RedisCommand> ConnectionAsync(ConnectionModel config)
    {
        var redisClient = await RedisClient.ConnectionAsync(config.Connection, config.UserName, config.Password);
        var redisCommand = new RedisCommand(redisClient);
        //连接配置
        return redisCommand;
    }

    public ValueTask DisposeAsync()
    {
        return _redisClient.DisposeAsync();
    }
}