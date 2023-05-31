using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Utils;

namespace RedisNaruto.Internal.RedisClients;

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
    /// 最小创建数
    /// </summary>
    private readonly int _minCount;

    /// <summary>
    /// 最大创建数
    /// </summary>
    private readonly int _maxCount;

    /// <summary>
    /// 
    /// </summary>
    private readonly IRedisClientFactory _redisClientFactory;

    /// <summary>
    /// 创建的总客户端总数
    /// </summary>
    private int _totalClientCount;

    /// <summary>
    /// 闲置时间 闲置多久进行释放 ，当池中的数量低于 最小不做处理
    /// </summary>
    private readonly int _idle;


    /// <summary>
    /// 
    /// </summary>
    public RedisClientPool(ConnectionBuilder connectionBuilder)
    {
        _totalClientCount = 0;
        _minCount = connectionBuilder.PoolCount;
        _maxCount = connectionBuilder.MaxPoolCount;
        _idle = connectionBuilder.Idle;
        _redisClientFactory = new RedisClientFactory(connectionBuilder);
        new Thread(InitAsync).Start();
        new Thread(TrimPoolAsync).Start();
    }

    /// <summary>
    /// 获取一个可用的客户端
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<IRedisClient> RentAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            //从队列中获取
            if (_freeClients.TryDequeue(out var redisClient))
            {
                //池内 空闲数减少
                Interlocked.Decrement(ref _freeCount);
                // 重置连接
                await redisClient.ResetAsync(cancellationToken);
                return redisClient;
            }

            //判断 是否达到最大的池数量 达到了最大的话 就等待
            if (Interlocked.Increment(ref _totalClientCount) <= _maxCount)
                return await _redisClientFactory.GetAsync(
                    async (client) => { await this.ReturnAsync(client); },
                    cancellationToken);
            //还原
            Interlocked.Decrement(ref _totalClientCount);
            //todo 这里的等待时间 通过配置
            //todo 是否要一直等待 还是等待指定
            await Task.Delay(500, cancellationToken);
        }

        throw new OperationCanceledException();
    }

    /// <summary>
    /// 初始化所有的连接
    /// </summary>
    private async void InitAsync()
    {
        for (var i = 0; i < _minCount; i++)
        {
            var redisClient = await _redisClientFactory.GetAsync(async (client) => { await this.ReturnAsync(client); });
            //递增
            Interlocked.Increment(ref _totalClientCount);
            //递增
            Interlocked.Increment(ref _freeCount);
            _freeClients.Enqueue(redisClient);
        }
    }

    /// <summary>
    /// 归还
    /// </summary>
    /// <param name="redisClient"></param>
    /// <returns></returns>
    private ValueTask ReturnAsync([NotNull] IRedisClient redisClient)
    {
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
        if (_idle == 0)
        {
            return;
        }

        //todo 时间配置
        using var periodicTimer = new PeriodicTimer(TimeSpan.FromMinutes(3));
        while (await periodicTimer.WaitForNextTickAsync())
        {
            //只有超过 最小数才裁剪
            var excessCount = _totalClientCount - _minCount;
            if (excessCount <= 0)
            {
                continue;
            }

            //获取当前时间戳
            var timestamp = TimeUtil.GetTimestamp();
            if (_freeClients.IsEmpty) continue;
            for (var i = 0; i < excessCount; i++)
            {
                //获取最开始的数据 并且 判断更新时间 是否为合理的时间
                if (_freeClients.TryPeek(out var redisClient) && timestamp - redisClient.LastDataTime >= _idle)
                {
                    // 读取
                    if (!_freeClients.TryDequeue(out var redisClient2)) continue;
                    //判断是否为同一对象
                    if (!RuntimeHelpers.Equals(redisClient2, redisClient))
                    {
                        //还原
                        _freeClients.Enqueue(redisClient2);
                        continue;
                    }

                    redisClient2.Close();
                    Interlocked.Decrement(ref _freeCount);
                    Interlocked.Decrement(ref _totalClientCount);
                }
                else
                {
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
    }

    #endregion
}