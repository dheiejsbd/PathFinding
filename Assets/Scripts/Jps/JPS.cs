using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
namespace JPS
{
    public class JPS : MonoBehaviour
    {
        #if UNITY_EDITOR
        [MenuItem("GameObject/Custom GameObject/JPS", false, int.MaxValue)]
        static void CreateJPS(MenuCommand menuCommand)
        {
            if (Object.FindObjectOfType<JPS>() != null) return;
            GameObject go = new GameObject("JPS");
            go.AddComponent(typeof(JPS));
            // Hierachy 윈도우에서 어떤 오브젝트를 선택하여 생성시에는 그 오브젝트의 하위 계층으로 생성
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // 생성된 오브젝트를 Undo 시스템에 등록
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

            // 생성한 오브젝트를 선택
            Selection.activeObject = go;
        }
        #endif
        public static JPS instance;

        //직선 이동 비용
        const int MOVE_STRAIGHT_COST = 10;
        //대각 이동 비용
        const int MOVE_DIAGONAL_COST = 14;

        Vector2Int mapSize;
        Vector2Int MaxChunkPos = Vector2Int.zero;
        Vector2Int MinChunkPos = Vector2Int.zero;

        JPSNode startNode, endNode;

        BitArray map;
        List<JPSNode> nodes = new List<JPSNode>();
        List<JPSNode> openNodes = new List<JPSNode>();


        public GridBaker baker = new GridBaker();


        [SerializeField]int gridCellCount = 50;
        [SerializeField]int gridCellSize = 1;
        [SerializeField] LayerMask[] gridLayer = new LayerMask[((GridLayer[])System.Enum.GetValues(typeof(GridLayer))).Length];


        void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
            baker.Initialize(gridCellCount, gridCellSize, gridLayer);
        }


        #region Gridbaker
        public void AddGrid(Vector3 _pos)
        {
            AddGrid(GridLocalization(_pos));
        }
        public void AddGrid(Vector2Int _pos)
        {
            baker.AddGrid(new Grid(_pos));
            MaxChunkPos.x = Mathf.Max(MaxChunkPos.x, _pos.x);
            MaxChunkPos.y = Mathf.Max(MaxChunkPos.y, _pos.y);
            MinChunkPos.x = Mathf.Min(MinChunkPos.x, _pos.x);
            MinChunkPos.y = Mathf.Min(MinChunkPos.y, _pos.y);
            mapSize.x = MaxChunkPos.x - MinChunkPos.x + 1;
            mapSize.y = MaxChunkPos.y - MinChunkPos.y + 1;
        }
        #endregion


        #region PathFinding
        public Vector3[] FindPath(GridLayer _mapLayer, Vector3 _startPos, Vector3 _endPos)
        {
            var s = CellLocalization(_startPos);
            var e = CellLocalization(_endPos);

            return FindPath(_mapLayer, s, e);
        }
        public Vector3[] FindPath(GridLayer _mapLayer, Vector2Int _startPos, Vector2Int _endPos)
        {
            openNodes.Clear();


            this.map = baker.GetMap(_mapLayer);
            
            InitializeGrid();

            startNode = GetNode(_startPos.x, _startPos.y);
            endNode = GetNode(_endPos.x, _endPos.y);

            startNode.gCost = 0;
            startNode.hCost = ManhattanHeuristic(startNode.pos, endNode.pos);

            Debug.DrawRay(new Vector3(startNode.pos.x,0, startNode.pos.y), Vector3.up, Color.white, 5);
            Debug.DrawRay(new Vector3(endNode.pos.x,0, endNode.pos.y), Vector3.up, Color.white, 5);

            openNodes.Add(startNode);

            while (openNodes.Count > 0)
            {
                JPSNode currentNode = GetLowetsFCost(openNodes);
                openNodes.Remove(currentNode);

                if(currentNode == endNode)
                {
                    Debug.Log("FindPath");

                    JPSNode node = endNode;
                    List<Vector3> path = new List<Vector3>();
                    while (true)
                    {
                        path.Insert(0,node.pos.ToVector3());
                        if (node.parentNode == null) break;
                        var p1 = new Vector3(node.pos.x,0, node.pos.y);
                        node = node.parentNode;
                        var p2 = new Vector3(node.pos.x,0, node.pos.y);
                    }
                    return path.ToArray();
                }

                if(CanMove(currentNode) == true)
                {
                    Right(currentNode);
                    Left(currentNode);
                    Up(currentNode);
                    Down(currentNode);
                    RightUp(currentNode);
                    RightDown(currentNode);
                    LeftUp(currentNode);
                    LeftDown(currentNode);
                }

                currentNode.moveAble = false;
            }
            return null;
        }

