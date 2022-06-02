using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JPS
{
    class GridBaker
    {
        List<Grid> grids = new List<Grid>();

        public BitArray[] layer;
        LayerMask[] layerMasks;

        int layerCount;
        int gridCellCount;
        float gridCellSize = 1;

        public BitArray GetMap(GridLayer _layer)
        {
            return layer[(int)_layer];
        }

        public void Initialize(int _layerCount, int _gridCellCount, LayerMask[] _layerMasks = null)
        {
            layerCount = _layerCount;
            layerMasks = _layerMasks;
            gridCellCount = _gridCellCount;
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
            Bake(1);
        }

        void Bake(int _layerID)
        {
            BitArray bit = new BitArray(BitCount(), true);

            Vector2Int max = RightUpCorner();
            Vector2Int min = LeftDownCorner();

            int xChunkCount = (max.x - min.x + 1);
            int yChunkCount = (max.y - min.y + 1);

            Vector3 celloffset = new Vector3(gridCellSize / 2f, 0, gridCellSize / 2f);


            for (int y = 0; y < yChunkCount * gridCellCount; y++)
            {
                for (int x = 0; x < xChunkCount * gridCellCount; x++)
                {
                    Vector3 chunkOffset = Vector3.zero;
                    if (min.x < 0) chunkOffset.x = min.x * gridCellCount * gridCellSize;
                    if (min.y < 0) chunkOffset.z = min.y * gridCellCount * gridCellSize;

                    int bitID = y * xChunkCount * gridCellCount + x;
                    bit.Set(bitID, !Physics.BoxCast(new Vector3(x * gridCellSize, -50, y * gridCellSize) + chunkOffset + celloffset,
                                                                             celloffset,
                                                                             Vector3.up,
                                                                             Quaternion.identity, 
                                                                             100, 
                                                                             layerMasks[_layerID-1]));
                }
            }

            layer[_layerID] = bit;
        }
        //청크 해금 여부
        public void BakePlane()
        {
            BitArray array = new BitArray(BitCount(), false);

            Vector2Int ru = RightUpCorner();
            Vector2Int ld = LeftDownCorner();

            int XcellCount = (ru.x - ld.x + 1) * gridCellCount;

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

        //가장 오른쪽 위 청크 좌표
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
        //가장 왼쪽 아래 청크 좌표
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

            foreach (var Chunk in grids)
            {
                Max.x = Mathf.Max(Chunk.GridPos.x, Max.x);
                Max.y = Mathf.Max(Chunk.GridPos.y, Max.y);

                Min.x = Mathf.Min(Chunk.GridPos.x, Min.x);
                Min.y = Mathf.Min(Chunk.GridPos.y, Min.y);
            }

            int x, y;

            x = (Max.x - Min.x) + 1;
            y = (Max.y - Min.y) + 1;

            return x * y * gridCellCount * gridCellCount;
        }

        int GetBitID(int _x, int _y)
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

            x = (Max.x - Min.x) + 1;

            return x * gridCellCount * _y + _x;
        }
    }
}
