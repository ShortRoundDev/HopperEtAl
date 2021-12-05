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
    [EntityId(3018)]
    public class Splitter : Entity, PseudoGeometry
    {
        public byte CollisionDirectionMask { get; set; } = (byte)(HIT_RIGHT | HIT_LEFT | HIT_TOP);

        public Splitter(int x, int y) : base(GraphicsManager.GetTexture("Splitter"), x, y, 32, 32)
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
                Random r = new Random();
                GameManager.MainPlayer.MoveVec.y = 0;
                GameManager.MainPlayer.MoveVec.x += (1 - (r.Next(2) * 2)) * 31;
                GameManager.MainPlayer.OnGround = true;
            }
        }
    }
}
