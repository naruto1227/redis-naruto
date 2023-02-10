using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;

namespace RedisNaruto.RedisCommands;

public partial class RedisCommand : IRedisCommand
{
    internal IRedisClientPool _redisClientPool;

    internal IRedisClient _redisClient;

    protected RedisCommand()
    {
    }

    private RedisCommand(IRedisClientPool redisClientPool)
    {
        _redisClientPool = redisClientPool;
    }

    internal void ChangeRedisClient(IRedisClient redisClient)
    {
        _redisClient = redisClient;
    }


    /// <summary>
    /// 获取redis客户端
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task<IRedisClient> GetRedisClient(CancellationToken cancellationToken)
    {
        if (_redisClient != default)
        {
            return _redisClient;
        }

        return await _redisClientPool.RentAsync(cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal static ValueTask<RedisCommand> ConnectionAsync(ConnectionModel config)
    {
        var redisCommand = new RedisCommand(new RedisClientPool(config));
        //连接配置
        return new ValueTask<RedisCommand>(redisCommand);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeCoreAsync(true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeCoreAsync(bool isDispose)
    {
        if (isDispose && _redisClient != default)
        {
            await _redisClientPool.ReturnAsync(_redisClient);
            _redisClient = null;
        }
    }
}