        void InitializeGrid()
        {
            this.nodes = new List<JPSNode>();

            for (int y = MinChunkPos.y * gridCellCount; y < (MaxChunkPos.y+ 1) * gridCellCount; y++)
            {
                for (int x = MinChunkPos.x * gridCellCount; x < (MaxChunkPos.x+1) * gridCellCount; x++)
                {
                    nodes.Add(new JPSNode(x, y, true));
                }
            }
        }
        Vector2Int GridLocalization(Vector3 _origin)
        {
            var localization = _origin;

            localization.x /= gridCellSize * gridCellCount;
            localization.z /= gridCellSize * gridCellCount;

            if (_origin.x < 0) localization.x -= 1;
            if (_origin.z < 0) localization.z -= 1;

            return new Vector2Int((int)localization.x, (int)localization.z);
        }
        Vector2Int CellLocalization(Vector3 _origin)
        {
            var localization = _origin;
            localization.x /= gridCellSize;
            localization.z /= gridCellSize;

            if (_origin.x < 0) localization.x -= 1;
            if (_origin.z < 0) localization.y -= 1;

            return new Vector2Int((int)localization.x, (int)localization.z);
        }
        

        #region Move
        bool Right(JPSNode _startNode, bool _moveAble = false)
        {
            int x = _startNode.pos.x;
            int y = _startNode.pos.y;

            JPSNode startNode = _startNode;
            JPSNode currentNode = startNode;

            bool isFind = false;

            while (currentNode.moveAble == true)
            {
                //map size comp
                if (Comp(x+1, y) == false) break;

                currentNode = GetNode(++x, y);

                if (CanMove(currentNode) == false) break;

                if(currentNode == endNode)
                {
                    isFind = true; 
                    AddOpenList(currentNode, startNode);
                    break;
                }

                //오른쪽 아래 대각
                if(CanMove(GetNode(x, y - 1)) == false)
                {
                    if(CanMove(GetNode(x+1, y-1)) == true)
                    {
                        if(_moveAble == false)
                        {
                            AddOpenList(currentNode, startNode);
                        }
                        isFind = true;
                        break;
                    }
                }
                //오른쪽 위 대각 
                if(CanMove(GetNode(x, y + 1)) == false)
                {
                    if(CanMove(GetNode(x+1, y + 1)) == true)
                    {
                        if(_moveAble == false)
                        {
                            AddOpenList(currentNode, startNode);
                        }
                        isFind = true;
                        break;
                    }
                }
            }

            return isFind;
        }
        bool Left(JPSNode _startNode, bool _moveAble = false)
        {
            int x = _startNode.pos.x;
            int y = _startNode.pos.y;

            JPSNode startNode = _startNode;
            JPSNode currentNode = startNode;

            bool isFind = false;

            while (currentNode.moveAble == true)
            {
                //map size comp
                if (Comp(x-1, y) == false) break;

                currentNode = GetNode(--x, y);

                if (CanMove(currentNode) == false) break;

                if (currentNode == endNode)
                {
                    isFind = true;
                    AddOpenList(currentNode, startNode);

                    break;
                }

                //왼쪽 위 대각 
                if (CanMove(GetNode(x, y + 1)) == false)
                {
                    if (CanMove(GetNode(x -1, y + 1)) == true)
                    {
                        if (_moveAble == false)
                        {
                            AddOpenList(currentNode, startNode);
                        }
                        isFind = true;
                        break;
                    }
                }
                //왼쪽 아래 대각
                if (CanMove(GetNode(x, y - 1)) == false)
                {
                    if (CanMove(GetNode(x + 1, y - 1)) == true)
                    {
                        if (_moveAble == false)
                        {
                            AddOpenList(currentNode, startNode);
                        }
                        isFind = true;
                        break;
                    }
                }
            }

            return isFind;
        }
        bool Down(JPSNode _startNode, bool _moveAble = false)
        {
            int x = _startNode.pos.x;
            int y = _startNode.pos.y;

            JPSNode startNode = _startNode;
            JPSNode currentNode = startNode;

            bool isFind = false;

            while (currentNode.moveAble == true)
            {
                //map size comp
                if (Comp(x, y -1) == false) break;

                currentNode = GetNode(x, --y);

                if (CanMove(currentNode) == false) break;

                if (currentNode == endNode)
                {
                    isFind = true;
                    AddOpenList(currentNode, startNode);

                    break;
                }

                //오른쪽 아래 대각
                if (CanMove(GetNode(x+1, y)) == false)
                {
                    if (CanMove(GetNode(x + 1, y-1)) == true)
                    {
                        if (_moveAble == false)
                        {
                            AddOpenList(currentNode, startNode);
                        }
                        isFind = true;
                        break;
                    }
                }
                //왼쪽 아래 대각 
                if (CanMove(GetNode(x -1 , y)) == false)
                {
                    if (CanMove(GetNode(x - 1, y - 1)) == true)
                    {
                        if (_moveAble == false)
                        {
                            AddOpenList(currentNode, startNode);
                        }
                        isFind = true;
                        break;
                    }
                }
            }

            return isFind;
        }
        bool Up(JPSNode _startNode, bool _moveAble = false)
        {
            int x = _startNode.pos.x;
            int y = _startNode.pos.y;

            JPSNode startNode = _startNode;
            JPSNode currentNode = startNode;

            bool isFind = false;

            while (CanMove(currentNode) == true)
            {
                //map size comp
                if (Comp(x, y+1) == false) break;

                currentNode = GetNode(x, ++y);

                if (CanMove(currentNode) == false) break;

                if (currentNode == endNode)
                {
                    isFind = true;
                    AddOpenList(currentNode, startNode);
                    break;
                }

                //오른쪽 위 대각
                if (CanMove(GetNode(x + 1, y)) == false)
                {
                    if (CanMove(GetNode(x + 1, y + 1)) == true)
                    {
                        if (_moveAble == false)
                        {
                            AddOpenList(currentNode, startNode);
                        }
                        isFind = true;
                        break;
                    }
                }
                //왼쪽 위 대각 
                if (CanMove(GetNode(x - 1, y)) == false)
                {
                    if (CanMove(GetNode(x - 1, y + 1)) == true)
                    {
                        if (_moveAble == false)
                        {
                            AddOpenList(currentNode, startNode);
                        }
                        isFind = true;
                        break;
                    }
                }
            }

            return isFind;
        }


