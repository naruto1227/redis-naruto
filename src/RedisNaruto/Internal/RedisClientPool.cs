using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Microsoft.Extensions.ObjectPool;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;

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

    // /// <summary>
    // /// 等待创建事件 300 ms
    // /// </summary>
    // private readonly int WaitTime = 300;

    /// <summary>
    /// 主机端口信息
    /// </summary>
    private readonly List<HostPort> _hostPorts = new();

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
            //池内 空闲数减少
            Interlocked.Decrement(ref _freeCount);
            return redisClient;
        }
        //随机获取一个主机信息
        var currentHostPort = _hostPorts.MinBy(a => Guid.NewGuid());
        redisClient = await RedisClient.ConnectionAsync(currentHostPort, _userName, _password, _defaultDbIndex,
            async (client) => { await this.ReturnAsync(client); });
        return redisClient;
    }

    /// <summary>
    /// 归还
    /// </summary>
    /// <param name="redisClient"></param>
    /// <returns></returns>
    public ValueTask ReturnAsync([NotNull] IRedisClient redisClient)
    {
        //递增当前池中的数量 验证 是否小于等于 最大的数量
        if (Interlocked.Increment(ref _freeCount) <= _maxCount)
        {
            //入队
            _freeClients.Enqueue(redisClient);
            return new ValueTask();
        }

        //多余的就释放资源
        redisClient.Close();
        Interlocked.Decrement(ref _freeCount);
        return new ValueTask();
    }


    #region Dispose

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool isDispose)
    {
        if (isDispose)
        {
        }
    }

    #endregion
}