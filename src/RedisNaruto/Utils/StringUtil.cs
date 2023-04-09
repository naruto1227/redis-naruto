namespace RedisNaruto.Utils;

internal static class StringUtil
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullOrWhiteSpace(this string source)
    {
        return string.IsNullOrWhiteSpace(source);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static int ToInt(this string source)
    {
        int.TryParse(source, out var res);
        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static long ToLong(this string source)
    {
        long.TryParse(source, out var res);
        return res;
    }
}