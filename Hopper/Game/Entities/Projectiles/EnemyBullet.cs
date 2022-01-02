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
    public class EnemyBullet : Projectile, Enemy
    {
        int Frame = 0;
        public EnemyBullet(Point start, bool left) : base(
            IntPtr.Zero,
            new Rect() { x = start.x, y = start.y, w = 6, h = 2 },
            new Point() { x = left ? -4 : 4, y = 0 },
            new Type[] { typeof(Player), typeof(Door) }
        )
        {
            GameManager.PlayChunkAtt("Shoot", this);
            Damage = 1;
        }

        public override void Draw()
        {
            Frame++;
            if ((Frame % 2) == 0)
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
