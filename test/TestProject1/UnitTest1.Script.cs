using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest1_Script : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest1_Script(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Test_EvalAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.EvalAsync("return ARGV[1]", null, new[] {"131223"});
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ScriptLoadAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ScriptLoadAsync("return ARGV[1]");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_EvalShaAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.EvalShaAsync("098e0f0d1448c0a81dafe820f66d460eb09263da", null,
            new[] {"hello xiaozhang"});
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ScriptExistsAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ScriptExistsAsync(new[] {"098e0f0d1448c0a81dafe820f66d460eb09263da"});
        foreach (var VARIABLE in res)
        {
            _testOutputHelper.WriteLine(VARIABLE.ToString());
        }
    }
}