namespace TestProject1;

public class BaseUnit
{
    protected virtual async Task<IRedisCommand> GetRedisAsync()
    {
        var redisCommand = await RedisConnection.CreateAsync(new ConnectionBuilder()
        {
            Connection = new string[]
            {
                "127.0.0.1:55000"
            },
            DataBase = 0,
            UserName = "default",
            Password = "redispw",
            PoolCount = 1
        });
        return redisCommand;
    }
}