using Hopper.Game.Attributes;
using Hopper.Game.Tags;
using Hopper.Geometry;
using Hopper.Graphics;
using Hopper.Managers;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities
{
    [EntityId(3021)]
    public class EasterMan : Entity, Enemy
    {
        int Enemy.Damage { get; set; } = 1;
        Type[] Enemy.CollideWith { get; set; } = new Type[] { typeof(Killable) };

        private int HopCooldown { get; set; } = 0;

        public EasterMan(int x, int y) : base(GraphicsManager.GetTexture("Easterman"), new Rect(x * 32 + 2, y * 32 + 2, 28, 28))
        {

        }

        public override void Draw()
        {
            var flip = MoveVec.y > 0 ? SDL.SDL_RendererFlip.SDL_FLIP_VERTICAL : SDL.SDL_RendererFlip.SDL_FLIP_NONE;

            Render.Box(Box.AsFRect(), Texture, flip);
        }

        public override void Update()
        {
            (this as Enemy).EnemyUpdate();
            if (OnGround) {
                HopCooldown--;
                if (HopCooldown <= 0)
                {
                    MoveVec.y = -7;
                    HopCooldown = 100;
                }
            }
            MoveVec.y += GameManager.Gravity * 0.5f;
            MoveAndCollide();
        }
    }
}
