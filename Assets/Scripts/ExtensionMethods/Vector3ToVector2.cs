using UnityEditor;
using UnityEngine;

public static class Vector3ToVector2
{
    public static Vector2 ToVector2(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }
    public static Vector2 ToVector2(this Vector3Int vec)
    {
        return new Vector2(vec.x, vec.z);
    }
    public static Vector3 ToVector3(this Vector2 vec)
    {
        return new Vector3(vec.x,0, vec.y);
    }
    public static Vector3 ToVector3(this Vector2Int vec)
    {
        return new Vector3(vec.x,0, vec.y);
    }
}
