using UnityEditor;
using UnityEngine;

namespace JPS
{
    public class JPSNode
    {
        public Vector2Int pos;
        public bool moveAble;
        public bool destructibleMoveAble;
        //node 도달비용, 남은거리
        public int gCost, hCost;
        public int dCost = 0;
        public float fCost => gCost + hCost+dCost;

        public JPSNode parentNode;

        public JPSNode(int _x, int _y, bool _moveAble, bool _destructibleMoveAble)
        {
            pos = new Vector2Int(_x, _y);
            moveAble = _moveAble;
            gCost = int.MaxValue;
            destructibleMoveAble = _destructibleMoveAble;
        }
    }
}