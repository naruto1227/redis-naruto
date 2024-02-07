using RedisNaruto.EventDatas;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.RedisResolvers;

/// <summary>
/// redis解析器
/// </summary>
internal interface IRedisResolver
{
    /// <summary>
    /// 执行
    /// </summary>
    Task<T> InvokeAsync<T>(Command command);

    /// <summary>
    /// 执行
    /// </summary>
    Task<RedisValue> InvokeSimpleAsync(Command command);

    /// <summary>
    /// 执行
    /// </summary>
    IAsyncEnumerable<object> InvokeMoreResultAsync(Command command);

    /// <summary>
    /// 注册拦截器
    /// </summary>
    public void RegisterInterceptorCommandBefore(EventHandler<InterceptorCommandBeforeEventArgs> eventHandler);
    /// <summary>
    /// 注册拦截器
    /// </summary>
    public void RegisterInterceptorCommandAfter(EventHandler<InterceptorCommandAfterEventArgs> eventHandler);
    /// <summary>
    /// 取消注册拦截器
    /// </summary>
    public void UnRegisterInterceptorCommandBefore(EventHandler<InterceptorCommandBeforeEventArgs> eventHandler);
    /// <summary>
    /// 取消注册拦截器
    /// </summary>
    public void UnRegisterInterceptorCommandAfter(EventHandler<InterceptorCommandAfterEventArgs> eventHandler);
}