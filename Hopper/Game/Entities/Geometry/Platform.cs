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
        
        public List<Entity> Captured { get; set; } = new List<Entity>();
        public Platform(int x, int y) : base(GraphicsManager.GetTexture("Platform"), x, y, 64, 16)
        {
        }

        public byte CollisionDirectionMask { get; set; } = HIT_BOTTOM;

        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Texture);
        }

        public override void Update()
        {
            MoveVec.x = dir;
            Point start = new(Box.x, Box.y);
            var hit = MoveAndCollide();
            if((hit & (HIT_LEFT | HIT_RIGHT)) != 0)
            {
                dir *= -1;
            }
            
            Point diff = new Point(Box.x, Box.y) - start;
            foreach(var ent in Captured)
            {
                ent.Impulse.x = diff.x;
            }
        }
    }
}
