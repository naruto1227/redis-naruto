namespace RedisNaruto.RedisCommands.Pipe;

/// <summary>
/// 流水线
/// </summary>
public interface IPipeRedisCommand : IRedisCommand
{
    /// <summary>
    /// 结束流水线 开启读取所有的回复数据
    /// 注意：批量操作并不保证所有命令的结果都立即返回，可能会有一定的延迟。需要在你的应用程序中进行适当的等待和处理
    /// </summary>
    /// <returns></returns>
    Task<object[]> EndPipeAsync(CancellationToken cancellationToke=default);
}