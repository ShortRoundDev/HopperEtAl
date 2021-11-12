using LibJohn.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace LibJohn
{
    public class TileMap
    {
        private RawLevelToken _RawLevel { get; set; }
        public LevelToken Level { get; set;}

        private RawWallToken[] _RawWalls { get; set; }
        public WallToken[] Walls { get; set; }

        private RawEntityToken[] _RawEntities { get; set; }
        public EntityToken[] Entities { get; set; }

        public TileMap()
        {
        }

        public void LoadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException();
            }
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            var data = File.ReadAllBytes(filePath);
            if(data.Length < 3)
            {
                throw new EmptyFileException();
            }

            if(data[0] != 'H' && data[1] != 'A' && data[2] != 'M')
            {
                throw new BadFileFormatException();
            }
            Deserialize(data);
            Convert(data);
        }

        public unsafe void Deserialize(byte[] data)
        {
            RawLevelToken levelHeader = new RawLevelToken();
            //levelheader
            levelHeader.WaterMark[0] = data[0];
            levelHeader.WaterMark[1] = data[1];
            levelHeader.WaterMark[2] = data[2];

            levelHeader.Version = data[3];
            levelHeader.Width = BitConverter.ToUInt16(data, 4);
            levelHeader.Height = BitConverter.ToUInt16(data, 6);
            levelHeader.TotalEntities = BitConverter.ToUInt64(data, 8);
            levelHeader.Walls = BitConverter.ToUInt64(data, 16);
            levelHeader.Entities = BitConverter.ToUInt64(data, 24);

            _RawLevel = levelHeader;

            _RawEntities = new RawEntityToken[_RawLevel.TotalEntities];
            DeserializeEntities(_RawLevel.TotalEntities, data, _RawLevel.Entities);

            _RawWalls = new RawWallToken[_RawLevel.Width * _RawLevel.Height];
            DeserializeWalls(_RawLevel.Width, _RawLevel.Height, data, _RawLevel.Walls);
        }
        private unsafe void DeserializeEntities(UInt64 total, byte[] data, UInt64 offset)
        {
            for(UInt64 i = 0; i < total; i++)
            {
                int iOff = (int)offset + (((int)i) * sizeof(RawEntityToken));

                var entity = new RawEntityToken();
                entity.EntityId = BitConverter.ToUInt16(data, iOff);
                entity.X = BitConverter.ToUInt16(data, iOff + 2);
                entity.Y = BitConverter.ToUInt16(data, iOff + 4);
                entity.Config = BitConverter.ToUInt64(data, iOff + 16);
                _RawEntities[i] = entity;
            }
        }

        private unsafe void DeserializeWalls(UInt16 width, UInt16 height, byte[] data, UInt64 offset)
        {
            for(UInt16 i = 0; i < height; i++)
            {
                for(UInt16 j = 0; j < width; j++)
                {
                    int idx = (i * width) + j;
                    int iOff = ((int)offset) + (idx * sizeof(RawWallToken));
                    var wall = new RawWallToken();
                    wall.WallType = BitConverter.ToUInt16(data, iOff);
                    wall.IsDoor = data[iOff + 2];
                    wall.LockType = data[iOff + 3];
                    wall.Floor = BitConverter.ToUInt16(data, iOff + 4);
                    wall.Ceiling = BitConverter.ToUInt16(data, iOff + 6);
                    wall.Zone = data[8];
                    wall.Message = BitConverter.ToUInt64(data, iOff + 16);

                    _RawWalls[idx] = wall;
                }
            }
        }

        private void Convert(byte[] data)
        {
            var level = new LevelToken();
            level.WaterMark = "HAM";
            level.Version = _RawLevel.Version;
            level.Width = _RawLevel.Width;
            level.Height = _RawLevel.Height;
            level.TotalEntities = _RawLevel.TotalEntities;
            level.Walls = new WallToken[level.Width, level.Height];
            level.Entities = new EntityToken[level.TotalEntities];
            Level = level;
            ConvertWalls(data);
        }

        private void ConvertWalls(byte[] data)
        {
            for(int i = 0; i < Level.Height; i++)
            {
                for(int j = 0; j < Level.Width; j++)
                {
                    var idx = i * Level.Width + j;
                    var wall = new WallToken();
                    var rawWall = _RawWalls[idx];

                    wall.WallType = rawWall.WallType;
                    wall.IsDoor = rawWall.IsDoor != 0;
                    wall.LockType = rawWall.LockType;
                    wall.Floor = rawWall.Floor;
                    wall.Ceiling = rawWall.Ceiling;
                    wall.Zone = rawWall.Zone;

                    UInt64 c = rawWall.Message;
                    if(c == 0)
                    {
                        continue;
                    }
                    for (; c < (UInt64)data.Length; c++)
                    {
                        if(data[c] == 0)
                        {
                            break;
                        }
                    }
                    byte[] message = data
                        .Skip((int)rawWall.Message)
                        .Take((int)(c - rawWall.Message))
                        .ToArray();
                    wall.Message = System.Text.Encoding.ASCII.GetString(message);
                }
            }
        }
    }
}