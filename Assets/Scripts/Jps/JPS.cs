using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
namespace JPS
{
    public class JPS
    {
        //직선 이동 비용
        const int MOVE_STRAIGHT_COST = 10;
        //대각 이동 비용
        const int MOVE_DIAGONAL_COST = 14;

        Vector2Int mapSize;
        JPSNode startNode, endNode;

        BitArray map;
        List<JPSNode> mapData = new List<JPSNode>();
        List<JPSNode> openNodes = new List<JPSNode>();

        public void FindPath(BitArray map, JPSNode startNode, JPSNode endNode, Vector2Int mapSize)
        {
            Debug.DrawRay(new Vector3(startNode.pos.x,0, startNode.pos.y), Vector3.up, Color.white, 5);
            openNodes.Clear();


            this.mapSize = mapSize;
            this.map = map;
            
            this.mapData = new List<JPSNode>();
            
            
            startNode.gCost = 0;
            startNode.hCost = ManhattanHeuristic(startNode.pos, endNode.pos);
            this.endNode = endNode;
            openNodes.Add(startNode);

            while (openNodes.Count > 0)
            {
                JPSNode currentNode = GetLowetsFCost(openNodes);
                openNodes.Remove(currentNode);

                if(currentNode == endNode)
                {
                    Debug.Log("FindPath");

                    JPSNode node = endNode;
                    while (true)
                    {
                        if (node.parentNode == null) break;
                        var p1 = new Vector3(node.pos.x,0, node.pos.y);
                        node = node.parentNode;
                        var p2 = new Vector3(node.pos.x,0, node.pos.y);
                        Debug.DrawLine(p1, p2, Color.yellow, 100);
                    }
                    return;
                }

                if(currentNode.moveAble == true)
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

                if (currentNode.moveAble == false) break;

                if(currentNode == endNode)
                {
                    isFind = true; 
                    AddOpenList(currentNode, startNode);
                    break;
                }

                //오른쪽 아래 대각
                if(GetNode(x, y - 1).moveAble == false)
                {
                    if(GetNode(x+1, y-1).moveAble == true)
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
                if(GetNode(x, y + 1).moveAble == false)
                {
                    if(GetNode(x+1, y + 1).moveAble == true)
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

                if (currentNode.moveAble == false) break;

                if (currentNode == endNode)
                {
                    isFind = true;
                    AddOpenList(currentNode, startNode);

                    break;
                }

                //왼쪽 위 대각 
                if (GetNode(x, y + 1).moveAble == false)
                {
                    if (GetNode(x -1, y + 1).moveAble == true)
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
                if (GetNode(x, y - 1).moveAble == false)
                {
                    if (GetNode(x + 1, y - 1).moveAble == true)
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

                if (currentNode.moveAble == false) break;

                if (currentNode == endNode)
                {
                    isFind = true;
                    AddOpenList(currentNode, startNode);

                    break;
                }

                //오른쪽 아래 대각
                if (GetNode(x+1, y).moveAble == false)
                {
                    if (GetNode(x + 1, y-1).moveAble == true)
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
                if (GetNode(x -1 , y).moveAble == false)
                {
                    if (GetNode(x - 1, y - 1).moveAble == true)
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

            while (currentNode.moveAble == true)
            {
                //map size comp
                if (Comp(x, y+1) == false) break;

                currentNode = GetNode(x, ++y);

                if (currentNode.moveAble == false) break;

                if (currentNode == endNode)
                {
                    isFind = true;
                    AddOpenList(currentNode, startNode);
                    break;
                }

                //오른쪽 위 대각
                if (GetNode(x + 1, y).moveAble == false)
                {
                    if (GetNode(x + 1, y + 1).moveAble == true)
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
                if (GetNode(x - 1, y).moveAble == false)
                {
                    if (GetNode(x - 1, y + 1).moveAble == true)
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


            while (currentNode.moveAble == true)
            {
                currentNode.moveAble = false;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크
                if (Comp(x + 1, y + 1) == false)   // 탐색 가능 여부
                {
                    break;
                }



                currentNode = GetNode(++x, ++y); // 다음 노드로 이동

                if (currentNode.moveAble == false)
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
                    if (GetNode(x - 1, y).moveAble == false) // 왼쪽이 막힘
                    {
                        if (GetNode(x - 1, y + 1).moveAble == true) // 왼쪽 위가 안막힘
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
                    if (GetNode(x, y - 1).moveAble == false) // 왼쪽이 벽이고
                    {
                        if (GetNode(x + 1, y - 1).moveAble == true) // 왼쪽 아래가 막혀 있지 않으면
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


            while (currentNode.moveAble == true)
            {
                currentNode.moveAble = false;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크
                if (Comp(x + 1, y - 1) == false)   // 탐색 가능 여부
                {
                    break;
                }



                currentNode = GetNode(++x, --y); // 다음 노드로 이동

                if (currentNode.moveAble == false)
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
                    if (GetNode(x - 1, y).moveAble == false) // 왼쪽이 막힘
                    {
                        if (GetNode(x - 1, y - 1).moveAble == true) // 왼쪽 아래가 안막힘
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
                    if (GetNode(x, y + 1).moveAble == false) // 위가 벽이고
                    {
                        if (GetNode(x + 1, y - 1).moveAble == true) // 오른쪽 위 막혀 있지 않으면
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


            while (currentNode.moveAble == true)
            {
                currentNode.moveAble = false;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크
                if (Comp(x - 1, y - 1) == false)   // 탐색 가능 여부
                {
                    break;
                }



                currentNode = GetNode(--x, --y); // 다음 노드로 이동

                if (currentNode.moveAble == false)
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
                    if (GetNode(x + 1, y).moveAble == false) // 오른쪽이 막힘
                    {
                        if (GetNode(x + 1, y - 1).moveAble == true) // 오른쪽 아래가 안막힘
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
                    if (GetNode(x, y + 1).moveAble == false) // 위가 벽이고
                    {
                        if (GetNode(x - 1, y + 1).moveAble == true) // 왼쪽 위 막혀 있지 않으면
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


            while (currentNode.moveAble == true)
            {
                currentNode.moveAble = false;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크
                if (Comp(x - 1, y + 1) == false)   // 탐색 가능 여부
                {
                    break;
                }



                currentNode = GetNode(--x, ++y); // 다음 노드로 이동

                if (currentNode.moveAble == false)
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
                    if (GetNode(x + 1, y).moveAble == false) // 왼쪽이 막힘
                    {
                        if (GetNode(x + 1, y + 1).moveAble == true) // 왼쪽 아래가 안막힘
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
                    if (GetNode(x, y - 1).moveAble == false) // 위가 벽이고
                    {
                        if (GetNode(x - 1, y - 1).moveAble == true) // 오른쪽 위 막혀 있지 않으면
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

        JPSNode GetNode(int _x, int _y)
        {
            if (mapSize.x <= _x) return new JPSNode(_x, _y, false);
            if (mapSize.y <= _y) return new JPSNode(_x, _y, false);
            return mapData[_y * mapSize.x + _x];
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
            if (_x <= 0) return false;
            if (_y <= 0) return false;
            if (_x >= mapSize.x-1) return false;
            if (_y >= mapSize.y-1) return false;

            return true;
        }
        void AddOpenList(JPSNode _currentNode, JPSNode _parentNode)
        {
            Debug.LogWarning("openlist" + _currentNode.pos);
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
    }
}