using System.Net;
using System.Runtime.CompilerServices;
using RedisNaruto.Enums;
using RedisNaruto.Internal.Attributes;
using RedisNaruto.Internal.Enums;
using RedisNaruto.Models;
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
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<bool> SetAsync(string key, object value, TimeSpan timeSpan = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量设置值
    /// </summary>
    /// <param name="vals"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_1, TimeComplexityEnum.O1)]
    Task<bool> MSetAsync(Dictionary<string, string> vals,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 如果key不存在 才添加值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="timeSpan">过期时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<bool> SetNxAsync(string key, object value,TimeSpan timeSpan = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回字符串的长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
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
    [RedisIdentification(RedisVersionSupportEnum.Since2_2_0, TimeComplexityEnum.O1)]
    Task<long> SetRangeAsync(string key, long offset, string value,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询字符串
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<RedisValue> GetAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询字符串
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<TResult> GetAsync<TResult>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> MGetAsync(string[] key, CancellationToken cancellationToken = default);

    ///  <summary>
    /// 如果key已经存在并且是一个字符串，此命令将value在字符串末尾附加。如果key不存在，则创建它并将其设置为空字符串，因此与这种特殊情况APPEND 类似SET。
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="val">值</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.O1)]
    Task AppendAsync(string key, string val, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按照指定的值递减
    /// </summary>
    /// <param name="key"></param>
    /// <param name="val">递减的值</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<long> DecrByAsync(string key, long val = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按照指定的值递增
    /// </summary>
    /// <param name="key"></param>
    /// <param name="val"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<long> IncrByAsync(string key, long val = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询指定的键值 如果存在就删除
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.O1)]
    Task<RedisValue> GetDelAsync(string key, CancellationToken cancellationToken = default);

    ///  <summary>
    /// 查询key的值 并设置过期时间
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="expireTime">过期时间</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.O1)]
    Task<RedisValue> GetExAsync(string key, TimeSpan expireTime,
        CancellationToken cancellationToken = default);

    ///  <summary>
    /// 获取字符串 指定区间的值
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="end">字符串的结束下标</param>
    ///  <param name="cancellationToken"></param>
    ///  <param name="begin">字符串的开始下标</param>
    ///  <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_4_0, TimeComplexityEnum.On)]
    Task<string> GetRangeAsync(string key, int begin, int end,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 匹配两个字符串的相似程度 返回匹配成功的内容
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since7_0_0, TimeComplexityEnum.Onm)]
    Task<string> LcsWithStringAsync(string key1, string key2,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 匹配两个字符串的相似程度 返回匹配成功的内容的长度
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since7_0_0, TimeComplexityEnum.Onm)]
    Task<long> LcsWithLenAsync(string key1, string key2, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布消息
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>收到消息的客户端数量</returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.On_m)]
    Task<int> PublishAsync(string topic, string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <param name="topics"></param>
    /// <param name="reciveMessage"></param>
    /// <param name="cancellationToken"></param>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.On)]
    Task SubscribeAsync(string[] topics, Func<string, string, Task> reciveMessage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消订阅消息
    /// </summary>
    /// <param name="topics"></param>
    /// <param name="cancellationToken"></param>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.On)]
    Task UnSubscribeAsync(string[] topics,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回客户端id
    /// 1.它从不重复，所以如果CLIENT ID返回相同的数字，调用者可以确定底层客户端没有断开并重新连接连接，但它仍然是同一个连接。
    ///2.ID 是单调递增的。如果一个连接的 ID 大于另一个连接的 ID，则可以保证在稍后的时间与服务器建立第二个连接。
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.O1)]
    Task<long> ClientIdAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<bool> PingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回选中数据库的key数量
    /// </summary>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<long> DbSizeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 慢日志查询
    /// </summary>
    /// <param name="count">返回的条数</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_1_2, TimeComplexityEnum.On)]
    Task<SlowLogModel[]> SlowLogAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// 慢日志的条数
    /// </summary>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_1_2, TimeComplexityEnum.O1)]
    Task<int> SlowLogLenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 清除慢日志记录
    /// </summary>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_1_2, TimeComplexityEnum.On)]
    Task<bool> SlowLogResetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行script脚本
    /// </summary>
    /// <param name="script"></param>
    /// <param name="keys"></param>
    /// <param name="argvs"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_6_0, TimeComplexityEnum.UnKnow)]
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
    [RedisIdentification(RedisVersionSupportEnum.Since2_6_0, TimeComplexityEnum.UnKnow)]
    Task<object> EvalShaAsync(string sha, object[] keys, object[] argvs,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将script 存储到redis lua缓存中
    /// </summary>
    /// <param name="script"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_6_0, TimeComplexityEnum.On)]
    Task<string> ScriptLoadAsync(string script,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证lua脚本 是否已经缓存
    /// </summary>
    /// <param name="sha"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_6_0, TimeComplexityEnum.On)]
    Task<List<bool>> ScriptExistsAsync(string[] sha,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开启事务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_2_0, TimeComplexityEnum.O1)]
    Task<ITransactionRedisCommand> MultiAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消key的监视
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_2_0, TimeComplexityEnum.UnKnow)]
    Task UnWatchAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 监视指定的 key,如果 之后key的值被修改了， 则影响事务的exec 命令执行失败 返回null
    /// https://redis.io/docs/manual/transactions/
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_2_0, TimeComplexityEnum.UnKnow)]
    Task WatchAsync(string[] keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// 开启流水线
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.SinceAll, TimeComplexityEnum.UnKnow)]
    Task<IPipeRedisCommand> BeginPipeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// hash删除
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.On)]
    Task<long> HDelAsync(string key, string[] fields, CancellationToken cancellationToken = default);

    /// <summary>
    /// hash存储
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.O1)]
    Task<long> HSetAsync(string key, Dictionary<string, object> fields,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// hash存在
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.O1)]
    Task<bool> HExistsAsync(string key, string field, CancellationToken cancellationToken = default);

    /// <summary>
    /// hash获取
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.O1)]
    Task<string> HGetAsync(string key, string field, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有的hash数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.On)]
    Task<Dictionary<string, RedisValue>> HGetAllAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// hash 递增
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="increment">递增的值</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.O1)]
    Task<double> HIncrByAsync(string key, string field, double increment = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// hash 的field 信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> HKeysAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// hash 长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.O1)]
    Task<long> HLenAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量获取
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fields"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> HMGetAsync(string key, string[] fields,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 随机获取hash数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">数量</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.On)]
    Task<Dictionary<string, RedisValue>> HRandFieldWithValueAsync(string key, int count = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 随机获取hash数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">数量</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> HRandFieldAsync(string key, int count = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取hash的值信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> HValsAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取hash字段对应值的长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since3_2_0, TimeComplexityEnum.O1)]
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
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.O1)]
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
    [RedisIdentification(RedisVersionSupportEnum.Since2_8_0, TimeComplexityEnum.O1)]
    IAsyncEnumerable<Dictionary<string, RedisValue>> HScanAsync(string key,
        string matchPattern = "*", int count = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<bool> SAddAsync(string key, object[] value,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回set中集合的长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<long> SCardAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回由第一个集合和所有后续集合之间的差异产生的集合成员。
    /// 类似 except
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> SDiffAsync(string[] key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 此命令等于SDIFF，但不是返回结果集，而是存储在destination.
    /// 将except的差异值存到目标destination
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<int> SDiffStoreAsync(string destination, string[] keys,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回由所有给定集的交集产生的集的成员。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> SInterAsync(string[] key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回交集的值数量 类似与 SInter，SInter返回具体的交集数据，SInterCard返回数
    /// </summary>
    /// <param name="key"></param>
    /// <param name="limit">默认情况下，该命令计算所有给定集的交集的基数。当提供可选LIMIT参数（默认为 0，表示无限制）时，如果交集基数在计算中途达到 limit，则算法将退出并产生 limit 作为基数。这样的实现确保了限制低于实际交集基数的查询的显着加速。</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since7_0_0, TimeComplexityEnum.Onm)]
    Task<long> SInterCardAsync(string[] key, int limit = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 此命令等于SInter，但不是返回结果集，而是存储在destination.
    /// 将except的差异值存到目标destination
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.Onm)]
    Task<int> SInterStoreAsync(string destination, string[] keys,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断值是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<bool> SisMemberAsync(string key, object member,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取set的数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> SMembersAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断值是否存在set中
    /// </summary>
    /// <param name="key"></param>
    /// <param name="members"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> SmisMemberAsync(string key, object[] members,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将值从 source 移动到 destination
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="member"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<bool> SMoveAsync(string source, string destination, object member,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 随机移除 指定数量的值 原子性操作
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> SPopAsync(string key, int count = 1,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// 返回随机对象
    /// 如果提供的参数为正，则返回不同元素count的数组。数组的长度是集合的基数 ( ) 之一，以较小者为准。
    /// 如果用否调整使用count，行为会改变，命可以多次返回相同的元素。在这种情况下，返回的元数是指定的绝对值count。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> SRandMemberAsync(string key, int count = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除成员信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="members">需要删除的成员</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<int> SRemAsync(string key, object[] members,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///返回多个set的并集
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> SUnionAsync(string[] keys,
        CancellationToken cancellationToken = default);

    ///  <summary>
    /// 将多个key的并集 存储到一个新的目标set中
    ///  </summary>
    ///  <param name="destination"></param>
    ///  <param name="keys"></param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<int> SUnionStoreAsync(string destination, string[] keys,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// 扫描set的数据
    /// https://redis.io/commands/scan/
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">条数</param>
    /// <param name="cancellationToken"></param>
    /// <param name="matchPattern">匹配条件</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_8_0, TimeComplexityEnum.O1)]
    IAsyncEnumerable<List<RedisValue>> SScanAsync(string key,
        string matchPattern = "*", int count = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 往list的指定元素 前或者后插入新的元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="pivot">目标元素</param>
    /// <param name="element">具体插入的元素</param>
    /// <param name="isBefore">是否插入到目标元素前</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_2_0, TimeComplexityEnum.On)]
    Task<int> LInsertAsync(string key, object pivot, object element, bool isBefore,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 往指定的下标处插入元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="index">插入的下标位置</param>
    /// <param name="element">具体插入的元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<bool> LSetAsync(string key, int index, object element,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将所有指定值插入存储在 的列表的尾部key。如果key不存在，则在执行推送操作之前将其创建为空列表。当key持有一个不是列表的值时，返回一个错误。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="element">具体插入的元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<int> RPushAsync(string key, object[] element,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将元素插入到列表的尾部，当key存在的时候才插入
    /// </summary>
    /// <param name="key"></param>
    /// <param name="element">具体插入的元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_2_0, TimeComplexityEnum.O1)]
    Task<int> RPushxAsync(string key, object[] element,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将所有指定值插入存储在 的列表的头部key。如果key不存在，则在执行推送操作之前将其创建为空列表。当key持有一个不是列表的值时，返回一个错误。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="element">具体插入的元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<int> LPushAsync(string key, object[] element,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将元素插入到列表的头部，当key存在的时候才插入
    /// </summary>
    /// <param name="key"></param>
    /// <param name="element">具体插入的元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<int> LPushxAsync(string key, object[] element,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修剪现有列表，使其仅包含指定范围的指定元素。和start都是stop从零开始的索引，其中0是列表的第一个元素（头），1下一个元素等等。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start">开始的下标 如果为负数的话，代表倒数第几</param>
    /// <param name="end">结束的下标 如果为负数的话，代表倒数第几</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<bool> LTrimAsync(string key, int start, int end,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除并返回存储在 的列表的最后一个元素key。
    /// 默认情况下，该命令从列表末尾弹出一个元素。当提供可选count参数时，回复将由最多count元素组成，具体取决于列表的长度。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">返回的消息条数</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    IAsyncEnumerable<RedisValue> RPopAsync(string key, int count = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除并返回存储在 的列表的头部一个元素key。
    /// 默认情况下，该命令从列表头部弹出一个元素。当提供可选count参数时，回复将由最多count元素组成，具体取决于列表的长度。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">返回的消息条数</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    IAsyncEnumerable<RedisValue> LPopAsync(string key, int count = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除列表元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">count > 0：删除元素等于element从头到尾移动。count < 0：删除等于element从尾部移动到头部的元素。count = 0: 移除所有等于 的元素element。</param>
    /// <param name="element">元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On_m)]
    Task<int> LRemAsync(string key, int count, object element,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询集合信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On_m)]
    IAsyncEnumerable<RedisValue> LRangeAsync(string key, int start, int end,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 队列长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<int> LLenAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回指定下标的元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="index">下标</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<RedisValue> LIndexAsync(string key, int index,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回元素的下标位置
    /// </summary>
    /// <param name="key"></param>
    /// <param name="element">具体元素</param>
    /// <param name="maxLen">最大遍历次数</param>
    /// <param name="cancellationToken"></param>
    /// <param name="rank">RANK选项指定要返回的第一个元素的“排名”，以防有多个匹配项。等级 1 表示返回第一个匹配项，等级 2 表示返回第二个匹配项，依此类推</param>
    /// <param name="count">返回count 个 匹配成功的元素</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_0_6, TimeComplexityEnum.On)]
    Task<List<RedisValue>> LPosAsync(string key, object element, int rank = 1, int count = 1, int maxLen = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从提供的键名列表中的第一个非空列表键中弹出一个或多个元素。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">每个key返回的消息数</param>
    /// <param name="cancellationToken"></param>
    /// <param name="isLeft">是从左还是右 弹出</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since7_0_0, TimeComplexityEnum.On_m)]
    Task<Dictionary<string, List<RedisValue>>> LmPopAsync(string[] key, bool isLeft = true, int count = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// lMPop的阻塞版本
    /// 从提供的键名列表中的第一个非空列表键中弹出一个或多个元素。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">每个key返回的消息数</param>
    /// <param name="cancellationToken"></param>
    /// <param name="timeout">超时时间</param>
    /// <param name="isLeft">是从左还是右 弹出</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since7_0_0, TimeComplexityEnum.On_m)]
    Task<Dictionary<string, List<RedisValue>>> BlMPopAsync(string[] key, TimeSpan timeout, bool isLeft = true,
        int count = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将 source 消息从左或者右 移除，并存储到 destination 元素中的 left 或者right
    /// </summary>
    /// <param name="isDestinationLeft">指定destinationKey 是从左还是从右写入</param>
    /// <param name="cancellationToken"></param>
    /// <param name="sourceKey">原始的key</param>
    /// <param name="destinationKey">目标的key</param>
    /// <param name="isSourceLeft">指定sourceKey 是从左还是从右读取</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.O1)]
    Task<RedisValue> LMoveAsync(string sourceKey, string destinationKey,
        bool isSourceLeft = true, bool isDestinationLeft = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// BLMOVE是 的阻塞变体LMOVE 当source为空时，Redis 将阻塞连接，直到另一个客户端推送到它或直到达到timeout
    /// 将 source 消息从左或者右 移除，并存储到 destination 元素中的 left 或者right
    /// </summary>
    /// <param name="isDestinationLeft">指定destinationKey 是从左还是从右写入</param>
    /// <param name="cancellationToken"></param>
    /// <param name="sourceKey">原始的key</param>
    /// <param name="destinationKey">目标的key</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="isSourceLeft">指定sourceKey 是从左还是从右读取</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.O1)]
    Task<RedisValue> BLMoveAsync(string sourceKey, string destinationKey, TimeSpan timeout,
        bool isSourceLeft = true, bool isDestinationLeft = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 阻塞
    /// 移除并返回存储在 的列表的最后一个元素key。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="timeout">超时时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.On)]
    Task<(string key, RedisValue value)> BRPopAsync(string[] key, TimeSpan timeout,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 阻塞
    /// 移除并返回存储在 的列表的头部一个元素key。
    /// 默认情况下，该命令从列表头部弹出一个元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="timeout">超时时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.On)]
    Task<(string key, RedisValue value)> BLPopAsync(string[] key, TimeSpan timeout,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// 添加zset 集合
    /// </summary>
    /// <param name="key"></param>
    /// <param name="values">存储的数据</param>
    /// <param name="type">操作类型</param>
    /// <param name="isIncr">指定此选项时的ZADD行为类似于ZINCRBY。在此模式下只能指定一个分数元素对。当指定了此元素 返回的值为更新完成的新的score</param>
    /// <param name="isCh">修改返回值，默认返回新添加的元素个数，修改为返回 总的变化的元素个数</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_2_0, TimeComplexityEnum.OlogN)]
    Task<int> ZAddAsync(string key, SortedSetAddModel[] values,
        SortedSetAddEnum type = SortedSetAddEnum.No, bool isIncr = false, bool isCh = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 扫描zset的数据
    /// https://redis.io/commands/scan/
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count">条数</param>
    /// <param name="cancellationToken"></param>
    /// <param name="matchPattern">匹配条件</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_8_0, TimeComplexityEnum.O1)]
    IAsyncEnumerable<Dictionary<string, RedisValue>> ZScanAsync(string key,
        string matchPattern = "*", int count = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回zset中集合的长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_2_0, TimeComplexityEnum.O1)]
    Task<long> ZCardAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回区间范围中的元素数量
    /// </summary>
    /// <param name="key"></param>
    /// <param name="max"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="min"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.OlogN)]
    Task<long> ZCountAsync(string key, string min = "-inf", string max = "+inf",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回由第一个集合和所有后续集合之间的差异产生的集合成员。
    /// 类似 except
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.OL_N_K_logN)]
    Task<List<RedisValue>> ZDiffAsync(string[] key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回由第一个集合和所有后续集合之间的差异产生的集合成员。
    /// 类似 except
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.OL_N_K_logN)]
    Task<Dictionary<RedisValue, long>> ZDiffWithScoreAsync(string[] key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 此命令等于ZDIFF，但不是返回结果集，而是存储在destination.
    /// 将except的差异值存到目标destination
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.OL_N_K_logN)]
    Task<int> ZDiffStoreAsync(string destination, string[] keys,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 递增zset元素中的score值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="increment"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_2_0, TimeComplexityEnum.OlogN)]
    Task<int> ZIncrByAsync(string key, object member, int increment = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 计算并集 将结果存储到 dest
    /// </summary>
    /// <code>
    /// ZUNIONSTORE result 2 zset1 zset2 WEIGHTS 2 3
    /// 这将对 zset1 的成员的分数乘以 2，对 zset2 的成员的分数乘以 3，然后计算它们的并集并将结果存储到 dest 中。
    /// </code>
    /// <param name="dest">目标元素</param>
    /// <param name="keys">参与操作的key集合</param>
    /// <param name="weights">用于指定每个输入有序集合的权重。默认情况下，每个输入有序集合的权重为 1。</param>
    /// <param name="aggregate">用于指定对于具有相同成员的元素，如何计算它们的分数。默认情况下使用 SUM 计算。其他可选项为 MIN 和 MAX。</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.ONK_O_M_logM)]
    Task<int> ZUnionStoreAsync(string dest, string[] keys, long[] weights = null,
        SortedSetAggregateEnum aggregate = SortedSetAggregateEnum.Sum,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回由所有给定集的交集产生的集的成员。
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="aggregate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="weights"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.ONK_O_M_logM)]
    Task<List<RedisValue>> ZInterAsync(string[] keys, long[] weights = null,
        SortedSetAggregateEnum aggregate = SortedSetAggregateEnum.Sum,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回由所有给定集的交集产生的集的成员。
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="aggregate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="weights"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.ONK_O_M_logM)]
    Task<Dictionary<RedisValue, long>> ZInterWithScoreAsync(string[] keys, long[] weights = null,
        SortedSetAggregateEnum aggregate = SortedSetAggregateEnum.Sum,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回交集的值数量 此命令类似于ZINTER，但它不返回结果集，而是仅返回结果的基数。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="limit">默认情况下，该命令计算所有给定集的交集的基数。当提供可选LIMIT参数（默认为 0，表示无限制）时，如果交集基数在计算中途达到 limit，则算法将退出并产生 limit 作为基数。这样的实现确保了限制低于实际交集基数的查询的显着加速。</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since7_0_0, TimeComplexityEnum.Onm)]
    Task<long> ZInterCardAsync(string[] key, int limit = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 此命令等于ZInter，但不是返回结果集，而是存储在destination.
    /// 将except的差异值存到目标destination
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="keys"></param>
    /// <param name="aggregate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="weights"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.ONK_O_M_logM)]
    Task<int> ZInterStoreAsync(string destination, string[] keys, long[] weights = null,
        SortedSetAggregateEnum aggregate = SortedSetAggregateEnum.Sum,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回由所有给定集的并集产生的集的成员。
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="aggregate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="weights"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.ON_O_M_logM)]
    Task<List<RedisValue>> ZUnionAsync(string[] keys, long[] weights = null,
        SortedSetAggregateEnum aggregate = SortedSetAggregateEnum.Sum,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回由所有给定集的并集产生的集的成员。
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="aggregate"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="weights"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.ON_O_M_logM)]
    Task<Dictionary<RedisValue, long>> ZUnionWithScoreAsync(string[] keys, long[] weights = null,
        SortedSetAggregateEnum aggregate = SortedSetAggregateEnum.Sum,
        CancellationToken cancellationToken = default);

    ///  <summary>
    /// 获取元素的分数信息
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="member">元素</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_2_0, TimeComplexityEnum.O1)]
    Task<long> ZScoreAsync(string key, object member,
        CancellationToken cancellationToken = default);

    ///  <summary>
    /// 当排序集中的所有元素都以相同的分数插入时，为了强制按字典顺序排序，此命令返回排序集中的元素数，其值介于key和min之间max。
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="min"></param>
    ///  <param name="max"></param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_8_9, TimeComplexityEnum.OlogN)]
    Task<long> ZLexCountAsync(string key, string min = "-", string max = "+",
        CancellationToken cancellationToken = default);

    ///  <summary>
    /// 从提供的键名列表中的第一个非空排序集中弹出一个或多个元素，即成员分数对。
    ///  </summary>
    ///  <param name="keys"></param>
    ///  <param name="minMax">标记从最小的分数 还是最大的分数弹出元素的数量</param>
    ///  <param name="count">弹出的数量</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since7_0_0, TimeComplexityEnum.ON_O_M_logM)]
    Task<(string key, Dictionary<RedisValue, long> data)> ZMpopAsync(string[] keys,
        SortedSetMinMaxEnum minMax = SortedSetMinMaxEnum.Min,
        long count = 1,
        CancellationToken cancellationToken = default);

    ///  <summary>
    /// 从提供的键名列表中的第一个非空排序集中弹出一个或多个元素，即成员分数对。
    /// 阻塞
    ///  </summary>
    ///  <param name="keys"></param>
    ///  <param name="timeout">超时时间</param>
    ///  <param name="minMax">标记从最小的分数 还是最大的分数弹出元素的数量</param>
    ///  <param name="count">弹出的数量</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since7_0_0, TimeComplexityEnum.ON_O_M_logM)]
    Task<(string key, Dictionary<RedisValue, long> data)> BZMpopAsync(string[] keys, TimeSpan timeout,
        SortedSetMinMaxEnum minMax = SortedSetMinMaxEnum.Min,
        long count = 1,
        CancellationToken cancellationToken = default);


    ///  <summary>
    /// 获取元素的分数信息
    ///  </summary>
    ///  <param name="key"></param>
    ///  <param name="members">元素</param>
    ///  <param name="cancellationToken"></param>
    ///  <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> ZMScoreAsync(string key, object[] members,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// 删除并返回count存储在 的排序集中得分最高的成员key。
    /// 当返回多个元素时，得分最高的将是第一个，其次是得分较小的元素。。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.OlogNM)]
    Task<Dictionary<RedisValue, long>> ZPopMaxAsync(string key, int count = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除并返回count存储在 的排序集中得分最低的成员key。
    /// 当返回多个元素时，得分最低的将是第一个，其次是得分较大的元素。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.OlogNM)]
    Task<Dictionary<RedisValue, long>> ZPopMinAsync(string key, int count = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 当仅使用key参数调用时，从存储在 的已排序集合值中返回一个随机元素key。
    /// 如果提供的参数为正，则返回不同元素count的数组。数组的长度是排序集的基数 ( ) 之一，以较小者为准。
    ///如果用否定调用count，行为会改变，命令可以多次返回相同的元素。在这种情况下，返回的元素数是指定的绝对值count
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> ZRandMemberAsync(string key, int count = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 当仅使用key参数调用时，从存储在 的已排序集合值中返回一个随机元素key。
    /// 如果提供的参数为正，则返回不同元素count的数组。数组的长度是排序集的基数 ( ) 之一，以较小者为准。
    ///如果用否定调用count，行为会改变，命令可以多次返回相同的元素。在这种情况下，返回的元素数是指定的绝对值count
    /// </summary>
    /// <param name="key"></param>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.On)]
    Task<List<SortedSetModel>> ZRandMemberWithScoreAsync(string key, int count = 1,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// 查询zset中数据的信息 默认是从低到高
    /// </summary>
    /// <code>
    /// ZRANGE zset (1 5 BYSCORE
    /// while将返回所有元素1 < score <= 5：
    /// 
    /// ZRANGE zset (5 (10 BYSCORE
    /// 将返回所有元素5 < score < 10（排除 5 和 10）
    /// </code>
    /// <param name="key"></param>
    /// <param name="start">开始位置 (-inf正无穷负无穷 需要搭配ByScore使用）, 在分数前加上 ( 符号 代表排除当前值</param>
    /// <param name="stop">结束位置 (-inf正无穷负无穷 需要搭配ByScore使用）, 在分数前加上 ( 符号 代表排除当前值</param>
    /// <param name="isLimit">6.2.0 开启limit</param>
    /// <param name="count">需要开启isLimit </param>
    /// <param name="offset">需要开启isLimit</param>
    /// <param name="scoreLex">6.2.0 </param>
    /// <param name="rev">6.2.0
    /// 默认的返回值顺序的从低到高，true 代表反转数据，从高到低
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_2_0, TimeComplexityEnum.OlogN_M)]
    Task<List<RedisValue>> ZRangeAsync(string key, string start = "0", string stop = "1",
        bool isLimit = false, int offset = 0, int count = 0,
        SortedSetScoreLexEnum scoreLex = SortedSetScoreLexEnum.Defaut, bool rev = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询zset中数据的信息 默认是从低到高
    /// </summary>
    /// <code>
    /// ZRANGE zset (1 5 BYSCORE
    /// while将返回所有元素1 < score <= 5：
    /// 
    /// ZRANGE zset (5 (10 BYSCORE
    /// 将返回所有元素5 < score < 10（排除 5 和 10）
    /// </code>
    /// <param name="key"></param>
    /// <param name="start">开始位置 (-inf正无穷负无穷 需要搭配ByScore使用）, 在分数前加上 ( 符号 代表排除当前值</param>
    /// <param name="stop">结束位置 (-inf正无穷负无穷 需要搭配ByScore使用）, 在分数前加上 ( 符号 代表排除当前值</param>
    /// <param name="isLimit">6.2.0 开启limit</param>
    /// <param name="count">需要开启isLimit </param>
    /// <param name="offset">需要开启isLimit</param>
    /// <param name="scoreLex">6.2.0 </param>
    /// <param name="rev">6.2.0
    /// 默认的返回值顺序的从低到高，true 代表反转数据，从高到低
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_5_0, TimeComplexityEnum.OlogN_M)]
    Task<List<SortedSetModel>> ZRangeWithScoreAsync(string key, string start = "0", string stop = "1",
        bool isLimit = false, int offset = 0, int count = 0,
        SortedSetScoreLexEnum scoreLex = SortedSetScoreLexEnum.Defaut, bool rev = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 此命令类似于ZRANGE，但将结果存储在<dest>目标键中
    /// </summary>
    /// <code>
    /// </code>
    /// <param name="dest">目标元素</param>
    /// <param name="key"></param>
    /// <param name="start">开始位置 (-inf正无穷负无穷 需要搭配ByScore使用）, 在分数前加上 ( 符号 代表排除当前值</param>
    /// <param name="stop">结束位置 (-inf正无穷负无穷 需要搭配ByScore使用）, 在分数前加上 ( 符号 代表排除当前值</param>
    /// <param name="isLimit">6.2.0 开启limit</param>
    /// <param name="count">需要开启isLimit </param>
    /// <param name="offset">需要开启isLimit</param>
    /// <param name="scoreLex">6.2.0 </param>
    /// <param name="rev">6.2.0
    /// 默认的返回值顺序的从低到高，true 代表反转数据，从高到低
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.OlogN_M)]
    Task<int> ZRangeStoreAsync(string dest, string key, string start = "0", string stop = "1",
        bool isLimit = false, int offset = 0, int count = 0,
        SortedSetScoreLexEnum scoreLex = SortedSetScoreLexEnum.Defaut, bool rev = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从低到高返回member对应的名次信息 默认从0 开始
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.OlogN)]
    Task<int> ZRankAsync(string key, object member,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// member返回存储在 的已排序集合中的排名key，分数从高到低排序。排名（或索引）从 0 开始，这意味着得分最高的成员具有排名0。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.OlogN)]
    Task<int> ZRevRankAsync(string key, object member,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// 从存储在 的排序集中删除指定成员key。不存在的成员将被忽略。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="member"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_2_0, TimeComplexityEnum.OlogN_M)]
    Task<int> ZRemAsync(string key, object[] member,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 此命令删除存储在 和 指定的字典序范围之间的有序集合中的所有key元素。minmax
    /// </summary>
    /// <param name="key"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_8_9, TimeComplexityEnum.OlogN_M)]
    Task<int> ZRemRangeByLexAsync(string key, string min, string max,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按照排行从低到高删除
    /// </summary>
    /// <param name="key"></param>
    /// <param name="start"></param>
    /// <param name="stop"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_0_0, TimeComplexityEnum.OlogN_M)]
    Task<int> ZRemRangeByRankAsync(string key, int start, int stop,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除score 分数之间的所有元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_2_0, TimeComplexityEnum.OlogN_M)]
    Task<int> ZRemRangeByScoreAsync(string key, string min, string max,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除最小的元素
    /// 阻塞版本
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.OlogN)]
    Task<(string key, SortedSetModel data)> BzPopMinAsync(string[] key, TimeSpan timeout,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除最大的元素
    /// 阻塞版本
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.OlogN)]
    Task<(string key, SortedSetModel data)> BzPopMaxAsync(string[] key, TimeSpan timeout,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 消息确认 针对消息组
    /// </summary>
    /// <param name="key"></param>
    /// <param name="group">消息组</param>
    /// <param name="messageId">消息id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.O1)]
    Task<int> XAckAsync(string key, string group, string[] messageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加流消息 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data">数据 kv结构存储到redis中为json格式数据</param>
    /// <param name="messageId">消息id * 号代表使用redis生成的消息id,如果自定义传递 新的id 需要大于流里面最新的id </param>
    /// <param name="isCreateStream">是否创建流，当为false的时候，当流不存在的话，不执行创建流命令. 6.2.0新增</param>
    /// <param name="maxMin">是否需要在添加的时候对流进行限制</param>
    /// <param name="threshold">临界值 需要指定 StreamAddMaxMinEnum</param>
    /// <param name="limitCount"> 6.2.0新增</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.O1)]
    Task<string> XAddAsync(string key, Dictionary<string, object> data, string messageId = "*",
        StreamMaxMinEnum maxMin = StreamMaxMinEnum.Default,
        string threshold = default, long? limitCount = null, bool isCreateStream = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除流消息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="messageId">消息id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.O1)]
    Task<int> XDelAsync(string key, string[] messageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取流长度
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.O1)]
    Task<int> XLenAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 裁剪流消息 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="maxMin">是否需要在添加的时候对流进行限制</param>
    /// <param name="threshold">临界值 需要指定 StreamAddMaxMinEnum</param>
    /// <param name="limitCount"> 6.2.0新增</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.On)]
    Task<string> XTrimAsync(string key,
        StreamMaxMinEnum maxMin = StreamMaxMinEnum.Default,
        string threshold = default, long? limitCount = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建消费者组 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="groupName">组名称,名称在流内唯一，重复会报错</param>
    /// <param name="id">该命令的id参数从新组的角度指定流中最后传送的条目。特殊 ID$是流中最后一个条目的 ID，但您可以用任何有效 ID 替换它。
    /// 例如，如果您希望组的消费者从头开始获取整个流，请使用零作为消费者组的起始 ID：XGROUP CREATE mystream mygroup 0</param>
    /// <param name="createStream">当流不存在的时候 是否创建一个长度为0 的流</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.O1)]
    Task<bool> XGroupCreateAsync(string key, string groupName, string id = "$",
        bool createStream = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建组内的消费者
    /// </summary>
    /// <param name="key"></param>
    /// <param name="groupName">指定的组</param>
    /// <param name="consumer">消费者名称</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.O1)]
    Task<bool> XGroupCreateConsumerAsync(string key, string groupName, string consumer,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除组内的消费者
    /// 消费者拥有的任何待处理消息在被删除后将变得不可领取。因此，强烈建议在从组中删除消费者之前声明或确认任何未决消息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="groupName">指定的组</param>
    /// <param name="consumer">消费者名称</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.O1)]
    Task<int> XGroupDeleteConsumerAsync(string key, string groupName, string consumer,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 销毁流对应的消费者组的所有数据 包含所有与消费者组的消息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="groupName">指定的组</param>
    /// <param name="cancellationToken"></param>
    /// <returns>消费者在删除之前拥有的待处理消息数</returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.On)]
    Task<int> XGroupDestroyAsync(string key, string groupName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置消费者组的最新交付的id
    /// </summary>
    /// <param name="key"></param>
    /// <param name="groupName">指定的组</param>
    /// <param name="id">重新读取 设置0，$为最新的</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.O1)]
    Task<bool> XGroupSetIdAsync(string key, string groupName, string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取流内的消费者组的信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.O1)]
    Task<List<XGroupInfoModel>> XGroupInfoAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取流的消费者组中的消费者的信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.O1)]
    Task<List<XConsumerInfoModel>> XConsumerInfoAsync(string key, string group,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取流信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.O1)]
    Task<XStreamInfoModel> XStreamInfoAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取流消息
    /// </summary>
    /// <param name="streamOffset">流信息</param>
    /// <param name="count">每个流返回的元素数据</param>
    /// <param name="blockTime">阻塞时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.UnKnow)]
    Task<List<StreamModel>> XReadAsync(ReadStreamOffset[] streamOffset, int count,
        TimeSpan? blockTime = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取流消息
    /// </summary>
    /// <param name="key">流的缓存key</param>
    /// <param name="start">从哪个消息id开始， -代表最小的，</param>
    /// <param name="end">从哪个消息id结束， +代表最大的，</param>
    /// <param name="count">返回的元素数据</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.On)]
    Task<List<StreamEntityModel>> XRangeAsync(string key, string start, string end, int count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取流消息
    /// 反向
    /// </summary>
    /// <param name="key">流的缓存key</param>
    /// <param name="start">从哪个消息id开始， +代表最大的，</param>
    /// <param name="end">从哪个消息id结束， -代表最小的，</param>
    /// <param name="count">返回的元素数据</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.On)]
    Task<List<StreamEntityModel>> XRevRangeAsync(string key, string start, string end, int count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从消费组读取消息
    /// </summary>
    /// <param name="streamOffset">流信息</param>
    /// <param name="groupName">消费者组名称</param>
    /// <param name="consumerName">消费者信息</param>
    /// <param name="blockTime">阻塞时间</param>
    /// <param name="count">每次读取的消息数量</param>
    /// <param name="noAck"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.UnKnow)]
    Task<List<StreamModel>> XReadGroupAsync(ReadGroupStreamOffset[] streamOffset, string groupName, string consumerName,
        int count, TimeSpan? blockTime = null, bool noAck = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取消费者组中的未ack的消息对应的消费者信息
    /// </summary>
    /// <param name="key">流信息</param>
    /// <param name="groupName">消费者组名称</param>
    /// <param name="consumerName">消费者信息</param>
    /// <param name="count">每次读取的消息数量</param>
    /// <param name="end">最大id</param>
    /// <param name="start">最小的id</param>
    /// <param name="idle">根据空闲时间来筛选。6.2.0</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.On)]
    Task<List<XPendingInfoModel>> XPendingAsync(string key, string groupName,
        int count, string consumerName = null, string start = null, string end = null, TimeSpan? idle = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 转移消息的所有权
    /// </summary>
    /// <param name="key">流</param>
    /// <param name="groupName">消费者组</param>
    /// <param name="consumerName">需要转移到的目标消费者</param>
    /// <param name="isForce">即使某些指定的 ID 尚未在分配给不同客户端的 PEL 中，也会在 PEL 中创建挂起的消息条目。但是消息必须存在于流中，否则不存在消息的 ID 将被忽略。</param>
    /// <param name="cancellationToken"></param>
    /// <param name="messageIds">需要转移的消息id</param>
    /// <param name="minIdleTime">最小的空闲时间</param>
    /// <param name="idle">设置消息的空闲时间（上次发送时间）。如果未指定 IDLE，则假定 IDLE 为 0，即重置时间计数，因为消息现在有新所有者尝试处理它</param>
    /// <param name="time">这与 IDLE 相同，但不是相对的毫秒数，而是将空闲时间设置为特定的 Unix 时间（以毫秒为单位）。这对于重写 AOF 文件生成XCLAIM命令很有用。</param>
    /// <param name="retryCount">将重试计数器设置为指定值。每次再次传递消息时，此计数器都会递增。通常XCLAIM不会更改此计数器，它仅在调用 XPENDING 命令时提供给客户端：这样客户端可以检测异常情况，例如在大量传递尝试后由于某种原因从未处理过的消息</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.OlogN)]
    Task<List<StreamEntityModel>> XClaimAsync(string key, string groupName, string consumerName,
        string[] messageIds,
        TimeSpan minIdleTime,
        TimeSpan? idle = default, TimeSpan? time = default,
        int? retryCount = default, bool isForce = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 转移消息的所有权
    /// 返回消息的id
    /// </summary>
    /// <param name="key">流</param>
    /// <param name="groupName">消费者组</param>
    /// <param name="consumerName">需要转移到的目标消费者</param>
    /// <param name="isForce">即使某些指定的 ID 尚未在分配给不同客户端的 PEL 中，也会在 PEL 中创建挂起的消息条目。但是消息必须存在于流中，否则不存在消息的 ID 将被忽略。</param>
    /// <param name="cancellationToken"></param>
    /// <param name="messageIds">需要转移的消息id</param>
    /// <param name="minIdleTime">最小的空闲时间</param>
    /// <param name="idle">设置消息的空闲时间（上次发送时间）。如果未指定 IDLE，则假定 IDLE 为 0，即重置时间计数，因为消息现在有新所有者尝试处理它</param>
    /// <param name="time">这与 IDLE 相同，但不是相对的毫秒数，而是将空闲时间设置为特定的 Unix 时间（以毫秒为单位）。这对于重写 AOF 文件生成XCLAIM命令很有用。</param>
    /// <param name="retryCount">将重试计数器设置为指定值。每次再次传递消息时，此计数器都会递增。通常XCLAIM不会更改此计数器，它仅在调用 XPENDING 命令时提供给客户端：这样客户端可以检测异常情况，例如在大量传递尝试后由于某种原因从未处理过的消息</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since5_0_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> XClaimWithIdAsync(string key, string groupName, string consumerName,
        string[] messageIds,
        TimeSpan minIdleTime,
        TimeSpan? idle = default, TimeSpan? time = default,
        int? retryCount = default, bool isForce = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 自动转移消息的所有权
    /// 返回消息的id
    /// </summary>
    /// <param name="key">流</param>
    /// <param name="groupName">消费者组</param>
    /// <param name="consumerName">需要转移到的目标消费者</param>
    /// <param name="count">条数上限</param>
    /// <param name="cancellationToken"></param>
    /// <param name="minIdleTime">最小的空闲时间</param>
    /// <param name="start">排查开始的id</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.O1)]
    Task<XAutoClaimWithIdModel> XAutoClaimWithIdAsync(string key, string groupName, string consumerName,
        TimeSpan minIdleTime, string start, int count = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 自动转移消息的所有权
    /// </summary>
    /// <param name="key">流</param>
    /// <param name="groupName">消费者组</param>
    /// <param name="consumerName">需要转移到的目标消费者</param>
    /// <param name="count">条数上限</param>
    /// <param name="cancellationToken"></param>
    /// <param name="minIdleTime">最小的空闲时间</param>
    /// <param name="start">排查开始的id</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.O1)]
    Task<XAutoClaimModel> XAutoClaimAsync(string key, string groupName, string consumerName,
        TimeSpan minIdleTime, string start, int count = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拷贝key
    /// </summary>
    /// <param name="source">原始key</param>
    /// <param name="dest">目标key</param>
    /// <param name="db">需要转移的目标的db</param>
    /// <param name="isReplace">如果true 代表如果目标key存在就先删除key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since6_2_0, TimeComplexityEnum.On)]
    Task<bool> CopyAsync(string source, string dest, int? db = null, bool isReplace = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="keys">需要删除的key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<int> DelAsync(string[] keys,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///  以 Redis 特定格式序列化存储在 key 中的值，并将其返回给用户。可以使用命令将返回值合成回 Redis 键RESTORE 。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_6_0, TimeComplexityEnum.Onm)]
    Task<RedisValue> DumpAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///  将dump序列化的值，反序列化到指定的key中
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expire">过期时间</param>
    /// <param name="isReplace">存储的话 先删除后添加</param>
    /// <param name="cancellationToken"></param>
    /// <param name="serializedValue">序列化值</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_6_0, TimeComplexityEnum.Onm)]
    Task<bool> ReStoreAsync(string key, object serializedValue, TimeSpan? expire = null,
        bool isReplace = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 校验是否存在
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<int> ExistsAsync(string[] keys,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置密钥的过期时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expire">过期时间</param>
    /// <param name="expireEnum"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<int> ExpireAsync(string key, TimeSpan expire, ExpireEnum expireEnum = ExpireEnum.No,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置密钥的过期时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="unixTimeSeconds">过期时间戳</param>
    /// <param name="expireEnum"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_2_0, TimeComplexityEnum.O1)]
    Task<int> ExpireAtAsync(string key, long unixTimeSeconds, ExpireEnum expireEnum = ExpireEnum.No,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///  返回给定密钥将过期的绝对 Unix 时间戳（自 1970 年 1 月 1 日起），以秒为单位。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since7_0_0, TimeComplexityEnum.O1)]
    Task<long> ExpireTimeAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    ///  返回所有匹配的键pattern。
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.On)]
    Task<List<RedisValue>> KeysAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    ///  迁移 原子性操作
    /// </summary>
    /// <param name="keys">需要迁移的key</param>
    /// <param name="remoteHost">目标主机</param>
    /// <param name="todatabase">目标库database</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="isCopy">迁移成功本地key不删除</param>
    /// <param name="userName">auth2模式需要填写</param>
    /// <param name="cancellationToken"></param>
    /// <param name="isReplace">是否替换目标库 如果key存在的话</param>
    /// <param name="authEnum">授权类型</param>
    /// <param name="password">密码</param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_6_0, TimeComplexityEnum.UnKnow)]
    Task<bool> MiGrateAsync(string[] keys, IPEndPoint remoteHost, int todatabase, TimeSpan timeout,
        bool isCopy = false, bool isReplace = false, AuthEnum? authEnum = null, string password = null,
        string userName = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// key从当前选择的数据库（参见 参考资料）移动SELECT到指定的目标数据库。当key它已经存在于目标数据库中，或者它不存在于源数据库中时，它什么也不做。MOVE因此可以用作锁定原语。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="db"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<bool> MoveAsync(string key, int db,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// 返回对象的编码信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_2_3, TimeComplexityEnum.O1)]
    Task<string> ObjectEncodingAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回对象的访问频率
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since4_0_0, TimeComplexityEnum.O1)]
    Task<int> ObjectFreqAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回对象的空闲时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_2_3, TimeComplexityEnum.O1)]
    Task<int> ObjectIdleTimeAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 对象重新计数
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_2_3, TimeComplexityEnum.O1)]
    Task<int> ObjectRefCountAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置key的过期时间为永久生效
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_2_0, TimeComplexityEnum.O1)]
    Task<bool> PersistAsync(string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置密钥的过期时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="expire">过期时间</param>
    /// <param name="expireEnum"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_6_0, TimeComplexityEnum.O1)]
    Task<int> PExpireAsync(string key, TimeSpan expire, ExpireEnum expireEnum = ExpireEnum.No,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置密钥的过期时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="unixTimeMillSeconds">过期时间戳 毫秒</param>
    /// <param name="expireEnum"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_6_0, TimeComplexityEnum.O1)]
    Task<int> PExpireAtAsync(string key, long unixTimeMillSeconds,
        ExpireEnum expireEnum = ExpireEnum.No,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///  返回给定密钥将过期的绝对 Unix 时间戳（自 1970 年 1 月 1 日起），以毫秒为单位。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since7_0_0, TimeComplexityEnum.O1)]
    Task<long> PExpireTimeAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    ///  返回毫秒 过期时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_6_0, TimeComplexityEnum.O1)]
    Task<long> PTtlAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    ///  返回秒 过期时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<long> TtlAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    ///从当前选定的数据库中返回一个随机密钥
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<string> RandomKeyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 改名
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newName">新名称</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<bool> ReNameAsync(string key, string newName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 改名
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newName">新名称</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<bool> ReNameNxAsync(string key, string newName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取数据类型
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<string> TypeAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 此命令与 非常相似DEL：它删除指定的键。就像DEL一个键如果不存在就会被忽略。但是，该命令在不同的线程中执行实际的内存回收，因此它不会阻塞，而DEL会。这就是命令名称的来源：该命令只是从键空间中取消链接键。实际删除将在稍后异步发生
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since4_0_0, TimeComplexityEnum.O1)]
    Task<int> UnLinkAsync(string[] keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// 此命令会阻塞当前客户端，直到所有先前的写命令都成功传输并至少被指定数量的副本确认。如果达到以毫秒为单位指定的超时，即使尚未达到指定的副本数，命令也会返回。
    /// <see cref="https://redis.io/commands/wait/"/>
    /// </summary>
    /// <param name="numreplicas">副本数量</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since3_0_0, TimeComplexityEnum.O1)]
    Task<int> WaitAsync(int numreplicas, TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///  此命令会阻塞当前客户端，直到所有先前的写入命令都被确认为已同步到本地 Redis 的 AOF 和/或至少指定数量的副本。如果达到以毫秒为单位指定的超时时间，即使未达到指定的确认次数，命令也会返回。
    /// <see cref="https://redis.io/commands/waitaof/"/>
    /// </summary>
    /// <param name="numlocal"></param>
    /// <param name="numreplicas">副本数量</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since7_2_0, TimeComplexityEnum.O1)]
    Task<int> WaitAofAsync(int numlocal, int numreplicas, TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///  更改密钥的最后访问时间。如果键不存在，则忽略该键。
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since3_2_0, TimeComplexityEnum.On)]
    Task<int> TouchAsync(string[] keys, CancellationToken cancellationToken = default);


    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="key"></param>
    /// <param name="elements">元素</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_8_9, TimeComplexityEnum.O1)]
    Task<int> PfAddAsync(string key, object[] elements, CancellationToken cancellationToken = default);

    /// <summary>
    /// 返回元素的基数值
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_8_9, TimeComplexityEnum.O1)]
    Task<long> PfCountAsync(string[] keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// 将多个 HyperLogLog 值合并为一个唯一值，该值将近似于观察到的源 HyperLogLog 结构集的并集的基数。
    /// </summary>
    /// <param name="keys">原始key</param>
    /// <param name="destKey">存储的目标key</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since2_8_9, TimeComplexityEnum.On)]
    Task<bool> PfMergeAsync(string[] keys, string destKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 切换db
    /// </summary>
    /// <param name="db"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RedisIdentification(RedisVersionSupportEnum.Since1_0_0, TimeComplexityEnum.O1)]
    Task<ISelectDbRedisCommand> SelectDbAsync(int db, CancellationToken cancellationToken = default);
}