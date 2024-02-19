using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using RedisNaruto.RedisCommands;
using Xunit.Abstractions;

namespace TestProject1;

public partial class UnitTest1 : BaseUnit
{

    public UnitTest1(ITestOutputHelper testOutputHelper):base(testOutputHelper)
    {
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
    public async Task Test_StringSet2()
    {
        var redisCommand = await GetRedisAsync();
        List<TestDto> li = Enumerable.Repeat(new TestDto
        {
            Id = Guid.NewGuid(),
            Name = "123",
            Time = DateTime.Now
        }, 1_000).ToList();
        await redisCommand.SetAsync("testBigobj", li);
    }

    [Fact]
    public async Task Test_StringSetRN()
    {
        var redisCommand = await GetRedisAsync();
        await redisCommand.SetAsync("strrn", new byte[]
        {
            (byte) '1',
            (byte) '\r',
            (byte) '\n'
        });
        var res = await redisCommand.GetAsync("strrn");
        // Assert.Equal(res.Length, 3);
        _testOutputHelper.WriteLine(res.ToBytes.Length.ToString());
    }

    [Fact]
    public async Task Test_StringSetBytes()
    {
        var redisCommand = await GetRedisAsync();
        //  using FileStream fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, "未命名.docx"),
        //      FileMode.Open,
        //      FileAccess.Read);
        //  using MemoryStream memoryStream = new MemoryStream();
        //  
        //  await fileStream.CopyToAsync(memoryStream);
        //
        //
        // var ss= memoryStream.ToArray();
        //  var res = await redisCommand.SetAsync("file", memoryStream.ToArray());

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
        var res = await redisCommand.SetNxAsync("testnx2", "testnx",TimeSpan.FromMinutes(1)
        );
        _testOutputHelper.WriteLine(res.ToString());
        res = await redisCommand.SetNxAsync("testnx2", "testnx"
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
        var res = await redisCommand.GetAsync("sr");
        var res2 = await redisCommand.GetAsync("sr");
        var res3 = await redisCommand.GetAsync("sr");
        if (!res.IsEmpty())
        {
            _testOutputHelper.WriteLine(res);
        }
    }

    [Fact]
    public async Task Test_StringGet_Class()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.GetAsync<TestDto>("testobj");
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(res));
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

    [Fact]
    public async Task Test_StringSet_Null()
    {
        var redisCommand = await GetRedisAsync();
        var res = await redisCommand.SetAsync("Test_StringSet_Null", null);
        var res2 = await redisCommand.GetAsync("Test_StringSet_Null");
    }

    [Fact]
    public void TestBytes()
    {
        double number = 11222211112222222222;
        var b1 = Encoding.Default.GetBytes(number.ToString());
        var b2 = BitConverter.GetBytes(number);
        var ss = BitConverter.ToDouble(b2);
    }


    [Fact]
    public async Task Test()
    {
        Memory<byte> newMemory = default;
        using (var memoryOwner = MemoryPool<byte>.Shared.Rent(10))
        {
            memoryOwner.Memory.Span[0] = (byte) '1';
            memoryOwner.Memory.Span[1] = (byte) '2';
            memoryOwner.Memory.Span[2] = (byte) '3';
            //创建一个新的 slice
            newMemory = memoryOwner.Memory[..3];
            newMemory.Span[0] = (byte) 'a';
        }

        byte[] newBytes = default;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            await memoryStream.WriteAsync(newMemory);
            newBytes = memoryStream.ToArray();
        }

        using (var memoryOwner = MemoryPool<byte>.Shared.Rent(10))
        {
            memoryOwner.Memory.Span[0] = (byte) '1';
            memoryOwner.Memory.Span[1] = (byte) '2';
            memoryOwner.Memory.Span[2] = (byte) '3';
            //创建一个新的 slice
            newMemory = memoryOwner.Memory[..3];
            newMemory.Span[0] = (byte) 'a';
        }
    }

    [Fact]
    public async Task Test2()
    {
        Memory<byte> newMemory = default;
        using (var memoryOwner = MemoryPool<byte>.Shared.Rent(10))
        {
            memoryOwner.Memory.Span[0] = (byte) '1';
            memoryOwner.Memory.Span[1] = (byte) '2';
            memoryOwner.Memory.Span[2] = (byte) '3';
            //创建一个新的 slice
            newMemory = memoryOwner.Memory[..3];
            newMemory.Span[0] = (byte) 'a';
        }

        byte[] newBytes = default;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            await memoryStream.WriteAsync(newMemory);
            newBytes = memoryStream.ToArray();
        }

        using (var memoryOwner = MemoryPool<byte>.Shared.Rent(10))
        {
            memoryOwner.Memory.Span[0] = (byte) '1';
            memoryOwner.Memory.Span[1] = (byte) '2';
            memoryOwner.Memory.Span[2] = (byte) '3';
            //创建一个新的 slice
            newMemory = memoryOwner.Memory[..3];
            newMemory.Span[0] = (byte) 'a';
        }
    }

    [Fact]
    public async Task Test_StringSet_Large()
    {
        var redisCommand = await GetRedisAsync();
        //初始化400m的数据
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < 40000000; i++)
        {
            stringBuilder.Append("test" + i);
        }

        _testOutputHelper.WriteLine(Encoding.Default.GetBytes(stringBuilder.ToString()).Length.ToString());
        Stopwatch stopwatch = Stopwatch.StartNew();
        await redisCommand.SetAsync("setbig", stringBuilder.ToString());
        stopwatch.Stop();
        _testOutputHelper.WriteLine(stopwatch.ElapsedMilliseconds.ToString());
    }

    /// <summary>
    /// 
    /// </summary>
    [Fact]
    public void TestSpan()
    {
        //定义下标
        var index = 0;
        using var memoryOwner =
            MemoryPool<string>.Shared.Rent(2);
        memoryOwner.Memory.Span[++index] = "1";
        var s = Encoding.Default.GetBytes("123");

        Span<byte> span = s;
        span[0] = (byte) '5';
        ReadOnlySpan<byte> readOnlySpan = span;
        span[1] = (byte) '5';
        TcpClient tcpClient = new TcpClient();
    }

    /// <summary>
    /// 
    /// </summary>
    [Fact]
    public async Task Testcr()
    {
        ConcurrentDictionary<IRedisCommand, long> dictionary = new();
        var redisCommand1= await RedisConnection.CreateAsync(new ConnectionBuilder()
        {
            Connection = new string[]
            {
                "127.0.0.1:55000"
            },
            DataBase = 0,
            UserName = "default",
            Password = "redispw",
            PoolCount = 1
        });
        dictionary.TryAdd(redisCommand1, 1);
        var redisCommand2 = await RedisConnection.CreateAsync(new ConnectionBuilder()
        {
            Connection = new string[]
            {
                "127.0.0.1:55000"
            },
            DataBase = 0,
            UserName = "default",
            Password = "redispw",
            PoolCount = 1
        });
        dictionary.TryAdd(redisCommand2, 2);
        var s = dictionary.Where(a => a.Value == 2).FirstOrDefault();
        _testOutputHelper.WriteLine(RuntimeHelpers.Equals(redisCommand2, s.Key).ToString());
        _testOutputHelper.WriteLine(RuntimeHelpers.Equals(redisCommand1, s.Key).ToString());
        // dictionary.Remove(s.Key, out var ss);
    }
}