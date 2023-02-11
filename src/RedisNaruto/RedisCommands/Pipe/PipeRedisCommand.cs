using RedisNaruto.Internal.Interfaces;

namespace RedisNaruto.RedisCommands.Pipe;

public class PipeRedisCommand : RedisCommand, IPipeRedisCommand
{
    internal PipeRedisCommand(IRedisClientPool redisClientPool)
    {
        RedisClientPool = redisClientPool;
    }

    public async Task<object[]> EndPipeAsync(CancellationToken cancellationToken = default)
    {
        var client = await GetRedisClient(cancellationToken);
        var result = await client.PipeReadMessageAsync();
        //结束流水线
        client.EndPipe();
        return result;
    }

    protected override async ValueTask DisposeCoreAsync(bool isDispose)
    {
        await base.DisposeCoreAsync(isDispose);
        await RedisClientPool.ReturnAsync(RedisClient);
    }
}