using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Collectibles
{
    public abstract class Collectible : Entity
    {
        public Collectible(IntPtr texture, int x, int y) : base(texture, x, y, 32, 32)
        {

        }

        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Texture);
        }

        public override void Update()
        {
            if (GameManager.MainPlayer.Box.Intersect(Box))
            {
                GameManager.MainPlayer.Score += 100;
                GameManager.DeleteEntity(this);
            }
        }
    }
}
