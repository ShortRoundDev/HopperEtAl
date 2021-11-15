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
        private static IntPtr BubbleTea { get; set; }
        private static IntPtr EmptyBubbleTea { get; set; }
        public static void Init()
        {
            Numbers = GraphicsManager.GetTexture("Numbers");
            BubbleTea = GraphicsManager.GetTexture("BubbleTea");
            EmptyBubbleTea = GraphicsManager.GetTexture("BubbleTeaEmpty");
        }

        public static void Draw()
        {

            while(NumberMessages.Count > 0)
            {
                var message = NumberMessages.Dequeue();
                DrawNumberString(message.Message, message.Position);
            }
            DrawHealthBar();
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

        private static void DrawHealthBar()
        {
            var player = GameManager.MainPlayer;

            var src = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 12,
                h = 16
            };
            
            for(int i = 0; i < player.Health; i++)
            {
                var dst = new SDL.SDL_Rect()
                {
                    x = 10 + i * 36,
                    y = 10,
                    w = 36,
                    h = 48
                };

                SDL.SDL_RenderCopy(GraphicsManager.Renderer, BubbleTea, ref src, ref dst);
            }
            for (int i = player.Health; i < player.MaxHealth; i++)
            {
                var dst = new SDL.SDL_Rect()
                {
                    x = 10 + i * 36,
                    y = 10,
                    w = 36,
                    h = 48
                };

                SDL.SDL_RenderCopy(GraphicsManager.Renderer, EmptyBubbleTea, ref src, ref dst);
            }
        }
    }
}
