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

    //수정
    //4각형 이상에서의 이동
    //도형의 점이 다른 도형에 겹쳐져 있을경우
    public void FixedUpdate()
    {
        ResetNode();
        Vector3[] path = FindPath(start.position, end.position);
        if (path == null) return;

        for (int i = 0; i < path.Length - 1; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], Color.blue);
        }


    }
    private void OnDrawGizmos()
    {

        for (int i = 0; i < navObstacles.Count; i++)
        {
            for (int k = 0; k < navObstacles[i].nodeCount; k++)
            {
                Handles.Label(navObstacles[i].Nodes[k].pos.ToVector3(), navObstacles[i].Nodes[k].dist.ToString() + "\n" + navObstacles[i].Nodes[k].pos + "\n" + navObstacles[i].Nodes[k].privewNode?.pos);
            }
        }
    }



    private void ResetNode()
    {
        for (int i = 0; i < navObstacles.Count; i++)
        {
            for (int k = 0; k < navObstacles[i].nodeCount; k++)
            {
                navObstacles[i].Nodes[k].privewNode = null;
            }
        }
    }
    public Vector3[] FindPath(Vector3 _start, Vector3 _end)
    {
        Vector3[] tr;
        tr = new Vector3[2];
        tr[0] = _start;
        tr[1] = _end;

        Node start = new Node(_start.ToVector2());
        Node end = new Node(_end.ToVector2());

        FindPath(start, end, -1, -1);
        if (end.privewNode == null) return null;

        List<Vector3> path = new List<Vector3>();

        while (true)
        {
            path.Insert(0, end.pos.ToVector3());
            if (end.privewNode == null) return path.ToArray();
            end = end.privewNode;
        }
    }

    void FindPath(Node node, Node Target, int navObstaclesIndex, int nodeIndex)
    {
        //도착점으로 이동가능한경우 return
        if (CanMove(node.pos, Target.pos))
        {
            if (Target.dist > NodeDist(node, Target) || Target.dist == -1)
            {
                Target.privewNode = node;
            }
            return;
        }

        //길찾기 시작
        for (int i = 0; i < navObstacles.Count; i++)
        {
            //같은 도형내 이동일 경우 좌우 Node로만 이동 가능
            if(i == navObstaclesIndex)
            {
                var navobs = navObstacles[i];
                int nextindex = navobs.nodeCount -1 == nodeIndex? 0 : nodeIndex+1;

                var next = navObstacles[i].Nodes[nextindex];
                if (CanMove(node.pos, next.pos) && (next.dist >= NodeDist(node, next) || next.dist == -1))
                {
                    if (Target.dist == -1 || Target.dist > NodeDist(node, next))
                    {
                        next.privewNode = node;
                        FindPath(next, Target, i, nextindex);
                    }
                }

                nextindex = nodeIndex == 0 ? navobs.nodeCount - 1 : nodeIndex - 1;
                next = navObstacles[i].Nodes[nextindex];
                if (CanMove(node.pos, next.pos) && (next.dist >= NodeDist(node, next) || next.dist == -1))
                {
                    if (Target.dist == -1 || Target.dist > NodeDist(node, next))
                    {
                        next.privewNode = node;
                        FindPath(next, Target, i, nextindex);
                    }
                }
            }
            else
            {
                for (int j = 0; j < navObstacles[i].nodeCount; j++)
                {
                    Node next = navObstacles[i].Nodes[j];
                    if (CanMove(node.pos, next.pos) && (next.dist >= NodeDist(node, next) || next.dist == -1))
                    {
                        if (Target.dist == -1 || Target.dist > NodeDist(node, next))
                        {
                            next.privewNode = node;
                            FindPath(next, Target, i, j);

                        }
                    }
                }
            }
        }
    }

    bool CanMove(Vector2 _start, Vector2 _end)
    {
        for (int i = 0; i < navObstacles.Count; i++)
        {
            NavObstacle nav = navObstacles[i];
            for (int j = 0; j < nav.nodeCount - 1; j++)
            {
                if (LineCollision(_start, _end, nav.GetGlobalNode(j).pos, nav.GetGlobalNode(j + 1).pos)) return false;
            }
            if (LineCollision(_start, _end, nav.GetGlobalNode(nav.nodeCount - 1).pos, nav.GetGlobalNode(0).pos)) return false;
        }
        return true;
    }
    float NodeDist(Node p1, Node p2)
    {
        return p1.dist + Vector2.Distance(p1.pos, p2.pos);
    }
    private bool LineCollision(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        Debug.DrawLine(new Vector3(p1.x, 0, p1.y), new Vector3(p2.x, 0, p2.y), Color.grey);
        Debug.DrawLine(new Vector3(p3.x, 1, p3.y), new Vector3(p4.x, 1, p4.y), Color.red);
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
[System.Serializable]
public class Node
{
    public Node()
    {

    }
    public Node(Vector2 pos)
    {
        this.pos = pos;
    }
    public Node privewNode;
    public Vector2 pos;
    public float dist => privewNode == null ? -1 : privewNode.dist + Vector2.Distance(pos, privewNode.pos);
}