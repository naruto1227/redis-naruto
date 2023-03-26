# RedisNaruto

#### Description
c#版本的redis客户端

## 已完成

- String
- Hash
- Set
- List
- PubSub
- Lua
- tran
- pipelining
- 哨兵
- 集群
### todo
- 余下命令
- 返回值的数据类型 需要返回的是二进制数据(如果写入的是文件的话，目前读取的都是字符串了)
- 增加针对于 参数或者 方法的 redis版本号标识
- 为每个数据类型增加对应的模型返回 取消字典的返回（zset）
- function