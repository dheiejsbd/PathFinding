using UnityEditor;
using UnityEngine;

namespace JPS
{
    [System.Serializable]
    public class JPSNode
    {
        public Vector2Int pos;
        public bool moveAble;
        //node 도달비용, 남은거리
        public int gCost, hCost;
        public float fCost => gCost + hCost;

        public JPSNode parentNode;

        public JPSNode(int _x, int _y, bool _moveAble)
        {
            pos = new Vector2Int(_x, _y);
            moveAble = _moveAble;
            gCost = int.MaxValue;
        }
    }
}