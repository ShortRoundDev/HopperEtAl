using Hopper.Geometry;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Items
{
    public abstract class Key : Entity
    {
        public int KeyType { get; set; } // 1 - r, 2 - g, 4 - b
        protected Key(int x, int y, int keyType) : base(
            GraphicsManager.GetTexture(keyType switch{
                1 => "RedKey",
                2 => "GreenKey",
                4 => "BlueKey"
            }),
            x, y, 12, 12
        )
        {
            KeyType = keyType;
        }

        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Texture);
        }

        public override void Update()
        {
            if (GameManager.MainPlayer.Box.Intersect(Box))
            {
                GameManager.MainPlayer.Keys |= KeyType;
                GameManager.DeleteEntity(this);
                // play sound?
            }
        }
    }
}
