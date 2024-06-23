using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.RedisResolvers;

/// <summary>
/// 切换数据库
/// </summary>
internal class SelectDbRedisResolver : DefaultRedisResolver, IDisposable
{
    private IRedisClient _redisClient;

    public SelectDbRedisResolver(IRedisClientPool redisClientPool) : base(redisClientPool)
    {
    }

    /// <summary>
    /// 初始化客户端
    /// </summary>
    public async Task InitClientAsync(int db)
    {
        _redisClient = await _redisClientPool.RentAsync();
        //SelectDb
        await DoWhileAsync(async rc => await rc.SelectDb(db), _redisClient);
    }

    public override async Task<T> InvokeAsync<T>(Command command)
    {
        return await _redisClient.ExecuteAsync<T>(command);
    }

    public override async Task<RedisValue> InvokeSimpleAsync(Command command)
    {
        return await _redisClient.ExecuteSampleAsync(command);
    }

    public void Dispose()
    {
         _redisClient.Dispose();
        _redisClient = null;
    }
}