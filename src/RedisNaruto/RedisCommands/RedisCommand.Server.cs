using RedisNaruto.Internal;
using RedisNaruto.Internal.Models;

namespace RedisNaruto.RedisCommands;

/// <summary>
/// 服务
/// </summary>
public partial class RedisCommand : IRedisCommand
{
    /// <summary>
    /// 返回选中数据库的key数量
    /// </summary>
    /// <returns></returns>
    public async Task<long> DbSizeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await using var client = await GetRedisClient(cancellationToken);
        return await client.ExecuteAsync<long>(new Command(RedisCommandName.DbSize, default));
    }
}