using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibJohn
{
    public struct LevelToken
    {
        public string WaterMark;
        public byte Version;
        public UInt16 Width;
        public UInt16 Height;
        public UInt64 TotalEntities;
        public WallToken[,] Walls;
        public EntityToken[] Entities;
    }

    public struct WallToken
    {
        public UInt16 WallType;
        public bool IsDoor;
        public byte LockType;
        public UInt16 Floor;
        public UInt16 Ceiling;
        public byte Zone;
        public string Message;
    }

    public struct EntityToken
    {
        public UInt16 EntityId;
        public UInt16 X;
        public UInt16 Y;
        public string Config;
    }
}
