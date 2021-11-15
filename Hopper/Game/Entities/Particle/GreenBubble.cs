using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Particle
{
    public class GreenBubble : OneLoopParticleEffect
    {
        public GreenBubble(int x, int y) : base(
            GraphicsManager.GetTexture("GreenBubble"),
            x, y, 10, 10,
            3, 0.3f
        )
        {

        }
    }
}
