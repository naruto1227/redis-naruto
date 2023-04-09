using RedisNaruto.Models;

namespace RedisNaruto.Utils;

public static class ListUtil
{
    public static Dictionary<string, RedisValue> ToDic(this List<object> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Execute();

        Dictionary<string, RedisValue> Execute()
        {
            //下标
            var i = 1;
            //上一个值
            var preName = "";
            var res = new Dictionary<string, RedisValue>();
            foreach (var item in source)
            {
                if (item is not RedisValue itemStr)
                {
                    continue;
                }

                //双数为值
                if (i % 2 == 0)
                {
                    res[preName] = itemStr;
                }
                //单数为key
                else
                {
                    preName = itemStr;
                    res[itemStr] = default;
                }

                i++;
            }

            return res;
        }
    }

    public static List<RedisValue> ToRedisValueList(this List<object> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Execute();

        List<RedisValue> Execute()
        {
            var res = new List<RedisValue>();
            foreach (var item in source)
            {
                if (item is not RedisValue itemStr)
                {
                    res.Add(RedisValue.Null());
                    continue;
                }

                res.Add(itemStr);
            }

            return res;
        }
    }

    public static Dictionary<string, RedisValue> ToDicObj(this List<object> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Execute();

        Dictionary<string, RedisValue> Execute()
        {
            //下标
            var i = 1;
            //上一个值
            var preName = "";
            var res = new Dictionary<string, RedisValue>();
            foreach (var item in source)
            {
                if (item is RedisValue redisValue)
                {
                    //双数为值
                    if (i % 2 == 0)
                    {
                        res[preName] = redisValue;
                    }
                    //单数为key
                    else
                    {
                        preName = redisValue;
                        res[preName] = default;
                    }
                    i++;
                }
            }

            return res;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="condition"></param>
    /// <param name="concat"></param>
    /// <returns></returns>
    public static IEnumerable<object> ConcatIf(this IEnumerable<object> source, bool condition,
        IEnumerable<object> concat)
    {
        return condition
            ? source.Concat(concat)
            : source;
    }
}