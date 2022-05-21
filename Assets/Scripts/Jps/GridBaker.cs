using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JPS
{
    class GridBaker
    {
        List<Grid> grids = new List<Grid>();
        List<JPSObstacle> obstacles = new List<JPSObstacle>();

        public BitArray[] layer;

        int layerCount;
        int gridCellCount;

        public void Initialize(int _layerCount, int _gridCellCount)
        {
            layerCount = _layerCount;
            gridCellCount = _gridCellCount;

            BakeAll();
        }

        public void AddGrid(Grid _grid)
        {
            grids.Add(_grid);
            BakeAll();
        }
        public void AddGrids(Grid[] _grids)
        {
            grids.AddRange(_grids);
            BakeAll();
        }
        public void AddGrid(Vector2Int pos)
        {
            grids.Add(new Grid(pos));
            BakeAll();
        }
        public void AddGrid(Vector2Int[] pos)
        {
            for (int i = 0; i < pos.Length; i++)
            {
                grids.Add(new Grid(pos[i]));
            }
            BakeAll();
        }





        void BakeAll()
        {
            layer = new BitArray[layerCount];

            BakePlane();
        }

        void Bake(int _layerID)
        {

            //장애물 관련 코드 필요
        }

        public void BakePlane()
        {
            BitArray array = new BitArray(BitCount(), false);

            Vector2Int ru = RightUpCorner();
            Vector2Int ld = LeftDownCorner();

            int XcellCount = (ru.x - ld.x+1) * gridCellCount;

            foreach (var item in grids)
            {
                int x = (item.GridPos.x - ld.x) * gridCellCount;
                for (int bity = 0; bity < gridCellCount; bity++)
                {
                    int bitOffset = XcellCount * ((item.GridPos.y - ld.y) * gridCellCount + bity) + x;

                    for (int bitX = 0; bitX < gridCellCount; bitX++)
                    {

                        array.Set(bitOffset + bitX, true);
                    }
                }
            }


            layer[0] = array;
        }

        Vector2Int RightUpCorner()
        {
            Vector2Int Max = Vector2Int.zero;

            foreach (var item in grids)
            {
                Max.x = Mathf.Max(item.GridPos.x, Max.x);
                Max.y = Mathf.Max(item.GridPos.y, Max.y);
            }

            return Max;
        }
        Vector2Int LeftDownCorner()
        {
            Vector2Int Min = Vector2Int.zero;

            foreach (var item in grids)
            {
                Min.x = Mathf.Min(item.GridPos.x, Min.x);
                Min.y = Mathf.Min(item.GridPos.y, Min.y);
            }
            return Min;
        }

        int BitCount()
        {
            Vector2Int Max = Vector2Int.zero;
            Vector2Int Min = Vector2Int.zero;

            foreach (var item in grids)
            {
                Max.x = Mathf.Max(item.GridPos.x, Max.x);
                Max.y = Mathf.Max(item.GridPos.y, Max.y);

                Min.x = Mathf.Min(item.GridPos.x, Min.x);
                Min.y = Mathf.Min(item.GridPos.y, Min.y);
            }

            int x, y;

            x = (Max.x - Min.x)+1;
            y = (Max.y - Min.y)+1;

            return x * y * gridCellCount * gridCellCount;
        }


    }
}
