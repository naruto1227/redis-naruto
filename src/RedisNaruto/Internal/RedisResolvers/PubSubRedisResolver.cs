using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.RedisResolvers;

/// <summary>
/// 发布订阅
/// </summary>
internal class PubSubRedisResolver : DefaultRedisResolver
{
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
        //ping
        await DoWhileAsync(async rc => await rc.PingAsync(), _redisClient);
    }

    public override async Task<T> InvokeAsync<T>(Command command)
    {
        return await _redisClient.ExecuteAsync<T>(command);
    }

    public override async Task<RedisValue> InvokeSimpleAsync(Command command)
    {
        return await _redisClient.ExecuteSampleAsync(command);
    }

    /// <summary>
    /// 流水线消息读取
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T> ReadMessageAsync<T>(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var res = await _redisClient.ReadMessageAsync();
        if (res is T redisValue)
        {
            return redisValue;
        }

        return default(T);
    }

    public async Task ReturnAsync()
    {
        await _redisClient.DisposeAsync();
        _redisClient = null;
    }
}