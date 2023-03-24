namespace RedisNaruto.Internal;

internal static class RedisCommandName
{
    #region 字符串

    /// <summary>
    /// 字符串 set
    /// </summary>
    public const string Set = "SET";

    /// <summary>
    /// 批量获取字符串
    /// </summary>
    public const string MGET = "MGET";

    /// <summary>
    /// 获取字符串
    /// </summary>
    public const string Get = "GET";

    /// <summary>
    /// 如果key已经存在并且是一个字符串，此命令将value在字符串末尾附加。如果key不存在，则创建它并将其设置为空字符串，因此与这种特殊情况APPEND 类似SET。
    /// </summary>
    public const string Append = "APPEND";

    /// <summary>
    /// 将存储在的数字减一。如果键不存在，则在0的基础上减1。如果键包含错误类型的值或包含不能表示为整数的字符串，则返回错误。此操作仅限于64 位有符号整数。
    /// </summary>
    public const string Decr = "DECR";

    /// <summary>
    /// 按照指定的数字递减
    /// key的数字递减decrement. 如果键不存在，则在0的基础上减。如果键包含错误类型的值或包含不能表示为整数的字符串，则返回错误。此操作仅限于 64 位有符号整数。
    /// </summary>
    public const string DecrBy = "DECRBY";

    /// <summary>
    ///获取键的值key并删除键。此命令类似于GET，除了它还会在成功时删除键（当且仅当键的值类型是字符串时）。
    ///  6.2.0
    /// </summary>
    public const string GetDel = "GETDEL";

    /// <summary>
    /// 获取key的值 并设置key的过期时间
    /// 6.2.0
    /// </summary>
    public const string GetEx = "GETEX";

    /// <summary>
    /// 获取字符串 指定区间的值
    /// </summary>
    public const string GetRange = "GETRANGE";

    /// <summary>
    /// 递增
    /// </summary>
    public const string IncrBy = "INCRBY";

    /// <summary>
    ///匹配两个字符串的 相似程度
    /// 7.0.0
    /// </summary>
    public const string Lcs = "LCS";

    /// <summary>
    /// 批量设置值
    /// </summary>
    public const string MSet = "MSET";

    /// <summary>
    /// 如果key不存在才添加
    /// </summary>
    public const string SetNx = "SETNX";

    /// <summary>
    /// 字符串的长度
    /// </summary>
    public const string StrLen = "STRLEN";

    /// <summary>
    /// 覆盖字符串的值 从指定的偏移量开始
    /// </summary>
    public const string SetRange = "SETRANGE";

    #endregion


    #region 发布订阅

    /// <summary>
    /// 发送消息 到频道
    /// </summary>
    public const string Pub = "PUBLISH";


    /// <summary>
    /// 订阅频道
    /// </summary>
    public const string Sub = "SUBSCRIBE";


    /// <summary>
    /// 取消订阅
    /// </summary>
    public const string UnSub = "UNSUBSCRIBE";

    #endregion

    #region client

    /// <summary>
    /// 客户端信息
    /// </summary>
    public const string Client = "CLIENT";

    /// <summary>
    /// 登陆
    /// </summary>
    public const string Auth = "AUTH";

    /// <summary>
    /// 退出
    /// </summary>
    public const string Quit = "QUIT";

    /// <summary>
    /// 选择 db
    /// </summary>
    public const string Select = "SELECT";

    /// <summary>
    /// Ping
    /// </summary>
    public const string Ping = "PING";

    #endregion

    #region server

    /// <summary>
    /// 返回选择数据库的 key数量
    /// </summary>
    public const string DbSize = "DBSIZE";

    #endregion

    #region script

    /// <summary>
    /// 执行lua脚本
    /// </summary>
    public const string Eval = "EVAL";

    /// <summary>
    /// 执行指定的 sha值的 lua缓存脚本 调用 SCRIPT LOAD 脚本返回的 sha值
    /// </summary>
    public const string EvalSha = "EVALSHA";

    /// <summary>
    /// 将script 存储到redis lua缓存中 返回 存储的sha值
    /// </summary>
    public const string Script = "SCRIPT";

