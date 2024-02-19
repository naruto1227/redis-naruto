using RedisNaruto.EventDatas;
using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.RedisClients;
using RedisNaruto.Internal.RedisResolvers;
using RedisNaruto.Internal.Serialization;
using RedisNaruto.Models;

namespace RedisNaruto.RedisCommands;

public partial class RedisCommand : IRedisCommand
{
    private readonly IRedisClientPool _redisClientPool;

    /// <summary>
    /// 序列化
    /// </summary>
    private static readonly ISerializer Serializer = new Serializer();

    internal readonly IRedisResolver RedisResolver;

    internal RedisCommand(IRedisResolver redisResolver, IRedisClientPool redisClientPool)
    {
        RedisResolver = redisResolver;
        _redisClientPool = redisClientPool;
    }

    private RedisCommand(IRedisClientPool redisClientPool)
    {
        _redisClientPool = redisClientPool;
        RedisResolver = new DefaultRedisResolver(redisClientPool);
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="redisValue"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    private static async Task<TResult> DeserializeAsync<TResult>(byte[] redisValue) => await
        Serializer.DeserializeAsync<TResult>(redisValue);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal static async ValueTask<RedisCommand> BuilderAsync(ConnectionBuilder config)
    {
        var redisCommand = new RedisCommand(await RedisClientPool.BuildAsync(config));
        //连接配置
        return redisCommand;
    }

    
    #region Interceptor

    public void RegisterInterceptorCommandBefore(EventHandler<InterceptorCommandBeforeEventArgs> eventHandler)=>this.RedisResolver.RegisterInterceptorCommandBefore(eventHandler);
    public void RegisterInterceptorCommandAfter(EventHandler<InterceptorCommandAfterEventArgs> eventHandler)=>this.RedisResolver.RegisterInterceptorCommandAfter(eventHandler);

    /// <summary>
    /// 取消注册拦截器
    /// </summary>
    public void UnRegisterInterceptorCommandBefore(EventHandler<InterceptorCommandBeforeEventArgs> eventHandler)=>this.RedisResolver.UnRegisterInterceptorCommandBefore(eventHandler);
    /// <summary>
    /// 取消注册拦截器
    /// </summary>
    public void UnRegisterInterceptorCommandAfter(EventHandler<InterceptorCommandAfterEventArgs> eventHandler)=>this.RedisResolver.UnRegisterInterceptorCommandAfter(eventHandler);
    #endregion

    public async ValueTask DisposeAsync()
    {
        await DisposeCoreAsync(true);
        GC.SuppressFinalize(this);
    }

    protected virtual ValueTask DisposeCoreAsync(bool isDispose)
    {
        // if (isDispose && RedisClient != default)
        // {
        //     await RedisClientPool.ReturnAsync(RedisClient);
        //     RedisClient = null;
        // }
        return ValueTask.CompletedTask;
    }
}