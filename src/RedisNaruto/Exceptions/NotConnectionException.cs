namespace RedisNaruto.Exceptions;

public class NotConnectionException : ApplicationException
{
    public NotConnectionException() : base("连接地址无效，请检查")
    {
    }
}