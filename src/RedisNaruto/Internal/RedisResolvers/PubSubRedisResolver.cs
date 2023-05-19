using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Message.MessageParses;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.RedisResolvers;

/// <summary>
/// 发布订阅
/// </summary>
internal class PubSubRedisResolver : DefaultRedisResolver
{
    private static readonly IMessageParse MessageParse = new MessageParse();

    private IRedisClient _redisClient;

    public PubSubRedisResolver(IRedisClientPool redisClientPool) : base(redisClientPool)
    {
    }

    /// <summary>
    /// 初始化客户端
    /// </summary>
    public async Task InitClientAsync()
    {
        _redisClient = await _redisClientPool.RentAsync();
    }

    public override async Task<T> InvokeAsync<T>(Command command)
    {
        var pipeReader = await _redisClient.ExecuteAsync(command);

        // await using var dispose = new AsyncDisposeAction(() => pipeReader.CompleteAsync().AsTask());

        var res = await MessageParse.ParseMessageAsync(pipeReader);
        if (res is T redisValue)
        {
            return redisValue;
        }

        return default(T);
    }
    public override Task<RedisValue> InvokeSimpleAsync(Command command)
    {
        //todo 
        return base.InvokeSimpleAsync(command);
    }
    /// <summary>
    /// 流水线消息读取
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T> ReadMessageAsync<T>(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var pipeReader = await _redisClient.ReadMessageAsync();
        await using var dispose = new AsyncDisposeAction(() => pipeReader.CompleteAsync().AsTask());
        var res = await MessageParse.ParseMessageAsync(pipeReader);
        if (res is T redisValue)
        {
            return redisValue;
        }

        return default(T);
    }

    public async Task ReturnAsync()
    {
        await _redisClientPool.ReturnAsync(_redisClient);
        _redisClient = null;
    }
}