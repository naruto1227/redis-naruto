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
- RESP2
- 分布式lock
### todo
- 为每个数据类型增加对应的模型返回 取消字典的返回（zset）
- function
- 客户端缓存
- RESP3
- 增加 简易版ioc实现，方便 使用者替换实现（暂时不考虑）