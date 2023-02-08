using System.Net.Sockets;
using System.Text;
using Xunit.Abstractions;

namespace TestProject1;

public class UnitTest1_Client : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest1_Client(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Test_ClientId()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.ClientIdAsync();
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_Ping()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.PingAsync();
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_DbSizeAsync()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.DbSizeAsync();
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task testTcp()
    {
        TcpClient tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("127.0.0.1", 55002);
        await tcpClient.GetStream().WriteAsync(Encoding.Default.GetBytes("ping"));
        var s = new byte[1024];
        await tcpClient.GetStream().ReadAsync(s);
        TcpClient tcpClient2 = new TcpClient();
        await tcpClient2.ConnectAsync("127.0.0.1", 55000);
    }
}