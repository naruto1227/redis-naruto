using System.Buffers;
using System.Buffers.Binary;
using System.Text;
using BenchmarkDotNet.Attributes;
using FreeRedis;
using NewLife.Caching;
using RedisNaruto;
using StackExchange.Redis;
using RedisClient = FreeRedis.RedisClient;

namespace BenchmarkTest;

[MemoryDiagnoser()]
public class StringSetTest : BaseTest
{
    /// <summary>
    /// 
    /// </summary>
    [Benchmark]
    public void Test_Set_NewLifeRedis()
    {
        for (int i = 0; i < N; i++)
            FullRedis.Set("sr", TestMsg);
    }

    /// <summary>
    /// 
    /// </summary>
    [Benchmark]
    public async Task Test_Set_StackExchange()
    {
        for (int i = 0; i < N; i++)
            await RedisConn.StringSetAsync("sr", TestMsg);
    }

    /// <summary>
    /// 
    /// </summary>
    [Benchmark]
    public async Task Test_Set_FreeRedis()
    {
        for (int i = 0; i < N; i++)
            await _redisClient.SetAsync("sr", TestMsg);
    }

    [Benchmark]
    public async Task Test_Set_RedisNaruto()
    {
        for (int i = 0; i < N; i++)
        {
            await RedisCommand.SetAsync("sr", TestMsg);
        }
    }



    // //[Benchmark]
    // public void T2()
    // {
    //     var reusableBuffer = ArrayPool<byte>.Shared.Rent(TestMsg.Length * 3);
    //
    //     var s = Encoding.UTF8.GetBytes(TestMsg, 0, TestMsg.Length, reusableBuffer, 0);
    //     ArrayPool<byte>.Shared.Return(reusableBuffer);
    //
    //
    //     // var reusableBuffer = MemoryPool<byte>.Shared.Rent();
    //     // // reusableBuffer.Memory;
    //     // Encoding.UTF8.GetBytes(TestMsg, 0, TestMsg.Length, reusableBuffer, 0);
    //     // reusableBuffer.Dispose();
    // }
    //
    // //  [Benchmark]
    // public void T3()
    // {
    //     var reusableBuffer = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetByteCount(TestMsg));
    //
    //     var s = Encoding.UTF8.GetBytes(TestMsg, 0, TestMsg.Length, reusableBuffer, 0);
    //     ArrayPool<byte>.Shared.Return(reusableBuffer);
    //
    //
    //     // var reusableBuffer = MemoryPool<byte>.Shared.Rent();
    //     // // reusableBuffer.Memory;
    //     // Encoding.UTF8.GetBytes(TestMsg, 0, TestMsg.Length, reusableBuffer, 0);
    //     // reusableBuffer.Dispose();
    // }
    
    
}