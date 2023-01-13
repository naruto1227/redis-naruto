namespace RedisNaruto.Exceptions;

/// <summary>
/// 哨兵异常
/// </summary>
public class RedisSentinelException : ApplicationException
{
    public RedisSentinelException(string error) : base(error)
    {
    }
}