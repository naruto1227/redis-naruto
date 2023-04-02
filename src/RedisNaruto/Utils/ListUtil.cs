namespace RedisNaruto.Utils;

public static class ListUtil
{
    public static Dictionary<string, string> ToDic(this List<object> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Execute();

        Dictionary<string, string> Execute()
        {
            //下标
            var i = 1;
            //上一个值
            var preName = "";
            var res = new Dictionary<string, string>();
            foreach (var item in source)
            {
                var itemStr = item.ToString();
                //双数为值
                if (i % 2 == 0)
                {
                    res[preName] = itemStr;
                }
                //单数为key
                else
                {
                    preName = itemStr;
                    res[itemStr] = "";
                }

                i++;
            }

            return res;
        }
    }

    public static Dictionary<string, object> ToDicObj(this List<object> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Execute();

        Dictionary<string, object> Execute()
        {
            //下标
            var i = 1;
            //上一个值
            var preName = "";
            var res = new Dictionary<string, object>();
            foreach (var item in source)
            {
                //双数为值
                if (i % 2 == 0)
                {
                    res[preName] = item;
                }
                //单数为key
                else
                {
                    preName = item.ToString();
                    res[preName] = null;
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