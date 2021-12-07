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
    public class Fire : Entity, Enemy
    {
        int Timer { get; set; } = 0;
        public int Damage { get; set; } = 1;
        public Type[] CollideWith { get; set; } = new Type[] { typeof(Player) };

        public Fire(float x, float y, int timer) : base(GraphicsManager.GetTexture("Fire"), new(x, y, 24, 32))
        {
            Timer = timer;
            Animate = new Animator()
            {
                SrcRect = new SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = 24,
                    h = 32
                },
                Rows = 1,
                Columns = 2,
                Speed = 0.2f
            };
        }
        public override void Update()
        {
            Animate.Update();
            if(Timer > 0)
            {
                (this as Enemy).EnemyUpdate();
                Timer--;
            } else
            {
                GameManager.DeleteEntity(this);
                Deleted = true;
            }
        }

        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Animate.GetUVMap(), Texture, SDLFlip);
        }
    }
}
