using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.RedisResolvers;

/// <summary>
/// 流水线
/// </summary>
internal class PipeRedisResolver : DefaultRedisResolver, IAsyncDisposable
{
    /// <summary>
    /// 流水线命令数
    /// </summary>
    private int _pipeCommand = 0;

    private IRedisClient _redisClient;

    public PipeRedisResolver(IRedisClientPool redisClientPool) : base(redisClientPool)
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
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override async Task<T> InvokeAsync<T>(Command command)
    {
        await _redisClient.ExecuteNoResultAsync(command);
        Interlocked.Increment(ref _pipeCommand);
        return default;
    }

    /// <summary>
    /// 流水线消息读取
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<object[]> PipeReadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = new object[_pipeCommand];
        for (var i = 0; i < _pipeCommand; i++)
        {
            result[i] = await _redisClient.ReadMessageAsync();
        }

        return result;
    }

    public override async Task<RedisValue> InvokeSimpleAsync(Command command)
    {
        await _redisClient.ExecuteNoResultAsync(command);
        Interlocked.Increment(ref _pipeCommand);
        return RedisValue.Null();
    }

    public async ValueTask DisposeAsync()
    {
        await _redisClientPool.ReturnAsync(_redisClient);
        _redisClient = null;
    }
}