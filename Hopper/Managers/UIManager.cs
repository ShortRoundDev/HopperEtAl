using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hopper.Geometry;
using Hopper.Graphics;
using SDL2;

namespace Hopper.Managers
{
    public static class UIManager
    {
        public static Queue<(string Message, Point Position)> NumberMessages { get; set; } = new();
        private static IntPtr Numbers { get; set; }
        public static void Init()
        {
            Numbers = GraphicsManager.GetTexture("Numbers");
        }

        public static void Draw()
        {

            while(NumberMessages.Count > 0)
            {
                var message = NumberMessages.Dequeue();
                DrawNumberString(message.Message, message.Position);
            }
        }

        public static void DrawNumbers(string message, Point position)
        {
            NumberMessages.Enqueue((message, position));
        }

        private static void DrawNumberString(string message, Point position)
        {
            var r = new SDL.SDL_FRect()
            {
                x = position.x,
                y = position.y,
                w = 28,
                h = 28
            };
            for (int i = 0; i < message.Length; i++)
            {
                char c = message[i];
                if (c < '0' || c > '9')
                {
                    continue;
                }
                r.x = position.x + i * 20;
                SDL.SDL_Rect src = new SDL.SDL_Rect()
                {
                    x = (c - '0') * 28,
                    y = 0,
                    w = 28,
                    h = 28
                };
                SDL.SDL_RenderCopyF(GraphicsManager.Renderer, Numbers, ref src, ref r);
                //Render.Box(r, src, Numbers, SDL.SDL_RendererFlip.SDL_FLIP_NONE);
            }
        }
    }
}
