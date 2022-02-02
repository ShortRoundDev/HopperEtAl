using Hopper.Game.Attributes;
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
    [EntityId(3019)]
    public class LaunchPad : Entity
    {
        bool Flip;
        public LaunchPad(int x, int y) : base(GraphicsManager.GetTexture("LaunchPad"), x, y, 32, 32)
        {

        }

        public override void Configure(string configuration)
        {
            Flip = configuration?.ToLower() == "left";
        }

        public override void Draw()
        {
            SDL.SDL_Rect uv = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 32,
                h = 32
            };
            Render.Box(Box.AsFRect(), uv, Texture, Flip ? SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL : SDL.SDL_RendererFlip.SDL_FLIP_NONE);
        }

        public override void Update()
        {
            foreach(var e in GameManager.CurrentLevel.Entities)
            {
                if (e.Box.Intersect(Box))
                {
                    e.MoveVec.x = Flip ? -6 : 6;
                    e.MoveVec.y = -15;
                    if(e is Player p)
                    {
                        p.Jumping = false;
                    }
                }
            }
        }
    }
}
