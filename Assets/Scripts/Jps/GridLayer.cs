using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPS
{
    public enum GridLayer: uint
    {
        Chunk           = 0x00000000,
        Terrain         = 0x00000001,
        ObjLevel1       = 0x00000002,
        ObjLevel2       = 0x00000004,
        ObjLevel3       = 0x00000008,
        ObjLevel4       = 0x00000010,
        ObjLevel5       = 0x00000020,
        Building        = 0x00000040,
        MovingObs       = 0x00000080,
    }
}
