using System.Collections.Concurrent;

namespace RedisNaruto.Utils;

internal static class StreamUtil<T>
{
    private static ConcurrentDictionary<string, int> _index;

    static StreamUtil()
    {
        _index = new ConcurrentDictionary<string, int>();
    }

    /// <summary>
    /// 查找流对应值
    /// </summary>
    /// <param name="source"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static object FindStreamValue(List<object> source, string name)
    {
        if (_index.TryGetValue(name, out var i))
        {
            return source[i];
        }

        i = source.IndexOf(name);

        if (i != -1)
        {
            _index.TryAdd(name, i + 1);
            return source[i + 1];
        }

        _index.TryAdd(name, i);
        return null;
    }
}