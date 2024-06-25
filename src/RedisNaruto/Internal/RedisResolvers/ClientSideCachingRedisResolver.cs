using RedisNaruto.Internal.Interfaces;
using RedisNaruto.Internal.Models;
using RedisNaruto.Models;

namespace RedisNaruto.Internal.RedisResolvers;

/// <summary>
/// 客户端缓存服务
/// </summary>
internal class ClientSideCachingRedisResolver: PubSubRedisResolver
{
    private readonly ClientSideCachingOption _clientSideCachingOption;
    public ClientSideCachingRedisResolver(IRedisClientPool redisClientPool,ClientSideCachingOption clientSideCachingOption) : base(redisClientPool)
    {
        _clientSideCachingOption = clientSideCachingOption;
    }

    public override async Task InitClientAsync()
    {
        await base.InitClientAsync();
        //获取客户端id
        await RedisClient.InitClientIdAsync();
    }
    
    #region 客户端缓存命令

    public virtual async Task BCastAsync()
    {
        if (this._clientSideCachingOption.KeyPrefix?.Length <= 0)
        {
            return;
        }

        //
        List<object> argv = new() //todo 临时写法
        {
            "TRACKING",
            "on",
            "REDIRECT",
            RedisClient.ClientId,
            "BCAST",
        };

        foreach (var se in this._clientSideCachingOption!.KeyPrefix!)
        {
            argv.Add("PREFIX");
            argv.Add(se);
        }

        _ = await InvokeSimpleAsync(new Command(RedisCommandName.Client, argv.ToArray()));
    }
    
    #endregion
}