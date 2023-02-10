namespace RedisNaruto.RedisCommands.Pipe;

/// <summary>
/// 流水线
/// </summary>
public interface IPipeRedisCommand : IRedisCommand
{
    /// <summary>
    /// 结束流水线 开启读取所有的回复数据
    /// </summary>
    /// <returns></returns>
    Task<object[]> EndPipeAsync(CancellationToken cancellationToke=default);
}