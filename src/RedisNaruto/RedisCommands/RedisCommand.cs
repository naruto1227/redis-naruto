using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Serialization;
using RedisNaruto.Models;

namespace RedisNaruto.RedisCommands;

public partial class RedisCommand : IRedisCommand
{
    internal IRedisClientPool RedisClientPool;

    internal IRedisClient RedisClient;

    /// <summary>
    /// 序列化
    /// </summary>
    private static readonly ISerializer _serializer = new Serializer();

    protected RedisCommand()
    {
    }

    private RedisCommand(IRedisClientPool redisClientPool)
    {
        RedisClientPool = redisClientPool;
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="redisValue"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    private static async Task<TResult> DeserializeAsync<TResult>(byte[] redisValue) => await
        _serializer.DeserializeAsync<TResult>(redisValue);

    private void ChangeRedisClient(IRedisClient redisClient)
    {
        RedisClient = redisClient;
    }


    /// <summary>
    /// 获取redis客户端
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task<IRedisClient> GetRedisClient(CancellationToken cancellationToken)
    {
        if (RedisClient != default)
        {
            return RedisClient;
        }

        return await RedisClientPool.RentAsync(cancellationToken);
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
        if (isDispose && RedisClient != default)
        {
            await RedisClientPool.ReturnAsync(RedisClient);
            RedisClient = null;
        }
    }
}