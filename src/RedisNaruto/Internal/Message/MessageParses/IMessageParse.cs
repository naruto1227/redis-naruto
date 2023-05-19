// using System.IO.Pipelines;
// using RedisNaruto.Models;
//
// namespace RedisNaruto.Internal.Message.MessageParses;
//
// public interface IMessageParse
// {
//     /// <summary>
//     /// 转换消息
//     /// </summary>
//     /// <param name="pipeReader"></param>
//     /// <returns></returns>
//     Task<object> ParseMessageAsync(PipeReader pipeReader);
//
//     /// <summary>
//     /// 转换消息
//     /// </summary>
//     /// <param name="pipeReader"></param>
//     /// <returns></returns>
//     Task<RedisValue> ParseSimpleMessageAsync(PipeReader pipeReader);
// }