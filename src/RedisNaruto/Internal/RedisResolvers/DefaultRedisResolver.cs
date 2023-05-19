using System.IO.Pipelines;
using RedisNaruto.Internal.Interfaces;
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
    

    public DefaultRedisResolver(IRedisClientPool redisClientPool)
    {
        _redisClientPool = redisClientPool;
    }

    /// <summary>
    /// 执行
    /// </summary>
    public virtual async Task<T> InvokeAsync<T>(Command command)
    {
        await using (var redisClient = await _redisClientPool.RentAsync())
        {
            return await redisClient.ExecuteAsync<T>(command);
        }
    }

    /// <summary>
    /// 执行
    /// </summary>
    public virtual async Task<RedisValue> InvokeSimpleAsync(Command command)
    {
        await using (var redisClient = await _redisClientPool.RentAsync())
        {
            return await redisClient.ExecuteSampleAsync(command);
        }
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