namespace RedisNaruto.Internal.Serialization;

/// <summary>
/// 序列化
/// </summary>
internal interface ISerializer
{
    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    Task<byte[]> SerializeAsync(object source);

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<TResult> DeserializeAsync<TResult>(byte[] source);
}