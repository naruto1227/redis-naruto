using System.Runtime.CompilerServices;
using RedisNaruto.RedisCommands;
using RedisNaruto.RedisCommands.Pipe;
using RedisNaruto.RedisCommands.Transaction;

namespace RedisNaruto;

/// <summary>
/// 对外暴露的redis命令接口
/// </summary>
public interface IRedisCommand : IAsyncDisposable
{
    /// <summary>
    /// 存储字符串
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="timeSpan"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetAsync(string key, Object value, TimeSpan timeSpan = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量设置值
    /// </summary>
    /// <param name="vals"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> MSetAsync(Dictionary<string, string> vals,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 如果key不存在 才添加值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetNxAsync(string key, object value,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回字符串的长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> StrLenAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 覆盖字符串的值 从指定的偏移量开始
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    Task<long> SetRangeAsync(string key, long offset, string value,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询字符串
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询字符串
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResult> GetAsync<TResult>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<TResult>> MGetAsync<TResult>(string[] key, CancellationToken cancellationToken = default);

    ///  <summary>
    /// 如果key已经存在并且是一个字符串，此命令将value在字符串末尾附加。如果key不存在，则创建它并将其设置为空字符串，因此与这种特殊情况APPEND 类似SET。
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="val">值</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    Task AppendAsync(string key, string val, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按照指定的值递减
    /// </summary>
    /// <param name="key"></param>
    /// <param name="val">递减的值</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> DecrByAsync(string key, long val = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按照指定的值递增
    /// </summary>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> IncrByAsync(string key, long val = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询指定的键值 如果存在就删除
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetDelAsync(string key, CancellationToken cancellationToken = default);

    ///  <summary>
    /// 查询key的值 并设置过期时间
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="expireTime">过期时间</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    Task<string> GetExAsync(string key, TimeSpan expireTime,
        CancellationToken cancellationToken = default);

    ///  <summary>
    /// 获取字符串 指定区间的值
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="end">字符串的结束下标</param>
    ///  <param name="cancellationToken"></param>
    ///  <param name="begin">字符串的开始下标</param>
    ///  <returns></returns>
    Task<string> GetRangeAsync(string key, int begin, int end,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 匹配两个字符串的相似程度 返回匹配成功的内容
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> LcsWithStringAsync(string key1, string key2,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 匹配两个字符串的相似程度 返回匹配成功的内容的长度
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> LcsWithLenAsync(string key1, string key2, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布消息
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>收到消息的客户端数量</returns>
    Task<int> PublishAsync(string topic, string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <param name="topics"></param>
    /// <param name="reciveMessage"></param>
    /// <param name="cancellationToken"></param>
    Task SubscribeAsync(string[] topics, Func<string, string, Task> reciveMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消订阅消息
    /// </summary>
    /// <param name="topics"></param>
    /// <param name="cancellationToken"></param>
    Task UnSubscribeAsync(string[] topics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回客户端id
    /// 1.它从不重复，所以如果CLIENT ID返回相同的数字，调用者可以确定底层客户端没有断开并重新连接连接，但它仍然是同一个连接。
    ///2.ID 是单调递增的。如果一个连接的 ID 大于另一个连接的 ID，则可以保证在稍后的时间与服务器建立第二个连接。
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> ClientIdAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<bool> PingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回选中数据库的key数量
    /// </summary>
    /// <returns></returns>
    Task<long> DbSizeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行script脚本
    /// </summary>
    /// <param name="script"></param>
    /// <param name="keys"></param>
    /// <param name="argvs"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<object> EvalAsync(string script, object[] keys, object[] argvs,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行指定的 sha值的 lua缓存脚本 调用 SCRIPT LOAD 脚本返回的 sha值
    /// </summary>
    /// <param name="sha"></param>
    /// <param name="keys"></param>
    /// <param name="argvs"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<object> EvalShaAsync(string sha, object[] keys, object[] argvs,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将script 存储到redis lua缓存中
    /// </summary>
    /// <param name="script"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> ScriptLoadAsync(string script,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证lua脚本 是否已经缓存
    /// </summary>
    /// <param name="sha"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<bool>> ScriptExistsAsync(string[] sha,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开启事务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ITransactionRedisCommand> MultiAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消key的监视
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UnWatchAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 监视指定的 key,如果 之后key的值被修改了， 则影响事务的exec 命令执行失败 返回null
    /// https://redis.io/docs/manual/transactions/
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task WatchAsync(string[] keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// 开启流水线
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IPipeRedisCommand> BeginPipeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// hash删除
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> HDelAsync(string key, string[] fields, CancellationToken cancellationToken = default);

    /// <summary>
    /// hash存储
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> HSetAsync(string key, Dictionary<string, object> fields,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// hash存在
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> HExistsAsync(string key, string field, CancellationToken cancellationToken = default);

    /// <summary>
    /// hash获取
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> HGetAsync(string key, string field, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有的hash数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Dictionary<string, string>> HGetAllAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// hash 递增
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="increment">递增的值</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<double> HIncrByAsync(string key, string field, double increment = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// hash 的field 信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<string>> HKeysAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// hash 长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> HLenAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<string>> HMGetAsync(string key, string[] fields,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 随机获取hash数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">数量</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Dictionary<string, string>> HRandFieldWithValueAsync(string key, int count = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 随机获取hash数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">数量</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<string>> HRandFieldAsync(string key, int count = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取hash的值信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<string>> HValsAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取hash字段对应值的长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    Task<long> HStrLenAsync(string key, string field,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置hast的值 如果不存在的话 就添加
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value">具体的值</param>
    /// <param name="cancellationToken"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    Task<bool> HSetNxAsync(string key, string field, object value,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 扫码hash的数据
    /// https://redis.io/commands/scan/
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">条数</param>
    /// <param name="cancellationToken"></param>
    /// <param name="matchPattern">匹配条件</param>
    /// <returns></returns>
    IAsyncEnumerable<Dictionary<string, string>> HScanAsync(string key,
        string matchPattern = "*", int count = 10,
        [EnumeratorCancellation] CancellationToken cancellationToken = default);
}