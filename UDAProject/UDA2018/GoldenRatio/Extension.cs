using System.Collections.Generic;

namespace UDA2018.GoldenRatio
{
    public static class Extension
    {
        public static bool TryGet<T>(this List<T> list, int index, out T obj)
        {
            obj = default(T);
            if (index >= list.Count)
                return false;
            obj = list[index];
            return true;
        }
    }
}
