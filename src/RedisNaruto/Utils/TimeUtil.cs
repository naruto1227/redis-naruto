namespace RedisNaruto.Utils;

internal static class TimeUtil
{
    /// <summary>
    /// 获取当前时间戳
    /// </summary>
    /// <returns></returns>
    public static long GetTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}