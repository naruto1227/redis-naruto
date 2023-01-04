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
    private List<HostPort> _hostPorts = new();

    /// <summary>
    /// 用户名
    /// </summary>
    private readonly string _userName;

    /// <summary>
    /// 密码
    /// </summary>
    private readonly string _password;

    /// <summary>
    /// 默认的地址
    /// </summary>
    private readonly int _defaultDbIndex;

    /// <summary>
    /// 
    /// </summary>
    public RedisClientPool(string[] connections, string userName, string password, int defaultDbIndex, int maxCount)
    {
        this._userName = userName;
        this._password = password;
        _defaultDbIndex = defaultDbIndex;
        _maxCount = maxCount;
        foreach (var item in connections)
        {
            if (item == null || item.Length <= 0)
            {
                throw new ArgumentNullException("连接地址异常");
            }

            var hostString = item.Split(":");
            if (!int.TryParse(hostString[1], out var port))
            {
                port = 6349;
            }

            _hostPorts.Add(new HostPort(hostString[0], port));
        }
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
        if (_currentUseCount >= _maxCount)
        {
            throw new InvalidOperationException("连接池已满");
        }

        //创建连接
        Interlocked.Increment(ref _currentUseCount);
        //随机获取一个主机信息
        var currentHostPort = _hostPorts.MinBy(a => Guid.NewGuid());
        redisClient = await RedisClient.ConnectionAsync(currentHostPort, _userName, _password, _defaultDbIndex,
            async (client) => { await this.ReturnAsync(client); });
        return redisClient;
    }

    public ValueTask ReturnAsync([NotNull] IRedisClient redisClient)
    {
        if (Interlocked.Decrement(ref _currentUseCount) < 0)
        {
            Interlocked.Increment(ref _currentUseCount);
            _freeClients.Enqueue(redisClient);
        }

        //多余的就释放资源
        redisClient.Close();
        return new ValueTask();
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