using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class NavObstacle : MonoBehaviour
{
    public List<Vector2> Nodes = new List<Vector2>();
    public Shape shape;
    [Range(3, 100)]public int nodeCount = 3;
    public Vector2 center;
    public Vector2 size = Vector2.one;
    public void Update()
    {
        Nodes.ToCount(nodeCount)
             .ToGeoSphare();
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 pos1 = GetNode(0);
        Vector3 pos2 = Vector3.zero;
        for (int i = 0; i < nodeCount-1; i++)
        {
            pos2 = GetNode(i + 1);
            Gizmos.DrawLine(pos1, pos2);
            pos1 = pos2;
        }
        Gizmos.DrawLine(GetNode(0), pos2);
    }

    public Vector3 GetNode(int index)
    {
        var value = Nodes[index];
        value = GetValue(value);
        var v = new Vector3(value.x, 0, value.y) + transform.position;
        v.y = 0;
        return v;
    }

    Vector2 GetValue(Vector2 vec)
    {
        vec.x *= size.x;
        vec.y *= size.y;

        vec.x += center.x;
        vec.y += center.y;
        return vec;
    }

}


