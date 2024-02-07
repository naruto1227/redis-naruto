namespace TestProject1;

[TestCaseOrderer(
    ordererTypeName: "TestProject1.PriorityOrderer",
    ordererAssemblyName: "TestProject1")]
public class BaseUnit
{
    protected virtual async Task<IRedisCommand> GetRedisAsync()
    {
        var redisCommand = await RedisConnection.CreateAsync("127.0.0.1:55000,username=default,password=redispw,resp3=true");
        return redisCommand;
    }
}