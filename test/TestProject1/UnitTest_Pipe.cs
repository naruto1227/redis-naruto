using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest_Pipe : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest_Pipe(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// 
    /// </summary>
    [Fact]
    public async Task UsePipe()
    {
        var redisCommand = await GetRedisAsync();
        await using (var pipe = await redisCommand.BeginPipeAsync())
        {
            var res1 = await pipe.SetAsync("pipe1", "pipe1");
            var res2 = await pipe.SetAsync("pipe2", "pipe2");
            var res3 = await pipe.SetAsync("pipe3", "pipe3");
            var res4 = await pipe.MGetAsync<string>(new[]
            {
                "pipe1",
                "pipe2",
                "pipe3"
            });
            //结束
            var res = await pipe.EndPipeAsync();
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }
        }
    }
}