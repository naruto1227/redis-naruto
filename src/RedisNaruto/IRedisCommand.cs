namespace RedisNaruto;

/// <summary>
/// 对外暴露的redis命令接口
/// </summary>
public interface IRedisCommand : IAsyncDisposable
{
    /// <summary>
    /// 存储字符串
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    Task<bool> StringSet(string key, Object value, TimeSpan timeSpan = default);

    /// <summary>
    /// 查询字符串
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<string> StringGet(string key);

    /// <summary>
    /// 查询字符串
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<TResult> StringGet<TResult>(string key);

    /// <summary>
    /// 批量获取
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<List<TResult>> StringMGet<TResult>(string[] key);

    /// <summary>
    /// 发布消息
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    /// <returns>收到消息的客户端数量</returns>
    Task<int> PublishAsync(string topic, string message);

    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <param name="topics"></param>
    /// <param name="reciveMessage"></param>
    /// <param name="cancellationToken"></param>
    Task SubscribeAsync(string[] topics,Func<string,string,Task> reciveMessage,CancellationToken cancellationToken=default);
}