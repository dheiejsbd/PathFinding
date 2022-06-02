using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JPS
{

    public class PathFinder : MonoBehaviour
    {
        #region Setting
        int chunkSize;
        int gridCellCount;
        int layerCount;
        #endregion

        #region About
        bool showGraphs;
        #endregion


        JPS jps = new JPS();
        GridBaker gridBaker = new GridBaker();
        void Awake()
        {
            gridBaker.Initialize(layerCount, gridCellCount);

        }

        void AddObs()
        {

        }

        public void AddGrid(Vector2Int pos)
        {
            gridBaker.AddGrid(pos);
        }



        public void GetGridMap(GridLayer IDs = GridLayer.Chunk)
        {
            
            if ((IDs & GridLayer.Chunk) == GridLayer.Chunk)
            {

            }
        }

    }

}