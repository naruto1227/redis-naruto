using RedisNaruto.EventDatas;
using Xunit.Abstractions;

namespace TestProject1;

[TestCaseOrderer(
    ordererTypeName: "TestProject1.PriorityOrderer",
    ordererAssemblyName: "TestProject1")]
public class BaseUnit
{
    public ITestOutputHelper _testOutputHelper;

    public BaseUnit(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    protected virtual async Task<IRedisCommand> GetRedisAsync()
    {
        var redisCommand = await RedisConnection.CreateAsync("127.0.0.1:55000,username=default,password=redispw,resp3=true");
        redisCommand.RegisterInterceptorCommandBefore(CommandBefore);
        redisCommand.RegisterInterceptorCommandBefore(CommandBefore2);
        redisCommand.RegisterInterceptorCommandAfter(CommandAfter);
        return redisCommand;
    }

    private void CommandBefore(object? sender, InterceptorCommandBeforeEventArgs e)
    {
        _testOutputHelper.WriteLine("before");
    }
    private void CommandBefore2(object? sender, InterceptorCommandBeforeEventArgs e)
    {
        _testOutputHelper.WriteLine("before2");
    }
    private void CommandAfter(object? sender, InterceptorCommandAfterEventArgs e)
    {
        _testOutputHelper.WriteLine("after");
    }
}