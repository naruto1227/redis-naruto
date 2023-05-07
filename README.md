# RedisNaruto

#### Description
c#版本的redis客户端（使用RESP3协议对接，所以服务端最少需要6.0）

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
### todo
- 增加针对于 参数或者 方法的 redis版本号标识
- 为每个数据类型增加对应的模型返回 取消字典的返回（zset）
- function
- 使用System.IO.Pipelines替换底层消息处理
- 客户端缓存