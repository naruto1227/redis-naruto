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
        if (config.MaxPoolCount < config.PoolCount)
        {
            throw new ArgumentException($"{nameof(config.MaxPoolCount)}不能小于最小池数量");
        }

        return await RedisCommands.RedisCommand.BuilderAsync(config);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static async Task<IRedisCommand> CreateAsync(string config)
    {
        return await CreateAsync(new ConnectionBuilder().Parse(config));
    }
}