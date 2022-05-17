using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JPS
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] int gridCount;
        [SerializeField] int gridSize;
        [SerializeField] Vector3 gizmoSize;
        [SerializeField] List<JPSNode> nodes = new List<JPSNode>();

        [SerializeField] Transform startTr;
        [SerializeField] Transform endTr;

        // Update is called once per frame
        void Start()
        {
            InitializeGrid();
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                JPS jps = new JPS();
                Vector2 offset = new Vector2(gridSize / 2f, gridSize / 2f);
                var st = startTr.position.ToVector2() + offset;
                var et = endTr.position.ToVector2() + offset;

                Vector2Int p1 = new Vector2Int((int)(st.x / gridSize), (int)(st.y / gridSize));
                Vector2Int p2 = new Vector2Int((int)(et.x / gridSize), (int)(et.y / gridSize));

                var sn = nodes[p1.x + p1.y * gridCount];
                var en = nodes[p2.x + p2.y * gridCount];
                jps.FindPath(nodes, sn, en, gridCount);
            }
        }
        public void InitializeGrid()
        {
            for (int y = 0; y < gridCount; y++)
            {
                for (int x = 0; x < gridCount; x++)
                {
                    nodes.Add(new JPSNode(x, y, true));
                }
            }
        }



        private void OnDrawGizmos()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                Gizmos.color = Color.white;

                JPSNode node = nodes[i];
                //Vector3Int p1 = new Vector3Int(node.pos.x * gridSize, 0, node.pos.y * gridSize);
                //Vector3Int p2 = new Vector3Int(node.pos.x * gridSize + gridSize, 0, node.pos.y * gridSize + gridSize);
                Vector3 center = new Vector3(node.pos.x * gridSize + gridSize/2, 0, node.pos.y * gridSize + gridSize/2);

                Gizmos.DrawWireCube(center, new Vector3(gridSize, 0, gridSize));

                Gizmos.color = node.moveAble ? Color.green : Color.red;
                Gizmos.DrawCube(center, gizmoSize);
            }
        }

        public void Obs(Vector2 _p1, Vector2 _p2)
        {
            Vector2Int p1 = new Vector2Int((int)_p1.x / gridSize, (int)_p1.y / gridSize);
            Vector2Int p2 = new Vector2Int((int)_p2.x / gridSize, (int)_p2.y / gridSize);

            for (int x = p1.x; x <= p2.x; x++)
            {
                for (int y = p1.y; y <= p2.y; y++)
                {
                    nodes[y * gridCount + x].moveAble = false;
                }
            }
        }
    }
}
