using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.RedisResolvers;

namespace RedisNaruto.RedisCommands.Pipe;

public class PipeRedisCommand : RedisCommand, IPipeRedisCommand
{
    private readonly PipeRedisResolver _pipeRedisResolver;

    internal PipeRedisCommand(IRedisClientPool redisClientPool, PipeRedisResolver pipeRedisResolver) : base(
        pipeRedisResolver,
        redisClientPool)
    {
        _pipeRedisResolver = pipeRedisResolver;
    }

    public async Task<object[]> EndPipeAsync(CancellationToken cancellationToken = default)
    {
        return await _pipeRedisResolver.PipeReadAsync(cancellationToken);
    }

    protected override void DisposeCore(bool isDispose)
    {
         _pipeRedisResolver.Dispose();
    }
}