using Hopper.Game.Tags;
using Hopper.Geometry;
using Hopper.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Projectiles
{
    public class PlayerBullet : Projectile
    {

        public PlayerBullet(Point start, Point move) : base(
            IntPtr.Zero, new() { x = start.x, y = start.y, w =6, h = 2 }, move,
            new Type[] {typeof(Enemy), typeof(PseudoGeometry)}
        )
        {
            Damage = 1;
        }
        public PlayerBullet(Point start, bool left) : this(
            start,
            new Point() { x = left ? -8 : 8, y = 0 }
        )
        {
        }

        public override void Draw()
        {
            Render.BoxFill(Box, 0xc3, 0xc3, 0xc3, 0xaa);
        }
    }
}
