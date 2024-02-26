using RedisNaruto.Internal.Interfaces;

namespace RedisNaruto.Internal.RedisResolvers;

/// <summary>
/// 客户端缓存服务
/// </summary>
internal class ClientSideCachingRedisResolver: DefaultRedisResolver, IAsyncDisposable
{
    public ClientSideCachingRedisResolver(IRedisClientPool redisClientPool) : base(redisClientPool)
    {
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}