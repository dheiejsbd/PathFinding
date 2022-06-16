using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JPS
{

    public class PathFinder : MonoBehaviour
    {
        [SerializeField] float stopDistance = 0.5f;
        [SerializeField] [EnumFlags] GridLayer gridLayer;

        public Transform target;
        Vector3 targetPos;
        Vector3[] path;
        int pathIndex;

        private void Update()
        {
            if (path == null) return;
            if (pathIndex >= path.Length) return;
            pathChack();
        }
        private void FixedUpdate()
        {
            if (target == null || target.position == targetPos) return;
            targetPos = target.position;
            path = JPS.instance.FindPath(gridLayer, transform.position, targetPos);
            if (path == null) pathIndex = int.MaxValue;
            pathIndex = 0;
        }

        void pathChack()
        {
            if (Vector3.Distance(transform.position, path[pathIndex]) <= stopDistance)
            {
                pathIndex++;
                if (pathIndex == path.Length)
                {
                    Debug.Log("JPS - " + gameObject + " : reach the target");
                }
            }

        }
        public Vector3 MoveVector()
        {
            return (path[pathIndex] - transform.position).normalized;
        }

        //circle 래스터화
        void drawCircle(int r)
        {
            List<Vector2Int> arr = new List<Vector2Int>();
            int x, y;
            int p;

            x = 0;
            y = r;
            p = 1 - r;

            arr.Add(new Vector2Int(y,x));
            arr.Add(new Vector2Int(-y,x));
            arr.Add(new Vector2Int(x,y));
            arr.Add(new Vector2Int(x,-y));

            ++x;
            while (x < y)
            {
                if (p < 0)
                {
                    p += x + x + 1;
                }
                else
                {
                    p += x + x - y - y + 1;
                    --y;
                }
                arr.Add(new Vector2Int(y, x));
                arr.Add(new Vector2Int(-y, x));
                arr.Add(new Vector2Int(y, -x));
                arr.Add(new Vector2Int(-y, -x));
                arr.Add(new Vector2Int(x, y));
                arr.Add(new Vector2Int(-x, y));
                arr.Add(new Vector2Int(x, -y));
                arr.Add(new Vector2Int(-x, -y));
                ++x;
            }

        }

        private void OnDrawGizmos()
        {
            if (path == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(path[0], transform.position);
            for (int i = 1; i < path.Length; i++)
            {
                Gizmos.DrawLine(path[i], path[i - 1]);
            }
        }
    }

}