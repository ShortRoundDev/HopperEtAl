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
        public EnemyBullet(Point start, bool left) : base(
            IntPtr.Zero,
            new Rect() { x = start.x, y = start.y, w = 6, h = 2 },
            new Point() { x = left ? -5 : 5, y = 0 },
            new Type[] { typeof(Player), typeof(PseudoGeometry) }
        )
        {
            Console.WriteLine("Shooting!!!!!!!");
            if (Distance(GameManager.MainPlayer) < 256)
            {
                GameManager.PlayChunk("Shoot");
            }
            Damage = 1;
        }

        public override void Draw()
        {
            Render.BoxFill(Box, 0xc3, 0xc3, 0xc3, 0xaa);
        }
    }
}
