using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
public static class ListToGeoSphare
{
    public static List<Vector2> ToGeoSphare(this List<Vector2> list)
    {
        float offset = 0;
        if(list.Count %2 == 0)
        {
            offset = 360 / list.Count / 2;
        }
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = AngleToPos((float)360 / list.Count * i + offset+90);
        }
        return list;
    }
    static Vector2 AngleToPos(float angle)
    {
        var x = Mathf.Cos(angle * Mathf.Deg2Rad);
        var y = Mathf.Sin(angle * Mathf.Deg2Rad);
        return new Vector2(x, y);
    }
}