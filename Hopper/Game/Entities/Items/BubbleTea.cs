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
    [EntityId(3005)]
    public class BubbleTea : Entity
    {
        public BubbleTea(int x, int y) : base(GraphicsManager.GetTexture("BubbleTea"), x, y, 12, 16)
        {

        }
        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Texture);
        }

        public override void Update()
        {
            if (Box.Intersect(GameManager.MainPlayer.Box) && GameManager.MainPlayer.Health < 3)
            {
                GameManager.MainPlayer.Health++;
                GameManager.DeleteEntity(this);
                Deleted = true;
            }
        }
    }
}
