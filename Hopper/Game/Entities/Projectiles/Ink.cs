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
    public class Ink : Projectile
    {
        public Ink(int x, int y, Point move) : base(GraphicsManager.GetTexture("Ink"), new() { x = x, y = y, w = 8, h = 8 }, move, new Type[] { typeof(Player) })
        {
            Damage = 1;
        }

        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Texture);
        }

        public override void Update()
        {
            base.Update();
            if (!InWater)
            {
                Deleted = true;
                GameManager.DeleteEntity(this);
            }
        }
    }
}
