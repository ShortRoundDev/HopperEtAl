using Hopper.Geometry;
using Hopper.Graphics;
using Hopper.Managers;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Tags
{
    public interface Usable
    {
        public float UseDistance { get; set; }
        public bool Used { get; set; }
        public void OnUse();

        public void UsableUpdate()
        {
            if (CanUse() && InputManager.Keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_E].Down && InputManager.Keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_E].Edge)
            {
                Used = true;
                OnUse();
            }
        }

        public void UsableDraw()
        {
            if (CanUse())
            {
                var _this = this as Entity;
                var useIcon = GraphicsManager.GetTexture("Use");
                var src = new SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = 12,
                    h = 12
                };

                var rect = new Rect(
                    (_this.Box.x + _this.Box.w / 2) - 6,
                    _this.Box.y - 12,
                    12,
                    12
                );

                Render.Box(rect.AsFRect(), src, useIcon, SDL.SDL_RendererFlip.SDL_FLIP_NONE);
            }
        }

        public bool CanUse()
        {
            var _this = this as Entity;
            return Math.Sqrt(Math.Pow(_this.Box.x - GameManager.MainPlayer.Box.x, 2) + Math.Pow(_this.Box.y - GameManager.MainPlayer.Box.y, 2)) < UseDistance;
        }
    }
}
