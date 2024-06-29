using System.Collections.Concurrent;
using RedisNaruto.EventDatas;
using RedisNaruto.Internal.DiagnosticListeners;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.Internal.Interceptors;

internal sealed class ClientSideCachingInterceptor : IDisposable
{
    private ConcurrentDictionary<string, CacheItem> _entries;

    private IRedisCommand _redisCommand;

    /// <summary>
    ///  客户端缓存配置
    /// </summary>
    private ClientSideCachingOption _clientSideCachingOption;

    private PeriodicTimer _periodicTimer;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clientSideCachingOption"></param>
    /// <param name="redisCommand"></param>
    public ClientSideCachingInterceptor(ClientSideCachingOption clientSideCachingOption, IRedisCommand redisCommand)
    {
        _clientSideCachingOption = clientSideCachingOption;
        _redisCommand = redisCommand;
        _entries = new ConcurrentDictionary<string, CacheItem>(Environment.ProcessorCount,clientSideCachingOption.Capacity);
        _periodicTimer = new PeriodicTimer(clientSideCachingOption.ExpiredMessageInterval);
        //开启线程 执行定时清理过程
        new Thread(ClearTimeOut).Start();
    }

    /// <summary>
    /// 清理超时数据
    /// </summary>
    private async void ClearTimeOut()
    {
        while (await _periodicTimer.WaitForNextTickAsync())
        {
            if (_periodicTimer == null || _entries is not {Count: > 0})
            {
                return;
            }

            foreach (var item in _entries)
            {
                if (item.Value.ExpireTime < DateTime.Now)
                {
                    _entries.TryRemove(item);
                }
            }
        }
    }

    public void CommandBefore(object? sender, InterceptorCommandBeforeEventArgs e)
    {
        //
        if (e.Command.Key.IsNullOrWhiteSpace())
        {
            return;
        }

        // 判断是否需要缓存
        var value = Get(e.Command.Key);
        if (value == null)
        {
            return;
        }

        e.SetRetValue(value);
    }

    public void CommandAfter(object? sender, InterceptorCommandAfterEventArgs e)
    {
        if (e.Command.Key.IsNullOrWhiteSpace())
        {
            return;
        }

        if (_clientSideCachingOption.KeyPrefix.Any(a => a.StartsWith(e.Command.Key)))
        {
            Set(e.Command.Key, e.Value, _clientSideCachingOption.TimeOut);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private object Get(string key)
    {
        if (_entries.TryGetValue(key, out var res))
        {
            if (res.ExpireTime < DateTime.Now)
            {
                Remove(key);
                return default;
            }

            res.LastAccessed = DateTime.Now;
            return res.Value;
        }

        return default;
    }

    /// <summary>
    /// 存储
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    private void Set(string key, object value, TimeSpan expireTime)
    {
        _entries.AddOrUpdate(key, add => new CacheItem
        {
            Value = value,
            ExpireTime = DateTime.Now.AddSeconds(expireTime.TotalSeconds),
            LastAccessed = null
        }, (_, old) => new CacheItem
        {
            Value = value,
            ExpireTime = DateTime.Now.AddSeconds(expireTime.TotalSeconds),
            LastAccessed = null
        });
        RedisDiagnosticListener.ClientSideCachingUpdate(key,value);
    }

    /// <summary>
    /// 移除缓存
    /// </summary>
    /// <param name="key"></param>
    public void Remove(string key)
    {
        _entries.TryRemove(key, out _);
        RedisDiagnosticListener.ClientSideCachingRemove(key);
    }

    private void DisposeCore(bool isDispose)
    {
        if (isDispose)
        {
            _entries?.Clear();
            _entries = null;
            _periodicTimer?.Dispose();
            _periodicTimer = null;
            _redisCommand.UnRegisterInterceptorCommandBefore(this.CommandBefore);
            _redisCommand.UnRegisterInterceptorCommandAfter(this.CommandAfter);
        }
    }

    public void Dispose()
    {
        DisposeCore(true);
        GC.SuppressFinalize(this);
    }
}