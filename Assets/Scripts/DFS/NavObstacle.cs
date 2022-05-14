using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class NavObstacle : MonoBehaviour
{
    public List<Node> Nodes = new List<Node>();
    public Shape shape;
    [Range(3, 100)]public int nodeCount = 3;
    public Vector2 center;
    public Vector2 size = Vector2.one;
    public void Update()
    {
        Nodes.ToCount(nodeCount, () => new Node())
             .ToGeoSphare()
             .Add(transform.position.ToVector2());
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 pos1 = GetGlobalNodeVector(0);
        Vector3 pos2 = Vector3.zero;
        for (int i = 0; i < nodeCount-1; i++)
        {
            pos2 = GetGlobalNodeVector(i+1);
            Gizmos.DrawLine(pos1, pos2);
            pos1 = pos2;
        }
        Gizmos.DrawLine(GetGlobalNodeVector(0), pos2);
    }

    public Vector3 GetGlobalNodeVector(int index)
    {
        var value = Nodes[index].pos - transform.position.ToVector2();
        value = GetValue(value) + transform.position.ToVector2();
        var v = new Vector3(value.x, 0, value.y);
        return v;
    }

    public Node GetGlobalNode(int index)
    {
        var value = Nodes[index];
        value.pos = GetValue(value.pos);
        return value;
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


