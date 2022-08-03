using Hopper.Game.Attributes;
using Hopper.Game.Tags;
using Hopper.Geometry;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Geometry
{
    [EntityId(3023)]
    public class Platform : Entity, PseudoGeometry
    {
        float dir = 1.0f;

        float startY;

        public List<Entity> Captured { get; set; } = new List<Entity>();
        public Platform(int x, int y) : base(GraphicsManager.GetTexture("Platform"), x, y, 64, 16)
        {
            startY = y * 32.0f;
        }

        public byte CollisionDirectionMask { get; set; } = HIT_BOTTOM;

        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Texture);
        }

        public override void Update()
        {
            MoveVec.y = dir;
            Point start = new(Box.x, Box.y);

            // check capture

            bool NotCaptured = !Captured.Contains(GameManager.MainPlayer);

            var hit = MoveAndCollide();

            if(startY - Box.y > (32 * 3) || startY - Box.y < 0)
            {
                dir *= -1;
            }

            /*if((hit & (HIT_LEFT | HIT_RIGHT)) != 0)
            {
                dir *= -1;
            }*/
            
            Point diff = new Point(Box.x, Box.y) - start;

            if (NotCaptured && GameManager.MainPlayer.MoveVec.y > 0 && MoveVec.y < 0 && GameManager.MainPlayer.Box.Intersect(Box))
            {
                //check lower bounded
                var playerFoot = GameManager.MainPlayer.Box.y + GameManager.MainPlayer.Box.h;
                if (playerFoot > Box.y && playerFoot <= Box.y + Box.h)
                {
                    GameManager.MainPlayer.PlatformParent = this;
                    Captured.Add(GameManager.MainPlayer);
                }
            }

            foreach(var ent in Captured)
            {
                ent.Impulse.x = diff.x;
                ent.Box.y = Box.y - ent.Box.h;
            }
        }
    }
}
