namespace RedisNaruto.Exceptions;

public class NotConnectionException : ApplicationException
{
    public NotConnectionException() : base("无可用连接")
    {
    }
}