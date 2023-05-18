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
}