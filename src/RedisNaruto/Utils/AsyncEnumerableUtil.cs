namespace RedisNaruto.Utils;

internal static class AsyncEnumerableUtil
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return ExecuteAsync();

        async Task<List<T>> ExecuteAsync()
        {
            var list = new List<T>();

            await foreach (var element in source)
            {
                list.Add(element);
            }

            return list;
        }
    }

    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<Dictionary<string, string>> ToDicAsync(this IAsyncEnumerable<string> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return ExecuteAsync();

        async Task<Dictionary<string, string>> ExecuteAsync()
        {
            //下标
            var i = 1;
            //上一个值
            var preName = "";
            var res = new Dictionary<string, string>();
            await foreach (var item in source)
            {
                //双数为值
                if (i % 2 == 0)
                {
                    res[preName] = item;
                }
                //单数为key
                else
                {
                    preName = item;
                    res[item] = "";
                }

                i++;
            }

            return res;
        }
    }
}