using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class PathFinder : MonoBehaviour
{
    List<NavObstacle> navObstacles = new List<NavObstacle>();
    [SerializeField] Transform start;
    [SerializeField] Transform end;

    private void Start()
    {
        NavObstacle[] obj = FindObjectsOfType<NavObstacle>();
        navObstacles = obj.ToList();
    }

    public void Update()
    {
        Vector3[] path = FindPath(start.position, end.position);
        if (path == null) return;

        for (int i = 0; i < path.Length-1; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], Color.green);
        }
    }

    public Vector3[] FindPath(Vector3 _start, Vector3 _end)
    {
        Vector3[] tr;
        tr = new Vector3[2];
        tr[0] = _start;
        tr[1] = _end;
        //직선이동이 가능한 경우 즉시 반환
        if (CanMove(_start, _end)) return tr;

        //길찾기 시작




        return null;
    }

    bool CanMove(Vector3 _start, Vector3 _end)
    {
        Vector2 s = new Vector2(_start.x, _start.z);
        Vector2 e = new Vector2(_end.x, _end.z);
        for (int i = 0; i < navObstacles.Count; i++)
        {
            NavObstacle nav = navObstacles[i];
            for (int j = 0; j < nav.nodeCount-1; j++)
            {
                if (LineCollision(s, e, nav.GetNode(j).ToVector2(), nav.GetNode(j+1).ToVector2())) return false;
            }
            if (LineCollision(s, e, nav.GetNode(nav.nodeCount-1).ToVector2(), nav.GetNode(0).ToVector2())) return false;
        }
        return true;
    }

    private bool LineCollision(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        Debug.DrawLine(new Vector3(p1.x, 0, p1.y), new Vector3(p2.x, 0, p2.y));
        Debug.DrawLine(new Vector3(p1.x, 0, p1.y), new Vector3(p3.x, 0, p3.y));

        if (ccw(p1, p3, p4) * ccw(p2, p3, p4) >= 0) return false;
        if (ccw(p3, p1, p2) * ccw(p4, p1, p2) >= 0) return false;


        return true;
    }

    /// <summary>
    /// output : 1 (counter clockwise), 0 (collinear), -1 (clockwise)
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v3"></param>
    /// <returns></returns>
    int ccw(Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float cross_product = (v2.x - v1.x) * (v3.y - v1.y) - (v3.x - v1.x) * (v2.y - v1.y);

        if (cross_product > 0)
        {
            return 1;
        }
        else if (cross_product < 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}
class Node
{
    Node privewNode;
    Vector2 pos;
    public float dist => privewNode == null? 0:privewNode.dist + Vector2.Distance(pos, privewNode.pos);
}