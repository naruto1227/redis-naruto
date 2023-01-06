using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;

namespace RedisNaruto;

public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 发布消息
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    /// <returns>收到消息的客户端数量</returns>
    public async Task<int> PublishAsync(string topic, string message)
    {
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
    /// 订阅消息
    /// </summary>
    /// <param name="topics"></param>
    /// <param name="reciveMessage"></param>
    /// <param name="cancellationToken"></param>
    public async Task SubscribeAsync(string[] topics, Func<string, string, Task> reciveMessage,
        CancellationToken cancellationToken = default)
    {
        await using var client = await _redisClientPool.RentAsync();
        //订阅消息
        await client.ExecuteAsync<object>(new Command(RedisCommandName.Sub, topics));
        while (!cancellationToken.IsCancellationRequested)
        {
            //读取消息
            var message = await client.ReadMessageAsync<object>();
            if (message is List<object> {Count: 3} result && result[0].Equals("message"))
            {
                var topic = result[1];
                var body = result[2];
                await reciveMessage(topic.ToString(), body.ToString());
            }
            //第一次开始订阅 消息会按照普通字符串的形式回复
            else if (message.Equals("message"))
            {
                var topic = await client.ReadMessageAsync<string>();
                var body = await client.ReadMessageAsync<string>();
                await reciveMessage(topic, body);
            }
        }
    }
}