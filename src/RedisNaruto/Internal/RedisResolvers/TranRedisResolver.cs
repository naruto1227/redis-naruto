using System.IO.Pipelines;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Message.MessageParses;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.RedisResolvers;

/// <summary>
/// 事务
/// </summary>
internal class TranRedisResolver : DefaultRedisResolver, IAsyncDisposable
{
    private static readonly IMessageParse MessageParse = new MessageParse();

    private IRedisClient _redisClient;

    public TranRedisResolver(IRedisClientPool redisClientPool) : base(redisClientPool)
    {
    }

    /// <summary>
    /// 初始化客户端
    /// </summary>
    public async Task InitClientAsync()
    {
        _redisClient = await _redisClientPool.RentAsync();
    }

    /// <summary>
    /// 开启事务
    /// </summary>
    public async Task BeginTranAsync()
    {
        await InvokeAsync<RedisValue>(new Command(RedisCommandName.Multi, null));
    }

    public override async Task<T> InvokeAsync<T>(Command command)
    {
        var pipeReader = await _redisClient.ExecuteAsync(command);
        await using var dispose = new AsyncDisposeAction(() => pipeReader.CompleteAsync().AsTask());
        var res = await MessageParse.ParseMessageAsync(pipeReader);
        if (res is T redisValue)
        {
            return redisValue;
        }

        return default(T);
    }

    public async ValueTask DisposeAsync()
    {
        await _redisClientPool.ReturnAsync(_redisClient);
        _redisClient = null;
    }
}