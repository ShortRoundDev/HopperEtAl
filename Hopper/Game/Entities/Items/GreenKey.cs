using Hopper.Game.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Items
{
    [EntityId('g')]
    public class GreenKey : Key
    {
        public GreenKey(int x, int y) : base(x, y, 2)
        {
        }
    }
}
