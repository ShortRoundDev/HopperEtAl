using Hopper.Geometry;
using Hopper.Graphics;
using Hopper.Managers;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Particle
{
    public class Gib : Entity
    {
        IntPtr chunk = IntPtr.Zero;
        bool PlayedSound = false;

        float decay = 512;

        public Gib(float x, float y, Point MoveVec) : base(IntPtr.Zero, new Rect(x, y, 4, 4))
        {
            this.MoveVec = MoveVec;
            Random r = new Random();
            float scale = (float)r.NextDouble() + 1.0f;
            Box.w *= scale;
            Box.h *= scale;

            var id = (r.Next() % 5);
            if (id > 0)
            {
                chunk = GameManager.GetAudio("Squish" + id);
                if (chunk == IntPtr.Zero)
                {
                    Console.WriteLine("oops fuck");
                }
            }
        }

        public override void Draw()
        {
            float alphaF = Math.Min(decay, 255.0f);
            byte alpha = (byte)alphaF;
            Render.BoxFill(Box, new SDL.SDL_Color() { r = 0xdd, g = 0, b = 0, a = alpha });
        }

        public override void Update()
        {
            decay--;
            if(decay <= 0)
            {
                GameManager.DeleteEntity(this);
                Deleted = true;
                return;
            }
            MoveVec.y += GameManager.Gravity * 0.5f;
            MoveAndCollide();
            if (OnGround)
            {
                if (!PlayedSound && chunk != IntPtr.Zero)
                {
                    SDL_mixer.Mix_PlayChannel(-1, chunk, 0);
                    PlayedSound = true;
                }
                MoveVec.x *= 0.8f;
            }
        }
    }
}
