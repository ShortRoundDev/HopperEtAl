using Hopper.Game.Attributes;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Collectibles
{
    [EntityId('h')]
    public class Burrito : Collectible
    {
        public Burrito(int x, int y) : base(GraphicsManager.GetTexture("Burrito"), x, y)
        {

        }
    }
}
