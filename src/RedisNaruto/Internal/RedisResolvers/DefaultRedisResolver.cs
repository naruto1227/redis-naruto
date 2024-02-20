using System.IO.Pipelines;
using System.Net.Sockets;
using RedisNaruto.EventDatas;
using RedisNaruto.Exceptions;
using RedisNaruto.Internal.DiagnosticListeners;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.RedisResolvers;

/// <summary>
/// 
/// </summary>
internal class DefaultRedisResolver : IRedisResolver
{
    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<InterceptorCommandBeforeEventArgs> CommandBefore;

    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<InterceptorCommandAfterEventArgs> CommandAfter;

    /// <summary>
    /// 连接
    /// </summary>
    protected readonly IRedisClientPool _redisClientPool;


    public DefaultRedisResolver(IRedisClientPool redisClientPool)
    {
        _redisClientPool = redisClientPool;
    }

    /// <summary>
    /// 执行
    /// </summary>
    public virtual async Task<T> InvokeAsync<T>(Command command)
    {
        await using (var redisClient = await _redisClientPool.RentAsync())
        {
            var eventArgs = new InterceptorCommandBeforeEventArgs(command);
            CommandBefore?.Invoke(null, eventArgs);
            //判断是否为缓存
            if (eventArgs.IsCache && eventArgs.Value is T result)
            {
                return result;
            }

            try
            {
                var res = await DoWhileAsync(async rc => await rc.ExecuteAsync<T>(command), redisClient);
                CommandAfter?.Invoke(null, new InterceptorCommandAfterEventArgs(command, res));
                return res;
            }
            catch (Exception e)
            {
                CommandAfter?.Invoke(null, new InterceptorCommandAfterEventArgs(command, null, e));
                throw;
            }
        }
    }

    /// <summary>
    /// 执行
    /// </summary>
    public virtual async Task<RedisValue> InvokeSimpleAsync(Command command)
    {
        await using (var redisClient = await _redisClientPool.RentAsync())
        {
            var eventArgs = new InterceptorCommandBeforeEventArgs(command);
            CommandBefore?.Invoke(null, eventArgs);
            //判断是否为缓存
            if (eventArgs.IsCache && eventArgs.Value is RedisValue result)// todo 校验转换
            {
                return result;
            }

            try
            {
                var res = await DoWhileAsync(async rc => await rc.ExecuteSampleAsync(command), redisClient);
                CommandAfter?.Invoke(null, new InterceptorCommandAfterEventArgs(command, res));
                return res;
            }
            catch (Exception e)
            {
                CommandAfter?.Invoke(null, new InterceptorCommandAfterEventArgs(command, null, e));
                throw;
            }
        }
    }

    public virtual async IAsyncEnumerable<object> InvokeMoreResultAsync(Command command)
    {
        //todo 使用迭代器
        var resultList = await InvokeAsync<List<object>>(command);
        if (resultList == null)
        {
            yield break;
        }

        foreach (var item in resultList)
        {
            yield return item;
        }
    }


    #region Interceptor

    public void RegisterInterceptorCommandBefore(EventHandler<InterceptorCommandBeforeEventArgs> eventHandler)
    {
        CommandBefore += eventHandler;
    }

    public void RegisterInterceptorCommandAfter(EventHandler<InterceptorCommandAfterEventArgs> eventHandler)
    {
        CommandAfter += eventHandler;
    }

    /// <summary>
    /// 取消注册拦截器
    /// </summary>
    public void UnRegisterInterceptorCommandBefore(EventHandler<InterceptorCommandBeforeEventArgs> eventHandler)
    {
        CommandBefore -= eventHandler;
    }

    /// <summary>
    /// 取消注册拦截器
    /// </summary>
    public void UnRegisterInterceptorCommandAfter(EventHandler<InterceptorCommandAfterEventArgs> eventHandler)
    {
        CommandAfter -= eventHandler;
    }

    #endregion

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="func"></param>
    /// <param name="redisClient"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected virtual async Task<T> DoWhileAsync<T>(Func<IRedisClient, Task<T>> func,
        IRedisClient redisClient)
    {
        while (true)
        {
            try
            {
                //执行命令
                return await func(redisClient);
            }
            catch (Exception e)
            {
                new SelectRedisClientErrorEventData(redisClient.CurrentHost, redisClient.CurrentPort, e)
                    .SelectRedisClientError();
                //判断异常的类型 是否为网络相关的
                if (e is not IOException or SocketException)
                {
                    new SelectRedisClientErrorEventData(redisClient.CurrentHost, redisClient.CurrentPort, e)
                        .SelectRedisClientError();
                    throw;
                }

                //如果是网络问题的话 重置连接，循环判断，直到没有连接可用 抛出异常
                while (true)
                {
                    //重置连接
                    try
                    {
                        await redisClient.ResetSocketAsync();
                        break;
                    }
                    catch (Exception exception)
                    {
                        new SelectRedisClientErrorEventData(redisClient.CurrentHost, redisClient.CurrentPort, exception)
                            .SelectRedisClientError();
                        //当没有连接的时候 抛出
                        if (exception is NotConnectionException)
                        {
                            throw;
                        }
                    }
                }
            }
        }
    }
}