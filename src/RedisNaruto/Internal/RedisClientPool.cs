using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Microsoft.Extensions.ObjectPool;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Internal.Sentinels;

namespace RedisNaruto.Internal;

internal sealed class RedisClientPool : IRedisClientPool
{
    /// <summary>
    ///空闲的客户端
    /// </summary>
    private readonly ConcurrentQueue<IRedisClient> _freeClients = new();

    /// <summary>
    /// 空闲数
    /// </summary>
    private int _freeCount = 0;

    /// <summary>
    /// 最大创建数
    /// </summary>
    private readonly int _maxCount;

    /// <summary>
    /// 
    /// </summary>
    private readonly IRedisClientFactory _redisClientFactory;

    private readonly SemaphoreSlim _semaphoreSlim;

    /// <summary>
    /// 
    /// </summary>
    public RedisClientPool(ConnectionModel connectionModel)
    {
        _maxCount = connectionModel.ConnectionPoolCount;
        _redisClientFactory = new RedisClientFactory(connectionModel);
        _semaphoreSlim = new(_maxCount, _maxCount);
        //
        new Thread(InitAsync).Start();
        new Thread(TrimPoolAsync).Start();
    }

    public async Task<IRedisClient> RentAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _semaphoreSlim.WaitAsync(cancellationToken);
        //从队列中获取
        if (_freeClients.TryDequeue(out var redisClient))
        {
            //池内 空闲数减少
            Interlocked.Decrement(ref _freeCount);
            //重置连接
            await redisClient.ResetAsync(cancellationToken);
        }
        else
        {
            redisClient = await _redisClientFactory.GetAsync(async (client) => { await this.ReturnAsync(client); },
                cancellationToken);
        }

        return redisClient;
    }

    /// <summary>
    /// 初始化所有的连接
    /// </summary>
    private async void InitAsync()
    {
        for (var i = 0; i < _maxCount; i++)
        {
            var redisClient = await _redisClientFactory.GetAsync(async (client) => { await this.ReturnAsync(client); });
            _freeClients.Enqueue(redisClient);
        }
    }

    /// <summary>
    /// 归还
    /// </summary>
    /// <param name="redisClient"></param>
    /// <returns></returns>
    public ValueTask ReturnAsync([NotNull] IRedisClient redisClient)
    {
        _semaphoreSlim.Release();
        Interlocked.Increment(ref _freeCount);
        //入队
        _freeClients.Enqueue(redisClient);
        return ValueTask.CompletedTask;
    }


    /// <summary>
    /// 释放多余的连接
    /// </summary>
    private async void TrimPoolAsync()
    {
        using var periodicTimer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await periodicTimer.WaitForNextTickAsync())
        {
            var excessCount = _freeClients.Count - _maxCount;
            while (excessCount > 0)
            {
                if (_freeClients.TryDequeue(out var redisClient))
                {
                    redisClient.Close();
                    Interlocked.Decrement(ref _freeCount);
                    excessCount--;
                }
                else
                {
                    // 如果队列为空，可以中断循环
                    break;
                }
            }
        }
    }

    #region Dispose

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool isDispose)
    {
        if (!isDispose) return;
        while (_freeClients.TryDequeue(out var redisClient))
        {
            redisClient.Close();
        }

        _semaphoreSlim.Dispose();
    }

    #endregion
}