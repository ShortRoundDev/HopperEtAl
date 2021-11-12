using Hopper.Game.Attributes;
using Hopper.Game.Tags;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities
{
    [EntityId('M')]
    public class Brainiac : Entity, Enemy, Killable
    {
        public int Health { get; set; } = 2;
        public int MaxHealth { get; set; } = 2;

        public Brainiac(int x, int y) : base(GraphicsManager.GetTexture("Brainiac"), x, y, 48, 48)
        {
            MoveVec.x = 1;
            Animate = new Animator()
            {
                Rows = 1,
                Columns = 4,
                Speed = 0.08f,
                SrcRect = new SDL2.SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = 48,
                    h = 48
                }
            };
        }
        public override void Draw()
        {
            Look();
            Render.Box(Box.AsFRect(), Animate.GetUVMap(), Texture, SDLFlip);
            Killable k = this;
            k.KillableDraw();
        }

        public override void Update()
        {
            Animate.Update();
            MoveVec.y += GameManager.Gravity;
            var hitSide = MoveAndCollide();
            Think(hitSide);
        }

        public void Think(byte hitSide)
        {
            if((hitSide & (HIT_LEFT)) != 0) {
                MoveVec.x = 1;
            }

            if ((hitSide & (HIT_RIGHT)) != 0) {
                MoveVec.x = -1;
            }
            if (OnGround)
            {
                MoveVec.x = Math.Sign(MoveVec.x);
            }
        }
    }
}
