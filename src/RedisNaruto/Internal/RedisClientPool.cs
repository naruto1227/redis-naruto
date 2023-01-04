using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Microsoft.Extensions.ObjectPool;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;

namespace RedisNaruto.Internal;

internal class RedisClientPool : IRedisClientPool
{
    /// <summary>
    ///空闲的客户端
    /// </summary>
    private readonly ConcurrentQueue<IRedisClient> _freeClients = new();

    /// <summary>
    /// 当前使用数
    /// </summary>
    private int _currentUseCount = 0;

    /// <summary>
    /// 最大创建数
    /// </summary>
    private int _maxCount;

    /// <summary>
    /// 等待创建事件 300 ms
    /// </summary>
    private readonly int _waitTime = 300;

    /// <summary>
    /// 主机端口信息
    /// </summary>
    private List<HostPort> _hostPorts;

    /// <summary>
    /// 
    /// </summary>
    public RedisClientPool(string[] connections,int maxCount)
    {
        _maxCount = maxCount;
        //todo 思考如何实现将空闲的客户端释放  
    }

    public async Task<IRedisClient> RentAsync()
    {
        //从队列中获取
        if (_freeClients.TryDequeue(out var redisClient))
        {
            Interlocked.Increment(ref _currentUseCount);
            return redisClient;
        }
        //判断当前使用数量和最大数量比较
        if (_currentUseCount>=_maxCount)
        {
            throw new InvalidOperationException("连接池已满");
        }
        //创建连接
        Interlocked.Increment(ref _currentUseCount);
        //随机伙计一个主机信息
       var currentHostPort= _hostPorts.MinBy(a => Guid.NewGuid());
       var tcpClient = new TcpClient();
       
        redisClient = new RedisClient(tcpClient);
        
    }

    public async Task ReturnAsync([NotNull] IRedisClient redisClient)
    {
        if (Interlocked.Decrement(ref _currentUseCount)<0)
        {
            Interlocked.Increment(ref _currentUseCount);
        }
       
        //多余的就释放资源
        await redisClient.DisposeAsync();
    }


    #region Dispose

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool isDispose)
    {
        if (isDispose)
        {
        }
    }

    #endregion
}