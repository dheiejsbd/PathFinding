using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPS
{
    [System.Flags]
    public enum GridLayer: byte
    {
        Terrain         = 1<<0,
        ObjLevel1       = 1<<1,
        ObjLevel2       = 1<<2,
        ObjLevel3       = 1<<3,
        ObjLevel4       = 1<<4,
        ObjLevel5       = 1<<5,
        Building        = 1<<6,
        MovingObs       = 1<<7,
    }
}
