using System.IO.Pipelines;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.RedisResolvers;

/// <summary>
/// 事务
/// </summary>
internal class TranRedisResolver : DefaultRedisResolver, IAsyncDisposable
{
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
        _ = await _redisClient.ExecuteSampleAsync(command);
        return default;
    }

    public override async Task<RedisValue> InvokeSimpleAsync(Command command)
    {
        _ = await _redisClient.ExecuteSampleAsync(command);
        return default;
    }

    public override async IAsyncEnumerable<object> InvokeMoreResultAsync(Command command)
    {
        //todo 使用迭代器
        var resultList = await _redisClient.ExecuteAsync<List<object>>(command);
        if (resultList == null)
        {
            yield break;
        }

        foreach (var item in resultList)
        {
            yield return item;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _redisClientPool.ReturnAsync(_redisClient);
        _redisClient = null;
    }
}