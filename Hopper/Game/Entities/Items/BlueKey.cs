using Hopper.Game.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Items
{
    [EntityId(1001)]
    public class BlueKey : Key
    {
        public BlueKey(int x, int y) : base(x, y, 4)
        {
        }
    }
}
