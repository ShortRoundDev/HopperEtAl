using Hopper.Geometry;
using Hopper.Graphics;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Particle
{
    public abstract class OneLoopParticleEffect : Entity
    {
        public OneLoopParticleEffect(IntPtr texture, int x, int y, int w, int h, int frames, float speed) : base(texture, new Rect(x, y, w, h))
        {
            Animate = new Animator()
            {
                SrcRect = new SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = w,
                    h = h
                },
                Columns = frames,
                Speed = speed,
                Rows = 1,
                Loop = false
            };
        }
        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Animate.GetUVMap(), Texture, SDL.SDL_RendererFlip.SDL_FLIP_NONE);
        }

        public override void Update()
        {
            Animate.Update();
            if (Animate.Finished)
            {
                Managers.GameManager.DeleteEntity(this);
                Deleted = true;
            }
        }
    }
}
