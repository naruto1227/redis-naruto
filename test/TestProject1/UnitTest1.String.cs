using System.Reflection;
using System.Text;
using Xunit.Abstractions;

namespace TestProject1;

public partial class UnitTest1 : BaseUnit
{
    private ITestOutputHelper _testOutputHelper;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Test_StringSet()
    {
        var redisCommand = await GetRedisAsync();
        await redisCommand.SetAsync("testobj", new TestDto
        {
            Id = Guid.NewGuid(),
            Name = "123",
            Time = DateTime.Now
        });
        await redisCommand.SetAsync("testobj2", new TestDto
        {
            Id = Guid.NewGuid(),
            Name = "123222",
            Time = DateTime.Now
        });
    }

    [Fact]
    public async Task Test_StringSetBytes()
    {
        var redisCommand = await GetRedisAsync();
        using FileStream fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, "未命名.docx"),
            FileMode.Open,
            FileAccess.Read);
        using MemoryStream memoryStream = new MemoryStream();

        await fileStream.CopyToAsync(memoryStream);


        var res = await redisCommand.SetAsync("file", memoryStream.ToArray());

        //读取
        var newPath = Path.Combine(AppContext.BaseDirectory, "未命名2.docx");
        if (File.Exists(newPath))
        {
            File.Delete(newPath);
        }

        var fileBytes = await redisCommand.GetAsync("file");
        using FileStream file = new FileStream(newPath,
            FileMode.CreateNew,
            FileAccess.Write);
        await file.WriteAsync(fileBytes.ToBytes);
    }
    //
    [Fact]
    public async Task Test_StringMGet()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.MGetAsync(new[]
        {
            "testobj",
            "testobj2"
        });
        if (res != null)
        {
            foreach (var VARIABLE in res)
            {
                _testOutputHelper.WriteLine(VARIABLE.ToString());
            }
        }
    }

    [Fact]
    public async Task Test_StringMSet()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.MSetAsync(new Dictionary<string, string>()
            {
                {"testmset", "1"}, {"testmset2", "2"},
                {"testmset3", "3"},
                {"testmset4", "4"}
            }
        );
    }

    [Fact]
    public async Task Test_StringSetNx()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SetNxAsync("testnx2", "testnx"
        );
        _testOutputHelper.WriteLine(res.ToString());
        res = await redisCommand.SetNxAsync("testnx", "testnx"
        );
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_StringLength()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.StrLenAsync("testnx"
        );
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_StringSetRange()
    {
        var redisCommand = await GetRedisAsync();
        await redisCommand.SetAsync("Test_StringSetRange", "1233");
        var res = await redisCommand.SetRangeAsync("Test_StringSetRange", 1, "abc"
        );
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_StringGet_String()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.GetAsync("testobj");
    }

    [Fact]
    public async Task Test_StringGet_Class()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.GetAsync<TestDto>("testobj");
    }


    [Fact]
    public async Task Test_StringAppend()
    {
        var redisCommand = await GetRedisAsync();
        await redisCommand.AppendAsync("testobjappned", "1");
        await redisCommand.AppendAsync("testobjappned", "2");
    }

    [Fact]
    public async Task Test_StringDecrBy()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.DecrByAsync("StringDecrBy");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_StringIncrBy()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.IncrByAsync("StringDecrBy");
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_StringGetDel()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.GetDelAsync("GetDelAsync");
        _testOutputHelper.WriteLine(res.IsEmpty() ? "null" : res);
    }

    [Fact]
    public async Task Test_StringGetEx()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.GetExAsync("testobjstr", TimeSpan.FromMinutes(280));
        _testOutputHelper.WriteLine(res.IsEmpty() ? "null" : res);
    }

    [Fact]
    public async Task Test_StringGetRange()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.GetRangeAsync("testobjappned", 0, -1);
        _testOutputHelper.WriteLine(res.ToString());
    }

    [Fact]
    public async Task Test_StringLCS()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.LcsWithLenAsync("testobj2", "testobj");
        _testOutputHelper.WriteLine(res.ToString());
        var res2 = await redisCommand.LcsWithStringAsync("testobj2", "testobj");
        _testOutputHelper.WriteLine(res2.ToString());
    }
}