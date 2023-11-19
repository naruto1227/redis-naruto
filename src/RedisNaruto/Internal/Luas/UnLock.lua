---
--- Created by zhanghaibo.
--- DateTime: 2023/11/18 12:08
---取消锁 先验证值是否一致，一致的情况下代表的是由锁的使用者主动取消
if redis.call("get",KEYS[1]) == ARGV[1] then
    return redis.call("del",KEYS[1])
else
    return 0
end