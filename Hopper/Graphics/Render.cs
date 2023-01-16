using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;

using SDL2;
using Hopper.Managers;
using Hopper.Geometry;

namespace Hopper.Graphics
{
    public static class Render
    {
        public static void Box(SDL.SDL_FRect rect, IntPtr texture, SDL.SDL_RendererFlip flip = SDL.SDL_RendererFlip.SDL_FLIP_NONE)
        {
            var src = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = (int)rect.w,
                h = (int)rect.h
            };
            Box(rect, src, texture, flip);
        }

        public static void Box(SDL.SDL_FRect rect, SDL.SDL_Rect src, IntPtr Texture, SDL.SDL_RendererFlip flip)
        {
            var dst = FRectPerspective(rect);
            SDL.SDL_RenderCopyExF(GraphicsManager.Renderer, Texture, ref src, ref dst, 0.0f, IntPtr.Zero, flip);
        }

        public static SDL.SDL_FRect FRectPerspective(SDL.SDL_FRect rect)
        {
            Vector4 dst = new Vector4(rect.x, rect.y, rect.w, rect.h);
            dst.X -= GraphicsManager.MainCamera.Position.x - (SystemManager.Width / (2 * GraphicsManager.MainCamera.Scale.x));
            dst.Y -= GraphicsManager.MainCamera.Position.y - (SystemManager.Height / (2 * GraphicsManager.MainCamera.Scale.y));

            dst *= new Vector4(
                GraphicsManager.MainCamera.Scale.x,
                GraphicsManager.MainCamera.Scale.y,
                GraphicsManager.MainCamera.Scale.x,
                GraphicsManager.MainCamera.Scale.y
            );

            var _dst = new SDL.SDL_FRect()
            {
                x = dst.X,
                y = dst.Y,
                w = dst.Z,
                h = dst.W
            };
            return _dst;
        }

        public static void BoxFill(Rect rect, byte r, byte g, byte b, byte a)
        {
            BoxFill(rect.AsFRect(), new SDL.SDL_Color() { r = r, g = g, b = b, a = a });
        }

        public static void BoxFill(Rect rect, SDL.SDL_Color color)
        {
            BoxFill(rect.AsFRect(), color);
        }

        public static void BoxFill(SDL.SDL_FRect rect, SDL.SDL_Color color)
        {
            byte r, g, b, a;
            SDL.SDL_GetRenderDrawColor(GraphicsManager.Renderer, out r, out g, out b, out a);
            SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, color.r, color.g, color.b, color.a);

            var dst = FRectPerspective(rect);
            SDL.SDL_RenderFillRectF(GraphicsManager.Renderer, ref dst);
            SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, r, g, b, a);
        }

        public static void BoxDraw(SDL.SDL_FRect rect, SDL.SDL_Color color)
        {
            byte r, g, b, a;
            SDL.SDL_GetRenderDrawColor(GraphicsManager.Renderer, out r, out g, out b, out a);
            SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, color.r, color.g, color.b, color.a);

            var dst = FRectPerspective(rect);
            SDL.SDL_RenderDrawRectF(GraphicsManager.Renderer, ref dst);
            SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, r, g, b, a);
        }
    }
}
