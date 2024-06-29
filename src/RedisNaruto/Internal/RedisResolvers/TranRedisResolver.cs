using System.IO.Pipelines;
using RedisNaruto.EventDatas;
using RedisNaruto.Internal.DiagnosticListeners;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.RedisResolvers;

/// <summary>
/// 事务
/// </summary>
internal class TranRedisResolver : DefaultRedisResolver, IDisposable
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
        //ping
        await DoWhileAsync(async rc => await rc.PingAsync(), _redisClient);
    }

    /// <summary>
    /// 开启事务
    /// </summary>
    public async Task BeginTranAsync()
    {
        await InvokeAsync<RedisValue>(new Command(RedisCommandName.Multi, null));
        RedisDiagnosticListener.BeginTran(_redisClient.CurrentHost, _redisClient.CurrentPort);
    }

    public override async Task<T> InvokeAsync<T>(Command command)
    {
        _ = await _redisClient.ExecuteSampleAsync(command);
        if (command.Cmd == RedisCommandName.DisCard)
            RedisDiagnosticListener.DiscardTran(_redisClient.CurrentHost, _redisClient.CurrentPort);
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
        if (command.Cmd == RedisCommandName.Exec)
        {
            RedisDiagnosticListener.EndTran(_redisClient.CurrentHost, _redisClient.CurrentPort, resultList);
        }

        if (resultList == null)
        {
            yield break;
        }

        foreach (var item in resultList)
        {
            yield return item;
        }
    }

    public void Dispose()
    {
         _redisClient.Dispose();
        _redisClient = null;
    }
}