    #endregion

    #region Sentinel

    /// <summary>
    /// 哨兵命令
    /// </summary>
    public const string Sentinel = "SENTINEL";

    #endregion

    /// <summary>
    /// 获取实例的角色信息 
    /// </summary>
    public const string Role = "ROLE";


    #region tran

    /// <summary>
    /// 开启事务
    /// </summary>
    public const string Multi = "MULTI";

    /// <summary>
    /// 提交事务
    /// </summary>
    public const string Exec = "EXEC";

    /// <summary>
    /// 事务丢弃
    /// </summary>
    public const string DisCard = "DISCARD";

    /// <summary>
    /// 标记要监视的给定键以进行有条件的 交易执行。
    /// </summary>
    public const string Watch = "WATCH";

    /// <summary>
    /// 刷新所有先前监视的交易密钥。
    /// 如果您调用EXECor DISCARD，则无需手动调用UNWATCH.
    /// </summary>
    public const string UnWatch = "UNWATCH";

    #endregion

    #region hash

    /// <summary>
    /// 删除hash
    /// </summary>
    public const string HDel = "HDEL";

    /// <summary>
    /// hash存储
    /// </summary>
    public const string HSet = "HSET";

    /// <summary>
    /// hash是否存在
    /// </summary>
    public const string HExists = "HEXISTS";

    /// <summary>
    /// hash获取
    /// </summary>
    public const string HGet = "HGET";


    /// <summary>
    /// 获取所有的hash数据
    /// </summary>
    public const string HGetAll = "HGETALL";

    /// <summary>
    /// 递增
    /// </summary>
    public const string HIncrBy = "HINCRBYFLOAT";

    /// <summary>
    /// 获取hash的key信息
    /// </summary>
    public const string HKeys = "HKEYS";

    /// <summary>
    /// hash长度
    /// </summary>
    public const string HLen = "HLEN";


    /// <summary>
    /// 批量获取
    /// </summary>
    public const string HMGet = "HMGET";

    /// <summary>
    /// 返回hash的随机数据
    ///
    /// 当仅使用key参数调用时，从存储在 的哈希值中返回一个随机字段key。
    /// 如果提供的参数为正，则返回不同字段count的数组。数组的长度或者是散列的字段数 ( )，以较小者为准。countHLEN
    /// 如果用否定调用，行为会改变并且允许命令多次count返回相同的字段。在这种情况下，返回的字段数是指定的绝对值。count
    /// 可选WITHVALUES修饰符更改回复，使其包含随机选择的哈希字段的相应值。
    /// 6.2.0
    /// </summary>
    public const string HRandField = "HRANDFIELD";

    /// <summary>
    /// 获取hash的所有值
    /// </summary>
    public const string HVals = "HVALS";

    /// <summary>
    /// 获取hash 字段对应值的长度
    /// </summary>
    public const string HStrLen = "HSTRLEN";

    /// <summary>
    /// 设置hast的值 如果不存在的话 就添加
    /// </summary>
    public const string HSetNx = "HSETNX";

    /// <summary>
    /// 扫描
    /// </summary>
    public const string HScan = "HSCAN";

    #endregion

    #region set

    /// <summary>
    /// 添加
    /// 将指定的成员添加到存储在 的集合中key。已经是该集合成员的指定成员将被忽略。如果key不存在，则在添加指定成员之前创建一个新集。
    /// </summary>
    public const string SAdd = "SADD";

    /// <summary>
    /// 返回set里面集合的长度
    /// </summary>
    public const string SCard = "SCARD";

    /// <summary>
    /// 返回由第一个集合和所有后续集合之间的差异产生的集合成员。
    /// 类似 except
    /// </summary>
    public const string SDiff = "SDIFF";

    /// <summary>
    /// 此命令等于SDIFF，但不是返回结果集，而是存储在destination.
    /// 将except的差异值存到目标key
    /// </summary>
    public const string SDiffStore = "SDIFFSTORE";

    /// <summary>
    /// 返回由所有给定集的交集产生的集的成员。
    /// </summary>
    public const string SInter = "SINTER";

