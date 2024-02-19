using System.Text.Json;
using RedisNaruto.Models;
using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest_Tran : BaseUnit
{

    public UnitTest_Tran(ITestOutputHelper testOutputHelper):base(testOutputHelper)
    {
    }

    /// <summary>
    /// 事务提交
    /// </summary>
    [Fact]
    public async Task TranExec()
    {
        var redisCommand = await GetRedisAsync();
        await redisCommand.SetAsync("transtr11", "1");
        await using var tran = await redisCommand.MultiAsync();
        var res1 = await tran.SetAsync("transtr11", "1");
        var res2 = await tran.SetAsync("transtr21", 2);
        var res3 = await tran.GetAsync("transtr");
        var res4 = await tran.MGetAsync(new[]
        {
            "transtr11", "transtr21"
        });
        var res = await tran.ExecAsync();
        foreach (var item in res)
        {
            _testOutputHelper.WriteLine(item.ToString());
        }
    }

    /// <summary>
    /// 事务取消
    /// </summary>
    [Fact]
    public async Task TranDiscard()
    {
        var redisCommand = await GetRedisAsync();
        await using var tran = await redisCommand.MultiAsync();
        await tran.SetAsync("TranDiscard11", "1");
        await tran.SetAsync("TranDiscard21", 2);
        await tran.GetAsync("transtr");
        await tran.MGetAsync(new[]
        {
            "transtr", "transtr2"
        });
        await tran.DiscardAsync();
    }

    /// <summary>
    /// 事务取消
    /// </summary>
    [Fact]
    public async Task Watch()
    {
        var redisCommand = await GetRedisAsync();
        //监视key
        await redisCommand.WatchAsync(new[] {"TranDiscard11"});
        //变动值
        // await redisCommand.SetAsync("TranDiscard11", "2");
        //开启事务
        await using var tran = await redisCommand.MultiAsync();
        var res1 = await tran.SetAsync("TranDiscard11", "2");
        var res2 = await tran.SetAsync("TranDiscard21", 2);
        var res3 = await tran.GetAsync("transtr");
        var res4 = await tran.MGetAsync(new[]
        {
            "transtr", "transtr2"
        });
        //事务提交 由于TranDiscard11在监听之后已经被修改了，所以事务提交失败 返回null
        var res = await tran.ExecAsync();
    }

    /// <summary>
    /// 事务提交
    /// </summary>
    [Fact]
    public async Task TranExecError()
    {
        var redisCommand = await GetRedisAsync();
        await using var tran = await redisCommand.MultiAsync();
        var res1 = await tran.SetAsync("transtr11", "1");
        var res2 = await tran.HSetAsync("transtr11", new Dictionary<string, object>()
        {
            {"1", "2"}
        });
        var res = await tran.ExecAsync();
        foreach (var item in res)
        {
            _testOutputHelper.WriteLine(item.ToString());
        }
    }
}