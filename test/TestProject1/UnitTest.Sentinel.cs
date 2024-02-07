using System.Net;
using System.Net.Sockets;
using System.Text;
using RedisNaruto.Internal;
using RedisNaruto.Internal.Serialization;
using RedisNaruto.Utils;
using Xunit.Abstractions;

namespace TestProject1;

/// <summary>
/// 哨兵
/// </summary>
public class UnitTest_Sentinel : BaseUnit
{

    public UnitTest_Sentinel(ITestOutputHelper testOutputHelper):base(testOutputHelper)
    {
    }

    [Fact]
    public async Task Con()
    {
        var redisCommand = await GetRedisAsync();
       await redisCommand.StrLenAsync("1");
    }
}