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
    [EntityId(3012)]
    public class Barrel : Entity, PseudoGeometry
    {
        public byte CollisionDirectionMask { get; set; } = HIT_BOTTOM;

        public Barrel(int x, int y) : base(GraphicsManager.GetTexture("Barrel"), x, y, 32, 32)
        {

        }

        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Texture);
        }

        public override void Update()
        {
        }
    }
}
