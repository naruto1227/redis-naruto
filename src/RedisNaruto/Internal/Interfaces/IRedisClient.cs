using RedisNaruto.Internal.Models;

namespace RedisNaruto.Internal.Interfaces;

/// <summary>
/// redis 客户端接口
/// </summary>
internal interface IRedisClient : IAsyncDisposable
{
    /// <summary>
    /// db访问库
    /// </summary>
    public int DB { get; }

    /// <summary>
    /// 用户
    /// </summary>
    public string UserName { get; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; }

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
    Task<List<TResult>> ExecuteMoreResultAsync<TResult>(Command command);

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
    /// 关闭
    /// </summary>
    void Close();
}