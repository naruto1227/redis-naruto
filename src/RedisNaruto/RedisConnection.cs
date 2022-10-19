namespace RedisNaruto;

public class RedisConnection
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static async Task<IRedisCommand> ConnectionAsync(ConnectionModel config)
    {
        return await RedisCommand.ConnectionAsync(config);
    }
}