using RedisNaruto.EventDatas;
using RedisNaruto.Internal.DiagnosticListeners;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.RedisResolvers;

/// <summary>
/// 流水线
/// </summary>
internal class PipeRedisResolver : DefaultRedisResolver, IDisposable
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
        //ping
        await DoWhileAsync(async rc => await rc.PingAsync(), _redisClient);
        RedisDiagnosticListener.BeginPipe(_redisClient.CurrentHost,_redisClient.CurrentPort);
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
        RedisDiagnosticListener.EndPipe(_redisClient.CurrentHost,_redisClient.CurrentPort,result);
        return result;
    }

    public override async Task<RedisValue> InvokeSimpleAsync(Command command)
    {
        await _redisClient.ExecuteNoResultAsync(command);
        Interlocked.Increment(ref _pipeCommand);
        return RedisValue.Null();
    }

    public void Dispose()
    {
         _redisClient.Dispose();
        _redisClient = null;
    }
}