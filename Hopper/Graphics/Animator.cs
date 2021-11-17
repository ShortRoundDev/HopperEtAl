using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SDL2;

namespace Hopper.Graphics
{
    public class Animator
    {
        public SDL.SDL_Rect SrcRect { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }

        public float Speed { get; set; } = 0.07f;

        public float Frame { get; set; } = 0.0f;
        public int Animation { get; set; } = 0;
        public bool Loop { get; set; } = true;
        public bool Finished { get; set; } = false;

        public void Update()
        {
            if (Finished)
            {
                return;
            }
            Frame += Speed;
            if (Loop)
            {
                Frame = Frame % Columns;
            }
            else if(Frame > Columns) {
                Frame = Columns - 1;
                Finished = true;
            }
        }

        public SDL.SDL_Rect GetUVMap()
        {
            var uvMap = SrcRect;
            uvMap.x += SrcRect.w * (int)Frame;
            uvMap.y += SrcRect.h * Animation;
            return uvMap;
        }
    }
}
