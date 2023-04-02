using RedisNaruto.Models;

namespace RedisNaruto.Utils;

internal static class EnumerableUtil
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

    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static Task<Dictionary<string, string>> ToDicAsync(this IAsyncEnumerator<string> source)
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
            while (await source.MoveNextAsync())
            {
                var item = source.Current;
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

    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<Dictionary<object, long>> ToZSetDicAsync(this IAsyncEnumerable<object> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return ExecuteAsync();

        async Task<Dictionary<object, long>> ExecuteAsync()
        {
            //下标
            var i = 1;
            //上一个值
            object preName = null;
            var res = new Dictionary<object, long>();
            await foreach (var item in source)
            {
                //双数为值
                if (i % 2 == 0)
                {
                    res[preName] = item.ToString().ToLong();
                }
                //单数为key
                else
                {
                    preName = item;
                    res[item] = 0;
                }

                i++;
            }

            return res;
        }
    }

    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<List<SortedSetModel>> ToZSetAsync(this IAsyncEnumerable<object> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return ExecuteAsync();

        async Task<List<SortedSetModel>> ExecuteAsync()
        {
            //下标
            var i = 1;
            var res = new List<SortedSetModel>();
            await foreach (var item in source)
            {
                //双数为值
                if (i % 2 == 0)
                {
                    res[i - 1 - 1].Score = item.ToString().ToLong();
                }
                //单数为key
                else
                {
                    res.Add(new SortedSetModel(item));
                }

                i++;
            }

            return res;
        }
    }

    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Dictionary<object, long> ToZSetDic(this IEnumerable<object> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Execute();

        Dictionary<object, long> Execute()
        {
            //下标
            var i = 1;
            //上一个值
            object preName = null;
            var res = new Dictionary<object, long>();
            foreach (var item in source)
            {
                //双数为值
                if (i % 2 == 0)
                {
                    res[preName] = item.ToString().ToLong();
                }
                //单数为key
                else
                {
                    preName = item;
                    res[item] = 0;
                }

                i++;
            }

            return res;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="condition"></param>
    /// <param name="data">数据</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static void IfAdd<T>(this List<T> source, bool condition, T data)
    {
        if (condition)
        {
            source.Add(data);
        }
    }

    /// <summary>
    /// 构建流消息内容
    /// </summary>
    /// <returns></returns>
    public static List<StreamModel> BuildStreamList(this List<object> list)
    {
        if (list is not {Count: > 0})
        {
            return default;
        }

        var resultList = new List<StreamModel>();

        foreach (var item in list)
        {
            if (item is not List<object> {Count: > 1} itemStreamList)
            {
                continue;
            }

            if (itemStreamList[1] is not List<object> {Count: > 0} itemStreamEntityList)
            {
                continue;
            }

            var info = new StreamModel(itemStreamList[0].ToString());
            info.StreamEntitys.AddRange(itemStreamEntityList.BuildStreamEntityModel());
            resultList.Add(info);
        }

        return resultList;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemStreamEntityList"></param>
    /// <returns></returns>
    public static List<StreamEntityModel> BuildStreamEntityModel(this List<object> itemStreamEntityList)
    {
        List<StreamEntityModel> streamEntityModels = new();
        foreach (var streamEntityList in itemStreamEntityList)
        {
            if (streamEntityList is not List<object> {Count: > 1} currentStreamEntityList)
            {
                continue;
            }

            // StreamEntityModel
            if (currentStreamEntityList[1] is not List<object> {Count: > 0} messageList)
            {
                continue;
            }

            var streamEntityModel =
                new StreamEntityModel(currentStreamEntityList[0].ToString(), messageList.ToDicObj());
            streamEntityModels.Add(streamEntityModel);
        }

        return streamEntityModels;
    }
}