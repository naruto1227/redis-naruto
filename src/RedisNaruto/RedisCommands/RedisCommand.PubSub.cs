using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Internal.RedisResolvers;
using RedisNaruto.Models;
using RedisNaruto.Utils;

namespace RedisNaruto.RedisCommands;

public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 发布消息
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>收到消息的客户端数量</returns>
    public async Task<int> PublishAsync(string topic, string message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result =
            await RedisResolver.InvokeSimpleAsync(new Command(RedisCommandName.Pub, new object[]
            {
                topic,
                message
            }));
        return result;
    }


    /// <summary>
    /// 订阅客户端
    /// </summary>
    private PubSubRedisResolver _pubSubRedisResolver;

    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <param name="topics"></param>
    /// <param name="reciveMessage"></param>
    /// <param name="cancellationToken"></param>
    public async Task SubscribeAsync(string[] topics, Func<string, string, Task> reciveMessage,
        CancellationToken cancellationToken = default)
    {
        _pubSubRedisResolver = new PubSubRedisResolver(_redisClientPool);
        await _pubSubRedisResolver.InitClientAsync();
        //订阅消息
        _ = await _pubSubRedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.Sub, topics))
            .ToRedisValueListAsync();
        using (_pubSubRedisResolver)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                //读取消息
                var message = await _pubSubRedisResolver.ReadMessageAsync<List<object>>(cancellationToken);
                if (message is {Count: 3} && message[0] is RedisValue rv && rv == "message")
                {
                    var topic = message[1];
                    var body = message[2];
                    await reciveMessage(topic.ToString(), body.ToString());
                }
                //第一次开始订阅 消息会按照普通字符串的形式回复
                else if (message[0] is RedisValue redisValue && redisValue == "message")
                {
                    var topic = await _pubSubRedisResolver.ReadMessageAsync<RedisValue>(cancellationToken);
                    var body = await _pubSubRedisResolver.ReadMessageAsync<RedisValue>(cancellationToken);
                    await reciveMessage(topic, body);
                }
            }
        }
    }

    /// <summary>
    /// 取消订阅消息
    /// </summary>
    /// <param name="topics"></param>
    /// <param name="cancellationToken"></param>
    public async Task UnSubscribeAsync(string[] topics,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_pubSubRedisResolver == default)
        {
            throw new InvalidOperationException("需要先订阅才能取消订阅");
        }

        _ = await _pubSubRedisResolver.InvokeMoreResultAsync(new Command(RedisCommandName.UnSub, topics))
            .ToRedisValueListAsync();
    }
}