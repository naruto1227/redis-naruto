using RedisNaruto.Internal.RedisResolvers;
using RedisNaruto.RedisCommands.Pipe;
using RedisNaruto.RedisCommands.Transaction;

namespace RedisNaruto.RedisCommands;

public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 开启流水线
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IPipeRedisCommand> BeginPipeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var pipeRedisResolver = new PipeRedisResolver(_redisClientPool);
        await pipeRedisResolver.InitClientAsync();
        //todo 执行auth select
        return new PipeRedisCommand(_redisClientPool, pipeRedisResolver);
    }
}