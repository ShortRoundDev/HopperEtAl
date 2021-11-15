using Hopper.Game.Attributes;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Collectibles
{
    [EntityId(1004)]
    public class Pizza : Collectible
    {
        public Pizza(int x, int y) : base(GraphicsManager.GetTexture("Pizza"), x, y)
        {

        }
    }
}
