using Hopper.Game.Attributes;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Items
{
    [EntityId(1000)]
    public class Ammo : Entity
    {
        public Ammo(int x, int y) : base(GraphicsManager.GetTexture("Ammo"), x, y, 12, 12)
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
                GameManager.PlayChunk("Ammo");
                GameManager.MainPlayer.Ammo += 5;
                GameManager.DeleteEntity(this);
            }
        }
    }
}
