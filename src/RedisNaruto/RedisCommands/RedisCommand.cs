using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;

namespace RedisNaruto.RedisCommands;

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
    internal static ValueTask<RedisCommand> ConnectionAsync(ConnectionModel config)
    {
        var redisCommand = new RedisCommand(new RedisClientPool(config.Connection, config.UserName, config.Password,
            config.DataBase, config.ConnectionPoolCount));
        //连接配置
        return new ValueTask<RedisCommand>(redisCommand);
    }

    public ValueTask DisposeAsync()
    {
        //todo 
        // return _redisClient.DisposeAsync();
        return new ValueTask();
    }
}