using RedisNaruto.Internal;
using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;

namespace RedisNaruto;

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
        await using var client = await _redisClientPool.RentAsync();
        var result =
            await client.ExecuteAsync<int>(new Command(RedisCommandName.Pub, new object[]
            {
                topic,
                message
            }));
        return result;
    }


    /// <summary>
    /// 订阅客户端
    /// </summary>
    private IRedisClient _subscribeClient;

    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <param name="topics"></param>
    /// <param name="reciveMessage"></param>
    /// <param name="cancellationToken"></param>
    public async Task SubscribeAsync(string[] topics, Func<string, string, Task> reciveMessage,
        CancellationToken cancellationToken = default)
    {
        // await using var client = await _redisClientPool.RentAsync();
        _subscribeClient = await _redisClientPool.RentAsync();
        //订阅消息
        _ = await _subscribeClient.ExecuteAsync<object>(new Command(RedisCommandName.Sub, topics));
        while (!cancellationToken.IsCancellationRequested)
        {
            //读取消息
            var message = await _subscribeClient.ReadMessageAsync<object>();
            if (message is List<object> {Count: 3} result && result[0].Equals("message"))
            {
                var topic = result[1];
                var body = result[2];
                await reciveMessage(topic.ToString(), body.ToString());
            }
            //第一次开始订阅 消息会按照普通字符串的形式回复
            else if (message.Equals("message"))
            {
                var topic = await _subscribeClient.ReadMessageAsync<string>();
                var body = await _subscribeClient.ReadMessageAsync<string>();
                await reciveMessage(topic, body);
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
        if (_subscribeClient == default)
        {
            throw new InvalidOperationException("需要先订阅才能取消订阅");
        }

        _ = await _subscribeClient.ExecuteAsync<object>(new Command(RedisCommandName.UnSub, topics));
    }
}