    /// <summary>
    /// 7.0.0
    /// 返回交集的值数量 类似与 SInter，SInter返回具体的交集数据，SInterCard返回数
    /// </summary>
    public const string SInterCard = "SINTERCARD";

    /// <summary>
    /// 此命令等于SINTER，但不是返回结果集，而是存储在destination.
    /// 如果destination已经存在，则将其覆盖
    /// </summary>
    public const string SInterStore = "SINTERSTORE";


    /// <summary>
    /// 判断set值是否存在
    /// </summary>
    public const string SisMember = "SISMEMBER";

    /// <summary>
    /// 获取set中所有的数据
    /// </summary>
    public const string SMembers = "SMEMBERS";

    /// <summary>
    /// 6.2.0
    /// 判断set中的值是否存在，存在就为1 不存在为0
    /// </summary>
    public const string SmisMember = "SMISMEMBER";

    /// <summary>
    /// 将一个集合的值 移动到指定的集合中
    /// </summary>
    public const string SMove = "SMOVE";

    /// <summary>
    /// 随机移除set中指定的值并返回 原子性
    /// </summary>
    public const string SPop = "SPOP";


    /// <summary>
    ///返回随机数据
    /// </summary>
    public const string SRandMember = "SRANDMEMBER";

    /// <summary>
    /// 移除成员信息
    /// </summary>
    public const string SRem = "SREM";

    /// <summary>
    /// set 扫描
    /// </summary>
    public const string SScan = "SSCAN";

    /// <summary>
    /// 返回由所有给定集合的并集生成的集合的成员。
    /// </summary>
    public const string SUnion = "SUNION";

    /// <summary>
    /// 将多个set的并集存储到一个新的set中
    /// </summary>
    public const string SUnionStore = "SUNIONSTORE";

    #endregion

    #region list

    /// <summary>
    /// 6.2.0
    /// 
    /// </summary>
    public const string BLMove = "BLMOVE";

    /// <summary>
    /// 往指定的元素 前或者后 插入元素
    /// </summary>
    public const string LInsert = "LINSERT";

    /// <summary>
    /// 往指定的下标处插入元素
    /// </summary>
    public const string LSet = "LSET";

    /// <summary>
    /// 将所有指定值插入存储在 的列表的尾部key。如果key不存在，则在执行推送操作之前将其创建为空列表。当key持有一个不是列表的值时，返回一个错误。
    /// </summary>
    public const string RPush = "RPUSH";

    /// <summary>
    ///将元素插入到列表的尾部，当key存在的时候才插入
    /// </summary>
    public const string RPushx = "RPUSHX";

    /// <summary>
    /// 将所有指定值插入存储在 的列表的头部key。如果key不存在，则在执行推送操作之前将其创建为空列表。当key持有一个不是列表的值时，返回一个错误。
    /// </summary>
    public const string LPush = "LPUSH";

    /// <summary>
    /// 将元素插入到列表的头部，当key存在的时候才插入
    /// </summary>
    public const string LPushx = "LPUSHX";

    /// <summary>
    /// 修剪现有列表，使其仅包含指定范围的指定元素。和start都是stop从零开始的索引，其中0是列表的第一个元素（头），1下一个元素等等。
    /// </summary>
    public const string LTrim = "LTRIM";

    /// <summary>
    /// 移除并返回存储在 的列表的最后一个元素key。
    /// 默认情况下，该命令从列表末尾弹出一个元素。当提供可选count参数时，回复将由最多count元素组成，具体取决于列表的长度。
    /// </summary>
    public const string RPop = "RPOP";

    /// <summary>
    /// 阻塞
    /// 移除并返回存储在 的列表的最后一个元素key。
    /// 默认情况下，该命令从列表末尾弹出一个元素
    /// </summary>
    public const string BRPop = "BRPOP";

    /// <summary>
    /// 移除并返回存储在 的列表的头部一个元素key。
    /// 默认情况下，该命令从列表头部弹出一个元素。当提供可选count参数时，回复将由最多count元素组成，具体取决于列表的长度。
    /// </summary>
    public const string LPop = "LPOP";

