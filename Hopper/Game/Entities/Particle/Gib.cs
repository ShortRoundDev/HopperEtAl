﻿using Hopper.Geometry;
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

        float WidthDelta = 0;
        float HeightDelta = 0;

        float decay = 512;

        bool Wall = false;
        bool Floor = false;

        byte WhichWall = 0;

        SDL.SDL_Color color { get; set; } = UIManager.LT_RED;

        public Gib(float x, float y, Point MoveVec) : this(x, y, MoveVec, UIManager.LT_RED)
        {

        }

        public Gib(float x, float y, Point MoveVec, SDL.SDL_Color color) : base(IntPtr.Zero, new Rect(x, y, 4, 4))
        {
            this.color = color;
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

            int side = ((WhichWall & HIT_RIGHT) != 0) ? -1 : 0;

            var DrawBox = new Rect()
            {
                x = (Box.x - WidthDelta / 2) - (Math.Min(HeightDelta, Box.w - 2) * side),
                y = Box.y + Math.Min(WidthDelta / 2, 3),
                w = Box.w + WidthDelta - Math.Min(HeightDelta, Box.w - 2),
                h = Box.h - Math.Min(WidthDelta / 2, 3) + (HeightDelta/2)
            };

            if (WidthDelta > 0)
            {
                int leftXlimit = (int)(Box.x / 32.0f) * 32;
                DrawBox.x = Math.Max(DrawBox.x, leftXlimit);

                int widthLimit = leftXlimit + 32;
                if(DrawBox.x + DrawBox.w > widthLimit)
                {
                    DrawBox.w = widthLimit - DrawBox.x;
                }

            }

            Render.BoxFill(DrawBox, new SDL.SDL_Color() { r = color.r, g = color.g, b = color.b, a = alpha });
        }

        public override void Update()
        {
            decay-=2;
            if(decay <= 0)
            {
                GameManager.DeleteEntity(this);
                Deleted = true;
                return;
            }

            if (Floor)
            {
                WidthDelta += 0.4f;
                MoveVec.x *= 0.8f;
                return;
            }
            else if (Wall)
            {
                HeightDelta += 0.4f;
                return;
            }

            byte hit = MoveAndCollide();
            if ((hit & (HIT_LEFT | HIT_RIGHT)) != 0)
            {
                Wall = true;
                WhichWall = hit;
                if (!PlayedSound && chunk != IntPtr.Zero)
                {
                    int channel = SDL_mixer.Mix_PlayChannel(-1, chunk, 0);
                    SDL_mixer.Mix_Volume(channel, (byte)(SystemManager.SfxVolume / 10.0f * 128.0f));
                    PlayedSound = true;
                }
            }
            else if (OnGround)
            {
                Floor = true;
                if (!PlayedSound && chunk != IntPtr.Zero)
                {
                    int channel = SDL_mixer.Mix_PlayChannel(-1, chunk, 0);
                    SDL_mixer.Mix_Volume(channel, (byte)(SystemManager.SfxVolume / 10.0f * 128.0f));
                    PlayedSound = true;
                }
            }
            else
            {
                if (InWater)
                    MoveVec.y = 0.8f;
                else
                    MoveVec.y += GameManager.Gravity * 0.5f;
            }
        }
    }
}
