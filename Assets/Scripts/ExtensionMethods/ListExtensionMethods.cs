using System.Collections.Generic;
using UnityEngine;
public static class ListExtensionMethods
{
    public static List<T> ToCount<T>(this List<T> list, int _count)
    {
        if (list.Count < _count)
        {
            list = list.AddDefault(_count - list.Count);
        }
        else if (list.Count > _count)
        {
            list.RemoveBack(list.Count - _count);
        }
        return list;
    }
    public static List<T> AddDefault<T>(this List<T> list, int _count)
    {
        for (int i = 0; i < _count; i++)
        {
            list.Add(default(T));
        }
        return list;
    }
    public static List<T> RemoveBack<T>(this List<T> list, int _count)
    {
        if (_count < 1) Debug.LogError("");
        if (list.Count < _count) Debug.LogError("");

        list.RemoveRange(list.Count - _count, _count);
        return list;
    }
}