using Hopper.Game.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Doors
{
    [EntityId('B')]
    public class BlueDoor : Door
    {
        public BlueDoor(int x, int y) : base(x, y, 4)
        {
        }
    }
}
