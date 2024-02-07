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
        var redisCommand = await GetRedisAsync();

        for (int i = 0; i < 100; i++)
        {
            //开启管道
            await using var pipe = await redisCommand.BeginPipeAsync();
//发送命令 其中结果res1 res2 res3 res4 都是空值，因为结果需要使用EndPipeAsync读取
            var res1 = await pipe.IncrByAsync("x");
            var res2 = await pipe.IncrByAsync("x");
            var res3 = await pipe.IncrByAsync("x");
            var res4 = await pipe.IncrByAsync("x");
//结束 一次性读取所有的消息，从管道中
            var res = await pipe.EndPipeAsync();
//遍利所有的结果，res数组中的结果值顺序按照命令写入顺序进行返回
            foreach (var item in res)
            {
                _testOutputHelper.WriteLine(JsonConvert.SerializeObject(item.ToString()));
            }
        }
    }
}