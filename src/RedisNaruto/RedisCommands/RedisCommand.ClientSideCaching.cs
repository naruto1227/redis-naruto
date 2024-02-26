using RedisNaruto.Models;

namespace RedisNaruto.RedisCommands;

public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 启用客户端缓存
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void UseClientSideCaching(ClientSideCachingOption option)
    {
        //todo 开启新的线程专门用于接收消息过期的key 然后清除缓存操作
    }
}