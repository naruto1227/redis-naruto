using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.RedisResolvers;

namespace RedisNaruto.RedisCommands;

public class SelectDbRedisCommand : RedisCommand, ISelectDbRedisCommand
{
    private readonly SelectDbRedisResolver _redisResolver;

    internal SelectDbRedisCommand(SelectDbRedisResolver redisResolver, IRedisClientPool redisClientPool) : base(redisResolver,
        redisClientPool)
    {
        _redisResolver = redisResolver;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="db"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<ISelectDbRedisCommand> SelectDbAsync(int db, CancellationToken cancellationToken = default)
    {
        //不允许再次切换
        throw new NotImplementedException();
    }

    protected override async ValueTask DisposeCoreAsync(bool isDispose)
    {
        await _redisResolver.DisposeAsync();
    }
}