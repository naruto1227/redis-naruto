using System.IO.Pipelines;

namespace RedisNaruto.Internal.Message.MessageParses;

public interface IMessageParse
{
    /// <summary>
    /// 转换消息
    /// </summary>
    /// <param name="pipeReader"></param>
    /// <returns></returns>
    Task<object> ParseMessageAsync(PipeReader pipeReader);
}