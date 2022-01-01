using Hopper.Game.Attributes;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Powerups
{
    [EntityId(3015)]
    public class ShotGun : Entity
    {
        public ShotGun(int x, int y) : base(GraphicsManager.GetTexture("ShotGun"), x, y, 64, 24)
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
                GameManager.PlayChunk("Rifle");
                this.Deleted = true;
                GameManager.DeleteEntity(this);
                GameManager.MainPlayer.Shotgun = true;
            }
        }
    }
}
