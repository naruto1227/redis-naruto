using System.IO.Pipelines;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Message.MessageParses;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.RedisResolvers;

/// <summary>
/// 
/// </summary>
internal class DefaultRedisResolver : IRedisResolver
{
    /// <summary>
    /// 连接
    /// </summary>
    protected readonly IRedisClientPool _redisClientPool;


    private static readonly IMessageParse MessageParse = new MessageParse();

    public DefaultRedisResolver(IRedisClientPool redisClientPool)
    {
        _redisClientPool = redisClientPool;
    }

    /// <summary>
    /// 执行
    /// </summary>
    public virtual async Task<T> InvokeAsync<T>(Command command)
    {
        PipeReader pipeReader = default;
        await using (var redisClient = await _redisClientPool.RentAsync())
        {
            pipeReader = await redisClient.ExecuteAsync(command);
        }

        await using var dispose = new AsyncDisposeAction(() => pipeReader.CompleteAsync().AsTask());

        var res = await MessageParse.ParseMessageAsync(pipeReader);
        if (res is T redisValue)
        {
            return redisValue;
        }

        return default(T);
    }

    public virtual async IAsyncEnumerable<object> InvokeMoreResultAsync(Command command)
    {
        //todo 使用迭代器
        var resultList = await InvokeAsync<List<object>>(command);
        if (resultList == null)
        {
            yield break;
        }

        foreach (var item in resultList)
        {
            yield return item;
        }
    }
}