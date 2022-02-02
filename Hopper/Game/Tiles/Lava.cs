using Hopper.Game.Attributes;
using Hopper.Game.Tags;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Tiles
{
    [EntityId(3020)]
    public class Lava : Entity, Enemy
    {
        public Lava(int x, int y) : base(GraphicsManager.GetTexture("Lava"), x, y, 32, 32)
        {
        }

        Type[] Enemy.CollideWith { get; set; } = new Type[] { typeof(Killable) };
        int Enemy.Damage { get; set; } = 3;

        public override void Draw()
        {
            RenderFlip = ((int)(GameManager.Frame / 10)) % 2 == 0;
            Render.Box(Box.AsFRect(), Texture, SDLFlip);
        }

        public override void Update()
        {
            (this as Enemy).EnemyUpdate(true);
        }
    }
}
