# RedisNaruto

#### Description
c#版本的redis客户端（目前基于使用RESP2协议对接）

[码云地址](https://gitee.com/haiboi/redis-naruto)

[github](https://github.com/zhanghaiboshiwo/redis-naruto)

## 已完成
- Generic
- String
- Hash
- Set
- ZSet
- List
- Stream
- HyperLogLog
- PubSub
- Lua
- tran
- pipelining
- 哨兵
- 集群
- 使用System.IO.Pipelines替换底层消息处理,减少内存分配
- RESP2
### todo
- 增加针对于 参数或者 方法的 redis版本号标识
- 为每个数据类型增加对应的模型返回 取消字典的返回（zset）
- function
- 客户端缓存
- TaskCompletionSource 调整pipe
- RESP3