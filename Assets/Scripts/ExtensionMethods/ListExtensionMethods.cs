using System.Collections.Generic;
using UnityEngine;
public static class ListExtensionMethods
{
    public static List<Node> Add(this List<Node> list, Vector2 value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].pos += value;
        }
        return list;
    }
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
    public static List<T> ToCount<T>(this List<T> list, int _count, System.Func<T> func)
    {
        if (list.Count < _count)
        {
            list = list.AddDefault(_count - list.Count, func);
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
    //반환값이 필요한 Event 는 Func<T>
    public static List<T> AddDefault<T>(this List<T> list, int _count, System.Func<T> func)
    {
        for (int i = 0; i < _count; i++)
        {
            list.Add(func());
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