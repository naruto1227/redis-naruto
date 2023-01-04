using System.Reflection;
using System.Text;

namespace TestProject1;

public class TestDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public DateTime Time { get; set; }
}
public class UnitTest1
{
    
    public UnitTest1()
    {
       var res= Type.GetTypeCode(typeof(UnitTest1));
       var res1= Type.GetTypeCode(typeof(string));
       var res2= Type.GetTypeCode(typeof(int));
       var res3= Type.GetTypeCode(typeof(double));
       var res4= Type.GetTypeCode(typeof(DateTime));
       var res5= Type.GetTypeCode(typeof(DateOnly));
       var res6= Type.GetTypeCode(typeof(byte));
       var res7= Type.GetTypeCode(typeof(byte[]));
       object date = DateOnly.MaxValue;
       var dateOnly=((DateOnly)date).ToString("yyyy-MM-dd");;
       
    }
    [Fact]
    public async Task Test_StringSet()
    {
        var redisCommand = await RedisConnection.ConnectionAsync(new ConnectionModel
        {
            Connection = new string[]
            {
                "127.0.0.1:55000"
            },
            UserName = null,
            Password = "redispw"
        });
       await redisCommand.StringSet("testobj",new TestDto
       {
           Id = Guid.NewGuid(),
           Name = "123",
           Time = DateTime.Now
       });
       await redisCommand.StringSet("testobj2",new TestDto
       {
           Id = Guid.NewGuid(),
           Name = "123222",
           Time = DateTime.Now
       });
    }
    
    [Fact]
    public async Task Test_StringMGet()
    {
        var redisCommand = await RedisConnection.ConnectionAsync(new ConnectionModel
        {
            Connection = new string[]
            {
                "127.0.0.1:55000"
            },
            UserName = null,
            Password = "redispw"
        });
       var res= await redisCommand.StringMGet<string>(new []
        {
            "testobj",
            "testobj2"
        });
    }
    [Fact]
    public async Task Test_StringGet_String()
    {
        var redisCommand = await RedisConnection.ConnectionAsync(new ConnectionModel
        {
            Connection = new string[]
            {
                "127.0.0.1:55000"
            },
            UserName = null,
            Password = "redispw"
        });
        var res= await redisCommand.StringGet("testobj");
    }
    [Fact]
    public async Task Test_StringGet_Class()
    {
        var redisCommand = await RedisConnection.ConnectionAsync(new ConnectionModel
        {
            Connection = new string[]
            {
                "127.0.0.1:55000"
            },
            UserName = null,
            Password = "redispw",
        });
        var res= await redisCommand.StringGet<TestDto>("testobj");
    }
}