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
    [EntityId(3016)]
    public class Crosshair : Entity
    {
        public Crosshair(int x, int y) : base(GraphicsManager.GetTexture("Crosshair"), x, y, 32, 32)
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
                Deleted = true;
                GameManager.DeleteEntity(this);
                GameManager.MainPlayer.CrossHair = true;
            }
        }
    }
}
