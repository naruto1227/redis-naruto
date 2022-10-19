using System.Diagnostics.CodeAnalysis;
using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;

namespace RedisNaruto;

public class RedisCommand : IRedisCommand
{
    private IRedisClient _redisClient;

    private RedisCommand(IRedisClient redisClient)
    {
        _redisClient = redisClient;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal static async Task<RedisCommand> ConnectionAsync(ConnectionModel config)
    {
        var redisClient = await RedisClient.ConnectionAsync(config.Connection, config.UserName, config.Password);
        var redisCommand = new RedisCommand(redisClient);
        //连接配置
        return redisCommand;
    }

    public ValueTask DisposeAsync()
    {
        return _redisClient.DisposeAsync();
    }

    public async Task<bool> StringSet(string key, object value, TimeSpan timeSpan = default)
    {
        var argv = timeSpan == default
            ? new object[]
            {
                key,
                value
            }
            : new object[]
            {
                key,
                value,
                "PX",
                timeSpan == default ? 0 : timeSpan.TotalMilliseconds
            };
        return await _redisClient.ExecuteAsync<bool>(new Command(RedisCommandName.Set, argv));
    }

    public async Task<string> StringGet(string key)
    {
        return await StringGet<string>(key);
    }

    /// <summary>
    /// 查询字符串
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<TResult> StringGet<TResult>(string key)
    {
        return await _redisClient.ExecuteAsync<TResult>(new Command(RedisCommandName.Get, new object[]{key}));
    }
    public async Task<List<TResult>> StringMGet<TResult>(string[] key)
    {
        return await _redisClient.ExecuteMoreResultAsync<TResult>(new Command(RedisCommandName.MGET, key));
    }
}