using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hopper.Game;
using OpenGL;
using SDL2;

namespace Hopper.Graphics
{
    public class Camera
    {
        public SDL.SDL_FPoint Position { get; set; } = new SDL.SDL_FPoint() { x = 0, y = 0 };
        public Func<SDL.SDL_FPoint> Tracker { get; set; }
        public SDL.SDL_FPoint Scale { get; set; }
        public void Draw()
        {
            
        }

        public void Update()
        {
            if (Tracker == null)
                return;
            SDL.SDL_FPoint focus = Tracker();
            float x = Position.x + ((focus.x - Position.x) * 0.1f);
            float y = Position.y + ((focus.y - Position.y) * 0.2f);

            Position = new() { x = x, y = y };
        }

        public void Track(Entity entity)
        {
            Tracker = () => new() {
                x = entity.Box.x + entity.Box.w / 2,
                y = entity.Box.y + entity.Box.h / 2
            };
        }
    }
}
