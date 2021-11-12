using Hopper.Game.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Doors
{
    [EntityId('G')]
    public class GreenDoor : Door
    {
        public GreenDoor(int x, int y) : base(x, y, 2)
        {
        }
    }
}
