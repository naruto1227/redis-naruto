using RedisNaruto.Internal.Models;

namespace RedisNaruto.Internal.Interfaces;

/// <summary>
/// redis 客户端接口
/// </summary>
internal interface IRedisClient : IAsyncDisposable
{
    /// <summary>
    /// 连接信息
    /// </summary>
    ConnectionModel ConnectionModel { get; }

    /// <summary>
    /// 连接id
    /// </summary>
    Guid ConnectionId { get; }

    /// <summary>
    /// 当前连接的主机信息
    /// </summary>
    string CurrentHost { get; }

    /// <summary>
    /// 当前连接的端口信息
    /// </summary>
    int CurrentPort { get; }

    /// <summary>
    /// 执行命令接口
    /// </summary>
    /// <param name="command">命令参数</param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<TResult> ExecuteAsync<TResult>(Command command);

    /// <summary>
    /// 返回多结果集
    /// </summary>
    /// <param name="command"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    IAsyncEnumerable<TResult> ExecuteMoreResultAsync<TResult>(Command command);

    /// <summary>
    /// 读取
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<TResult> ReadMessageAsync<TResult>();

    /// <summary>
    /// 选择 db库
    /// </summary>
    /// <param name="db"></param>
    /// <returns></returns>
    Task<bool> SelectDb(int db);

    /// <summary>
    /// ping redis
    /// </summary>
    /// <returns></returns>
    Task<bool> PingAsync();

    /// <summary>
    /// 登陆授权
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="password">密码</param>
    /// <returns></returns>
    Task<bool> AuthAsync(string userName, string password);

    /// <summary>
    /// 退出
    /// </summary>
    /// <returns></returns>
    Task<bool> QuitAsync();

    /// <summary>
    /// 重置
    /// </summary>
    /// <returns></returns>
    Task ResetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 关闭
    /// </summary>
    void Close();
}