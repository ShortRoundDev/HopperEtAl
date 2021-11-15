using Hopper.Game.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Doors
{
    [EntityId(2002)]
    public class RedDoor : Door
    {
        public RedDoor(int x, int y) : base(x, y, 1)
        {
        }
    }
}
