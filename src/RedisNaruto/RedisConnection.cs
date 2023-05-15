namespace RedisNaruto;

/// <summary>
/// 连接对象
/// </summary>
public static class RedisConnection
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static async Task<IRedisCommand> CreateAsync(ConnectionBuilder config)
    {
        return await RedisCommands.RedisCommand.BuilderAsync(config);
    }
}