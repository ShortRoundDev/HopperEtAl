using Hopper.Game.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Tiles
{

    [TileId(7)]
    public class Ice : Tile
    {
        public Ice(int x, int y, ushort tileNum) : base(x, y, tileNum)
        {
            Friction = 0.0f;
        }
    }
}
