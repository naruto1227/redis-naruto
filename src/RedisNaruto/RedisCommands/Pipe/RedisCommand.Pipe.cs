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
        var client = await GetRedisClient(cancellationToken);
        client.BeginPipe();
        var pipeRedisCommand = new PipeRedisCommand(RedisClientPool);
        pipeRedisCommand.ChangeRedisClient(client);
        return pipeRedisCommand;
    }
}