using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
namespace JPS
{
    class Test : MonoBehaviour
    {
        GridBaker baker = new GridBaker();

        public Transform tr;
        int Xcount = 1;
        int Ycount = 1;

        Vector2Int Max;
        Vector2Int Min;
        public LayerMask[] layermask;
        private void Start()
        {
            baker.Initialize(2, 50, layermask);
            baker.AddGrid(new Grid(new Vector2Int(0, 0)));
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Vector2Int pos = new Vector2Int((int)(tr.position.x / 50),(int)(tr.position.z / 50));
                if (tr.position.x < 0) pos.x -= 1;
                if (tr.position.z < 0) pos.y -= 1;
                baker.AddGrid(new Grid(pos));
                baker.BakePlane();
                Max.x = Mathf.Max(Max.x, pos.x);
                Max.y = Mathf.Max(Max.y, pos.y);
                Min.x = Mathf.Min(Min.x, pos.x);
                Min.y = Mathf.Min(Min.y, pos.y);
                Xcount = Max.x - Min.x + 1;
                Ycount = Max.y - Min.y + 1;
            }
        }

        public void OnDrawGizmos()
        {
            Vector3Int pos = new Vector3Int((int)(tr.position.x / 50)*50, 0, (int)(tr.position.z/50)*50);
            if (tr.position.x < 0) pos.x -= 50;
            if (tr.position.z < 0) pos.z -= 50;
            Gizmos.DrawWireCube(pos + new Vector3(25,0,25), new Vector3(50, 0, 50));

            BitArray bit = baker?.layer[1];
            for (int y = 0; y < 50 * Ycount; y++)
            {
                for (int x = 0; x < 50 * Xcount; x++)
                {
                    Vector3Int vec = Vector3Int.zero;
                    if (Min.x < 0) vec.x = Min.x * 50;
                    if (Min.y < 0) vec.z = Min.y * 50;
                    Debug.DrawRay(new Vector3(x , 0, y )+vec, Vector3.up, bit.Get(y * Xcount * 50 + x) ? Color.green : Color.red);
                }
            }
        }
    }
}