        //대각
        void RightUp(JPSNode _startNode)
        {
            int x = _startNode.pos.x;
            int y = _startNode.pos.y;

            JPSNode startNode = _startNode;
            JPSNode currentNode = startNode;


            while (CanMove(currentNode) == true)
            {
                currentNode.moveAble = false;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크
                if (Comp(x + 1, y + 1) == false)   // 탐색 가능 여부
                {
                    break;
                }



                currentNode = GetNode(++x, ++y); // 다음 노드로 이동

                if (CanMove(currentNode) == false)
                {
                    break;
                }

                if (currentNode == this.endNode)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                #endregion

                #region 왼쪽이 막혀 있으면서 왼쪽 위가 막혀있지 않은 경우

                if (y + 1 < mapSize.y && x > 0)
                {
                    if (CanMove(GetNode(x - 1, y)) == false) // 왼쪽이 막힘
                    {
                        if (CanMove(GetNode(x - 1, y + 1)) == true) // 왼쪽 위가 안막힘
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion

                #region 아래가 막혀 있으면서 오른쪽 아래가 안막혔으면

                if (y > 0 && x + 1 < mapSize.x)
                {
                    if (CanMove(GetNode(x, y - 1)) == false) // 왼쪽이 벽이고
                    {
                        if (CanMove(GetNode(x + 1, y - 1)) == true) // 왼쪽 아래가 막혀 있지 않으면
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }


                #endregion

                if (Right(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                if (Up(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }
            }

            _startNode.moveAble = true;
        }
        void RightDown(JPSNode _startNode)
        {
            int x = _startNode.pos.x;
            int y = _startNode.pos.y;

            JPSNode startNode = _startNode;
            JPSNode currentNode = startNode;


            while (CanMove(currentNode) == true)
            {
                currentNode.moveAble = false;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크
                if (Comp(x + 1, y - 1) == false)   // 탐색 가능 여부
                {
                    break;
                }



                currentNode = GetNode(++x, --y); // 다음 노드로 이동

                if (CanMove(currentNode) == false)
                {
                    break;
                }

                if (currentNode == this.endNode)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                #endregion

                #region 왼쪽이 막혀 있으면서 왼쪽 아래가 막혀있지 않은 경우

                if (y + 1 < mapSize.y && x > 0)
                {
                    if (CanMove(GetNode(x - 1, y)) == false) // 왼쪽이 막힘
                    {
                        if (CanMove(GetNode(x - 1, y - 1)) == true) // 왼쪽 아래가 안막힘
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion

                #region 위가 막혀 있으면서 오른쪽 위가 안막혔으면

                if (y > 0 && x + 1 < mapSize.x)
                {
                    if (CanMove(GetNode(x, y + 1)) == false) // 위가 벽이고
                    {
                        if (CanMove(GetNode(x + 1, y - 1)) == true) // 오른쪽 위 막혀 있지 않으면
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }


                #endregion

                if (Right(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                if (Down(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }
            }

            _startNode.moveAble = true;
        }

        void LeftDown(JPSNode _startNode)
        {
            int x = _startNode.pos.x;
            int y = _startNode.pos.y;

            JPSNode startNode = _startNode;
            JPSNode currentNode = startNode;


            while (CanMove(currentNode) == true)
            {
                currentNode.moveAble = false;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크
                if (Comp(x - 1, y - 1) == false)   // 탐색 가능 여부
                {
                    break;
                }



                currentNode = GetNode(--x, --y); // 다음 노드로 이동

                if (CanMove(currentNode) == false)
                {
                    break;
                }

                if (currentNode == this.endNode)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                #endregion

                #region 오른쪽이 막혀 있으면서 오른쪽 아래가 막혀있지 않은 경우

                if (y + 1 < mapSize.y && x > 0)
                {
                    if (CanMove(GetNode(x + 1, y)) == false) // 오른쪽이 막힘
                    {
                        if (CanMove(GetNode(x + 1, y - 1)) == true) // 오른쪽 아래가 안막힘
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion

                #region 위가 막혀 있으면서 왼쪽 위가 안막혔으면

                if (y > 0 && x + 1 < mapSize.x)
                {
                    if (CanMove(GetNode(x, y + 1)) == false) // 위가 벽이고
                    {
                        if (CanMove(GetNode(x - 1, y + 1)) == true) // 왼쪽 위 막혀 있지 않으면
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }


                #endregion

                if (Left(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                if (Down(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }
            }

            _startNode.moveAble = true;
        }
        void LeftUp(JPSNode _startNode)
        {
            int x = _startNode.pos.x;
            int y = _startNode.pos.y;

            JPSNode startNode = _startNode;
            JPSNode currentNode = startNode;


            while (CanMove(currentNode) == true)
            {
                currentNode.moveAble = false;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크
                if (Comp(x - 1, y + 1) == false)   // 탐색 가능 여부
                {
                    break;
                }



                currentNode = GetNode(--x, ++y); // 다음 노드로 이동

                if (CanMove(currentNode) == false)
                {
                    break;
                }

                if (currentNode == this.endNode)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                #endregion

                #region 오른쪽이 막혀 있으면서 오른쪽 위가 막혀있지 않은 경우

                if (y + 1 < mapSize.y && x > 0)
                {
                    if (CanMove(GetNode(x + 1, y)) == false) // 왼쪽이 막힘
                    {
                        if (CanMove(GetNode(x + 1, y + 1)) == true) // 왼쪽 아래가 안막힘
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion

                #region 아래가 막혀 있으면서 왼쪽 아래가 안막혔으면

                if (y > 0 && x + 1 < mapSize.x)
                {
                    if (CanMove(GetNode(x, y - 1)) == false) // 위가 벽이고
                    {
                        if (CanMove(GetNode(x - 1, y - 1)) == true) // 오른쪽 위 막혀 있지 않으면
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }


                #endregion

                if (Left(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                if (Up(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }
            }

            _startNode.moveAble = true;
        }


        #endregion

        bool CanMove(JPSNode _node)
        {
            return _node.moveAble && Getbit(_node.pos);
        }

        JPSNode GetNode(int _x, int _y)
        {
            _x -= MinChunkPos.x * gridCellCount;
            _y -= MinChunkPos.y * gridCellCount;
            if (mapSize.x * gridCellCount <= _x) return new JPSNode(_x, _y, false);
            if (mapSize.y * gridCellCount <= _y) return new JPSNode(_x, _y, false);
            return nodes[_y * mapSize.x * gridCellCount + _x];
        }
        bool Getbit(Vector2Int _pos)
        {
            _pos -= MinChunkPos * gridCellCount;
            int ID = _pos.x + _pos.y * gridCellCount*mapSize.x;
            
            
            return map[ID];
        }



        JPSNode GetLowetsFCost(List<JPSNode> _openList)
        {
            JPSNode node = _openList[0];
            for (int i = 1; i < _openList.Count; i++)
            {
                if (node.fCost > _openList[i].fCost) node = _openList[i];
            }
            return node;
        }

        bool Comp(int _x, int _y)
        {
            if (_x <= MinChunkPos.x * gridCellCount) return false;
            if (_y <= MinChunkPos.y * gridCellCount) return false;
            if (_x >= (MaxChunkPos.x+1) * gridCellCount) return false;
            if (_y >= (MaxChunkPos.y+1) * gridCellCount) return false;

            return true;
        }
        void AddOpenList(JPSNode _currentNode, JPSNode _parentNode)
        {
            int nextCost = _parentNode.gCost + ManhattanHeuristic(_parentNode.pos, _currentNode.pos);
            if (nextCost < _currentNode.gCost)
            {
                _currentNode.parentNode = _parentNode;
                _currentNode.gCost = nextCost;
                _currentNode.hCost = ManhattanHeuristic(_currentNode.pos, endNode.pos);
                openNodes.Add(_currentNode);
            }
        }

        //대각선으로 이동하는 칸수: Mathf.Min(x, y)
        //직선이동 칸 수 reming
        /// <summary>
        /// 도착노드까지의 길이
        /// </summary>
        /// <param name="_currPosition"></param>
        /// <param name="_endPosition"></param>
        /// <returns></returns>
        private int ManhattanHeuristic(Vector2Int _currPosition, Vector2Int _endPosition)
        {
            int x = Mathf.Abs(_currPosition.x - _endPosition.x);
            int y = Mathf.Abs(_currPosition.y - _endPosition.y);
            int reming = Mathf.Abs(x - y);

            return MOVE_DIAGONAL_COST * Mathf.Min(x, y) + MOVE_STRAIGHT_COST * reming;
        }
        #endregion


    }
}