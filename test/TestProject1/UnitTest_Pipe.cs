using Newtonsoft.Json;
using RedisNaruto.Models;
using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest_Pipe : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest_Pipe(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }


    [Fact]
    public void TEst()
    {
        ReadOnlySpan<byte> ReadOnlySpan = new byte[]
        {
            (byte) '\r',
            (byte) '1',
            (byte) '\n',
        };
        ReadOnlySpan<byte> ReadOnlySpan2 = new byte[]
        {
            (byte) '\r',
            (byte) '1',
        };
        var s = ReadOnlySpan.IndexOf(ReadOnlySpan2);
    }

    /// <summary>
    /// 
    /// </summary>
    [Fact]
    public async Task UsePipe()
    {
        for (int i = 0; i < 10; i++)
        {
            var redisCommand = await GetRedisAsync();
            await using var pipe = await redisCommand.BeginPipeAsync();
            // var res1 = await pipe.SetAsync("pipe1", "pipe1");
            // var res2 = await pipe.SetAsync("pipe2", "pipe2");
            // var res3 = await pipe.SetAsync("pipe3", "pipe3");
            var res4 = await pipe.MGetAsync(new[]
            {
                "pipe1",
                "pipe2",
                "pipe3"
            });
            await pipe.GetAsync("pipe1");
            //等待 redis所有结果处理完成，不然的话有可能会造成消息读取不完整
            await Task.Delay(50);
            //结束
            var res = await pipe.EndPipeAsync();
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(JsonConvert.SerializeObject(item));
            }
        }
       
    }
}