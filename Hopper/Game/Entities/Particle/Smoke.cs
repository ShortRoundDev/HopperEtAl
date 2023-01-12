using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Particle
{
    public class Smoke : OneLoopParticleEffect
    {
        public Smoke(int x, int y) : base(GraphicsManager.GetTexture("Smoke"), x, y, 12, 12, 1, 0.05f)
        {
        }
        public override void Update()
        {
            base.Update();
            Box.w -= 1.0f;
            Box.h -= 1.0f;
            Box.x += 0.5f;
            Box.y += 0.5f;
            if(Box.w < 0.0f)
            {
                GameManager.DeleteEntity(this);
            }
        }
    }
}
