using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibJohn
{
    internal unsafe struct RawLevelToken
    {
        public fixed byte WaterMark[3];
        public byte Version;
        public UInt16 Width;
        public UInt16 Height;
        public UInt64 TotalEntities;
        public UInt64 Walls;
        public UInt64 Entities;
        public fixed byte Reserved[24];
    }

    internal unsafe struct RawEntityToken
    {
        public UInt16 EntityId;
        public UInt16 X;
        public UInt16 Y;
        public fixed byte Reserved[10];
        public UInt64 Config;
    }

    internal unsafe struct RawWallToken
    {
        public UInt16 WallType;         // 0, 1
        public byte IsDoor;             // 2
        public byte LockType;           // 3
        public UInt16 Floor;            // 4, 5
        public UInt16 Ceiling;          // 6, 7
        public byte Zone;               // 8
        public fixed byte Reserved[7];  // 9 - 15
        public UInt64 Message;          // 16 - 23
    }
}
