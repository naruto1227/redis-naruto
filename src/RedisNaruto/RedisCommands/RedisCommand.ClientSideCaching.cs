// using RedisNaruto.Models;
//
// namespace RedisNaruto.RedisCommands;
//
// public partial class RedisCommand : IRedisCommand
// {
//     /// <summary>
//     /// 启用客户端缓存
//     /// 当前方法需要在构建的时候就调用，因为需要在处理化连接池的时候redirect client id
//     /// </summary>
//     /// <exception cref="NotImplementedException"></exception>
//     public void UseClientSideCaching(ClientSideCachingOption option)
//     {
//         //todo 开启新的线程专门用于接收消息过期的key 然后清除缓存操作
//         /*
//          * https://redis.io/docs/latest/develop/use/client-side-caching/
//          * 客户端缓存 支持两种模式
//          * 模式1：跟踪模式下（TRACKING），服务端记住客户端访问了哪些key，然后当key更新的时候，发送消息给指定的客户端，但是这种模式需要消耗服务端的很多内存，并且也只会为了客户端中可能存在的缓存而发送可能的无效命令，因为客户端并不是所有访问的key都缓存的
//          *  example: 这个场景对于连接池的客户端，需要每次建立一个新的连接的时候，都需要开启跟踪，并将REDIRECT 到接收订阅消息的客户端
//          *      client 1 :订阅失效的消息
//          *          SUBSCRIBE __redis__:invalidate
//          *      client 2 ：开启跟踪 并将clientid 指向 client 1
//          *          CLIENT TRACKING on REDIRECT  client1Id
//          *          获取命令 使服务端记住了客户端访问了哪个key
//          *          get test 
//          *      client 3: 设置缓存 ，然后 客户端1 就获取到订阅的消息，客户端去清除缓存，然后 client2 又去 get test，这时候服务端重新记录了这个key 反复执行此操作
//          *          set test 11
//          *
//          *  上面模式下，服务端会缓存所有客户端的key,会导致产生很多无效的客户端消息和内存占用，而且并不是所有的key都需要缓存，白白的占用了服务端的内存，所以需要一种更优的方案
//          *      选择缓存模式：增加 OPTIN 参数,然后客户端需要缓存什么的时候，需要在读取命令之前先发送 CLIENT CACHING YES 告诉服务端下面的命令需要缓存
//          *      CLIENT TRACKING on REDIRECT  client1Id OPTIN
//          *      然后当客户端需要缓存的时候 先开启
//          *      CLIENT CACHING YES
//          * 
//          * 模式2；在广播模式下，服务端不会记住客户端访问了哪些key，这样的话服务端就没有内存的占用，相反通过客户端订阅了key的前缀 ，比如 user: 这样的话当匹配到这些key的前缀的时候,就会发送消息给客户端,客户端去自主的清除客户端缓存
//          *  example：客户端使用 BCAST 选项启用客户端缓存，使用 PREFIX 选项指定一个或多个前缀。例如: CLIENT TRACKING on REDIRECT 10 BCAST PREFIX object: PREFIX user: 。
//          *          如果根本没有指定前缀，则假定前缀为空字符串，因此客户端将收到每个被修改的密钥的无效消息。相反，如果使用一个或多个前缀，则只有与指定前缀之一匹配的密钥才会在无效消息中发送。
//          *          服务器不会在无效表中存储任何内容。相反，它使用不同的前缀表，其中每个前缀与一个客户端列表相关联
//          *          每次修改匹配任何前缀的键时，订阅该前缀的所有客户端都将收到无效消息。
//          */
//     }
// }