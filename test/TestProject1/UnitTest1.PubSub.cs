using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest1_PubSub : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest1_PubSub(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    [Fact]
    public async Task Test_Publish()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.PublishAsync("testredisnarutopub", "testtt");
    }

    [Fact]
    public async Task Test_Sub()
    {
        var redisCommand = await GetRedisAsync();
        await redisCommand.SubscribeAsync(new[] {"testredisnarutopub", "te", "23"}, (topic, message) =>
        {
            _testOutputHelper.WriteLine($"topic={topic},message={message}");
            return Task.CompletedTask;
        });
    }

    [Fact]
    public async Task Test_UnSub()
    {
        var redisCommand = await GetRedisAsync();
        await redisCommand.SubscribeAsync(new[] {"testredisnarutopub", "te", "23"}, async (topic, message) =>
        {
            _testOutputHelper.WriteLine($"topic={topic},message={message}");
            await redisCommand.UnSubscribeAsync(new[] {"testredisnarutopub"});
        });
    }
}