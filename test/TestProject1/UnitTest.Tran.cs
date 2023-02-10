using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest_Tran : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest_Tran(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Multi()
    {
        var redisCommand = await GetRedisAsync();
        await using var tran = await redisCommand.MultiAsync();
       var res1= await tran.SetAsync("transtr11", "1");
       var res2= await tran.SetAsync("transtr21", 2);
       var res3= await tran.GetAsync("transtr");
       var res4= await tran.MGetAsync<string>(new []
        {
            "transtr","transtr2"
        });
        var res = await tran.ExecAsync();
    }
}