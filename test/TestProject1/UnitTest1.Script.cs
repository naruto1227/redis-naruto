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
        var script = """
                    return redis.call("SET",KEYS[1],ARGV[1])
                    """;
        var res = await redisCommand.EvalAsync(script, new[]
        {
            "skey1"
        }, new[] {"131223"});
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_ScriptLoadAsync()
    {
        var redisCommand = await GetRedisAsync();
        var script = """
                    return redis.call("SET",KEYS[1],ARGV[1])
                    """;
        var res = await redisCommand.ScriptLoadAsync(script);
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_EvalShaAsync()
    {
        //获取redis客户端
        var redisCommand = await GetRedisAsync();
        //定义需要执行的脚本
        var script = """
          return redis.call("SET",KEYS[1],ARGV[1])
          """;
        //将脚本缓存到redis中，返回对应的id，用来执行EVALSHA
        var shaId = await redisCommand.ScriptLoadAsync(script);
        //使用id调用缓存的命令，所有客户端都可以通过此id调用
        var res = await redisCommand.EvalShaAsync(shaId, new[]
        {
            "skey1"
        }, new[] {"131223"});
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