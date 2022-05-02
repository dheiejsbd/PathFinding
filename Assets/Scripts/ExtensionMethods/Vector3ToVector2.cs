using UnityEditor;
using UnityEngine;

public static class Vector3ToVector2
{
    public static Vector2 ToVector2(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }
}
