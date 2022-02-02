using Hopper.Game.Attributes;
using Hopper.Game.Tags;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Geometry
{
    [EntityId(3017)]
    public class Spring : Entity, PseudoGeometry
    {
        public byte CollisionDirectionMask { get; set; } = (byte)(HIT_RIGHT | HIT_LEFT | HIT_TOP);

        public Spring(int x, int y) : base(GraphicsManager.GetTexture("Spring"), x, y, 32, 32)
        {
           
        }

        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Texture);
        }

        public override void Update()
        {
            if (Box.Intersect(GameManager.MainPlayer.Box))
            {
                GameManager.MainPlayer.MoveVec.x *= 2;
                GameManager.MainPlayer.MoveVec.y = -15;
                GameManager.MainPlayer.Jumping = false;
            }
        }
    }
}
