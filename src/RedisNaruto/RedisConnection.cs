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
    public static async Task<IRedisCommand> ConnectionAsync(ConnectionModel config)
    {
        return await RedisCommands.RedisCommand.ConnectionAsync(config);
    }
}