using System.Numerics;
using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest_Lock : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest_Lock(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }


    [Fact]
    public async Task Test_Lock()
    {
        var redisCommand = await GetRedisAsync();
        var lockRes2 = await redisCommand.CreateLockAsync("lock_test", TimeSpan.FromMinutes(1),
            TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10));
        {
            if (lockRes2.IsAcquired)
            {
                _testOutputHelper.WriteLine("获取到锁");
            }
            else
            {
                _testOutputHelper.WriteLine("失败");
            }
        }
        await using (var lockRes = await redisCommand.CreateLockAsync("{lock}_test", TimeSpan.FromMinutes(1),
                         TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(3)))
        {
            if (lockRes.IsAcquired)
            {
                _testOutputHelper.WriteLine("获取到锁");
            }
            else
            {
                _testOutputHelper.WriteLine("失败");
            }
        }
        await using (var lockRes = await redisCommand.CreateLockAsync("lock_test", TimeSpan.FromMinutes(1),
                         TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(3)))
        {
            if (lockRes.IsAcquired)
            {
                _testOutputHelper.WriteLine("获取到锁");
            }
            else
            {
                _testOutputHelper.WriteLine("失败");
            }
        }
    }
}