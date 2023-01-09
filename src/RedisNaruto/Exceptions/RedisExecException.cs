namespace RedisNaruto.Exceptions;

/// <summary>
/// redis执行异常
/// </summary>
public class RedisExecException : ApplicationException
{
    public RedisExecException(string error):base(error)
    {
        
    }
}