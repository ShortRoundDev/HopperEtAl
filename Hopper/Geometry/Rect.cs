using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Hopper.Geometry
{
    public class Rect
    {
        public float x { get; set; }
        public float y { get; set; }
        public float w { get; set; }
        public float h { get; set; }

        public Rect()
        {

        }

        public Rect(float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public SDL.SDL_FRect AsFRect()
        {
            return new SDL.SDL_FRect()
            {
                x = this.x,
                y = this.y,
                w = this.w,
                h = this.h
            };
        }

        public bool Intersect(Rect r)
        {
            float   aMin = x,
                    aMax = aMin + w,
                    bMin = r.x,
                    bMax = bMin + r.w;

            if(bMin > aMin)
            {
                aMin = bMin;
            }
            if(bMax < aMax)
            {
                aMax = bMax;
            }
            if(aMax <= aMin)
            {
                return false;
            }

            aMin = y;
            aMax = aMin + h;
            bMin = r.y;
            bMax = bMin + r.h;

            if(bMin > aMin)
            {
                aMin = bMin;
            }
            if (bMax < aMax)
            {
                aMax = bMax;
            }
            if(aMax <= aMin)
            {
                return false;
            }
            return true;
        }

        public static Rect operator + (Rect a, Point b)
        {
            return new Rect(a.x + b.x, a.y + b.y, a.w, a.h);
        }

        public static Rect operator - (Rect a, Point b)
        {
            return new Rect(a.x - b.x, a.y - b.y, a.w, a.h);
        }
    }
}
