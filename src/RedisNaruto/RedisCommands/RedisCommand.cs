using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.RedisResolvers;
using RedisNaruto.Internal.Serialization;
using RedisNaruto.Models;

namespace RedisNaruto.RedisCommands;

public partial class RedisCommand : IRedisCommand
{
    private readonly IRedisClientPool _redisClientPool;
    //
    // internal IRedisClient RedisClient;

    /// <summary>
    /// 序列化
    /// </summary>
    private static readonly ISerializer _serializer = new Serializer();

    internal readonly IRedisResolver RedisResolver;

    internal RedisCommand(IRedisResolver redisResolver, IRedisClientPool redisClientPool)
    {
        RedisResolver = redisResolver;
        _redisClientPool = redisClientPool;
    }

    private RedisCommand(IRedisClientPool redisClientPool)
    {
        _redisClientPool = redisClientPool;
        RedisResolver = new DefaultRedisResolver(redisClientPool);
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="redisValue"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    private static async Task<TResult> DeserializeAsync<TResult>(byte[] redisValue) => await
        _serializer.DeserializeAsync<TResult>(redisValue);

    // private void ChangeRedisClient(IRedisClient redisClient)
    // {
    //     RedisClient = redisClient;
    // }


    // /// <summary>
    // /// 获取redis客户端
    // /// </summary>
    // /// <param name="cancellationToken"></param>
    // /// <returns></returns>
    // internal async Task<IRedisClient> GetRedisClient(CancellationToken cancellationToken)
    // {
    //     if (RedisClient != default)
    //     {
    //         return RedisClient;
    //     }
    //
    //     return await RedisClientPool.RentAsync(cancellationToken);
    // }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal static ValueTask<RedisCommand> BuilderAsync(ConnectionBuilder config)
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
        // if (isDispose && RedisClient != default)
        // {
        //     await RedisClientPool.ReturnAsync(RedisClient);
        //     RedisClient = null;
        // }
    }
}