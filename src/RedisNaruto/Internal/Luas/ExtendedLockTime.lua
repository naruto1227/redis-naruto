---
--- Created by zhanghaibo.
--- DateTime: 2023/11/18 12:09
---延长锁的 锁定时间
local currentVal = redis.call("get",KEYS[1])
if(currentVal==false) then
    return -1
--验证值是否一致
elseif(currentVal==ARGV[1])then
    -- 当锁没有过期就设置毫秒的过期时间
    return	redis.call("PEXPIRE",KEYS[1],ARGV[2],"NX");
else
    return -1
end
