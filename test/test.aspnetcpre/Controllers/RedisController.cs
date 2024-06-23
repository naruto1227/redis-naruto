using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RedisNaruto;
using StackExchange.Redis;

namespace test.aspnetcpre.Controllers;

[ApiController]
[Route("[controller]")]
public class RedisController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private static readonly string SumStr = JsonSerializer.Serialize(Summaries);

    private readonly ILogger<RedisController> _logger;

    public RedisController(ILogger<RedisController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }


    [HttpPost("naruto")]
    public async Task<object> TestRedisNaruto([FromServices] IRedisCommand redisCommand)
    {
        await redisCommand.SetAsync("weather", SumStr);
        string res = await redisCommand.GetAsync("weather");
        return new SuccessModel
        {
            Code = 200,
            Data = res
        };
    }

    [HttpPost("tran")]
    public async Task<object> TestRedisNaruto_Tran([FromServices] IRedisCommand redisCommand)
    { 
        using var command = await redisCommand.MultiAsync();
        string res = await command.GetAsync("weather");
       var ss2=await command.ExecAsync();
        return new SuccessModel
        {
            Code = 200,
            Data = res
        };
    }

    [HttpPost("stack_exchange")]
    public async Task<object> TestStackExchangeRedis([FromServices] ConnectionMultiplexer connectionMultiplexer)
    {
        await connectionMultiplexer.GetDatabase().StringSetAsync("weather2", SumStr);
        var res = await connectionMultiplexer.GetDatabase().StringGetAsync("weather2");
        return new SuccessModel
        {
            Code = 200,
            Data = res.ToString()
        };
    }
    
    [HttpGet("clientCache")]
    public async Task<object> clientCache([FromServices] IRedisCommand redisCommand)
    {
       
        string res =  await redisCommand.GetAsync("test");
        return new SuccessModel
        {
            Code = 200,
            Data = res
        };
    }

    [HttpGet("clientCacheSet")]
    public async Task<object> clientCacheSet([FromServices] IRedisCommand redisCommand)
    {
        await redisCommand.SetAsync("test", "123123");
        return new SuccessModel
        {
            Code = 200,
            Data = 1
        };
    }
}