using Hopper.Game.Entities.Doors;
using Hopper.Game.Tags;
using Hopper.Geometry;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Projectiles
{
    public class PlayerBullet : Projectile
    {
        int Frame = 0;
        public PlayerBullet(Point start, Point move) : base(
            IntPtr.Zero, new() { x = start.x, y = start.y, w =2, h = 2 }, move,
            new Type[] {typeof(Enemy), typeof(Door)}
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
            Frame++;
            if ((Frame% 2) == 0)
            {
                Render.BoxFill(Box, 0, 0xff, 0, 0xff);
            }
            else
            {
                Render.BoxFill(Box, 0xff, 0, 0, 0xff);
            }
        }
    }
}