    /// <summary>
    /// 阻塞
    /// 移除并返回存储在 的列表的头部一个元素key。
    /// 默认情况下，该命令从列表头部弹出一个元素。
    /// </summary>
    public const string BLPop = "BLPOP";

    /// <summary>
    /// 从列表删除元素
    /// </summary>
    public const string LRem = "LREM";

    /// <summary>
    /// 查询列表元素
    /// </summary>
    public const string LRange = "LRANGE";

    /// <summary>
    /// 队列的长度
    /// </summary>
    public const string LLen = "LLEN";


    /// <summary>
    /// 返回指定下标的元素
    /// </summary>
    public const string LIndex = "LINDEX";

    /// <summary>
    /// 6.0.6
    /// 返回元素的下标位置
    /// </summary>
    public const string LPos = "LPOS";

    /// <summary>
    /// 7.0.0
    /// 从指定的多个元素中出列 指定数量的消息信息
    /// </summary>
    public const string LmPop = "LMPOP";

    /// <summary>
    /// 6.2.0
    ///将 source 消息从左或者右 移除，并存储到 destination 元素中的 left 或者right
    /// </summary>
    public const string LMove = "LMOVE";

    /// <summary>
    /// 7.0.0
    /// 
    /// </summary>
    public const string BlMPop = "BLMPOP";

    #endregion


    #region zset

    /// <summary>
    /// 插入数据
    /// </summary>
    public const string ZAdd = "ZADD";


    /// <summary>
    /// 扫描
    /// </summary>
    public const string ZScan = "ZSCAN";


    /// <summary>
    /// 返回元素中的个数
    /// </summary>
    public const string ZCard = "ZCARD";

    /// <summary>
    /// 返回区间范围中的元素数量
    /// </summary>
    public const string ZCount = "ZCOUNT";

    /// <summary>
    /// 返回由第一个集合和所有后续集合之间的差异产生的集合成员。
    /// 类似 except
    /// 6.2.0
    /// </summary>
    public const string ZDiff = "ZDIFF";

    /// <summary>
    /// 此命令等于ZDIFF，但不是返回结果集，而是存储在destination.
    /// 将except的差异值存到目标key
    /// 6.2.0
    /// </summary>
    public const string ZDiffStore = "ZDIFFSTORE";

    /// <summary>
    /// 递增zset元素中的score值
    /// </summary>
    public const string ZIncrBy = "ZINCRBY";

    /// <summary>
    /// 返回由所有给定集的交集产生的集的成员。
    /// 6.2.0
    /// </summary>
    public const string ZInter = "ZINTER";


    /// <summary>
    /// 此命令类似于ZINTER，但它不返回结果集，而是仅返回结果的基数。
    /// 7.0.0
    /// </summary>
    public const string ZInterCard = "ZINTERCARD";

    /// <summary>
    /// 此命令等于ZINTER，但不是返回结果集，而是存储在destination.
    /// 如果destination已经存在，则将其覆盖
    /// </summary>
    public const string ZInterStore = "ZINTERSTORE";

    /// <summary>
    /// 合并取并集
    /// </summary>
    public const string ZUnionStore = "ZUNIONSTORE";

    /// <summary>
    /// 合并取并集
    /// 6.2.0
    /// </summary>
    public const string ZUnion = "ZUNION";

    /// <summary>
    /// 取分数
    /// </summary>
    public const string ZScore = "ZSCORE";

    /// <summary>
    /// 当排序集中的所有元素都以相同的分数插入时，为了强制按字典顺序排序，此命令返回排序集中的元素数，其值介于key和min之间max。
    /// </summary>
    public const string ZLexCount = "ZLEXCOUNT";

    /// <summary>
    /// 从提供的键名列表中的第一个非空排序集中弹出一个或多个元素，即成员分数对。
    /// 7.0.0
    /// </summary>
    public const string ZMpop = "ZMPOP";


    /// <summary>
    /// BZMPOP是 的阻塞变体ZMPOP
    /// 7.0.0
    /// </summary>
    public const string BZMpop = "BZMPOP";

    #endregion
}