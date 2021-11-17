using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hopper.Geometry;
using Hopper.Graphics;
using Hopper.Managers;
using SDL2;

namespace Hopper.Game
{
    public abstract class Entity
    {

        public static readonly byte HIT_TOP     = 1;
        public static readonly byte HIT_RIGHT   = 2;
        public static readonly byte HIT_BOTTOM  = 4;
        public static readonly byte HIT_LEFT    = 8;

        public IntPtr Texture { get; set; }
        public Rect Box { get; set; }
        public bool RenderFlip { get; set; } = false;
        public SDL.SDL_RendererFlip SDLFlip { get => RenderFlip
                ? SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL
                : SDL.SDL_RendererFlip.SDL_FLIP_NONE;
        }
        public bool OnGround { get; set; } = false;
        protected Animator Animate { get; set; }
        public SDL.SDL_Rect IntBox
        {
            get => new SDL.SDL_Rect()
            {
                x = (int)Box.x,
                y = (int)Box.y,
                w = (int)Box.w,
                h = (int)Box.h
            };
        }
        public Point MoveVec { get; set; }

        public bool Deleted { get; set; } = false;

        public Entity(IntPtr Texture, Rect Box)
        {
            this.Texture = Texture;
            this.Box = Box;
            MoveVec = new();
        }

        public Entity(IntPtr Texture, int x, int y, int w, int h)
        {
            this.Texture = Texture;
            Box = new Rect(
                (float)x * 32,
                (float)y * 32,
                (float)w,
                (float)h
            );
            MoveVec = new();
        }

        public abstract void Update();
        public abstract void Draw();

        protected byte MoveAndCollide()
        {

            byte hitDirection = 0;

            var hypoX = new Rect()
            {
                x = (Box.x + MoveVec.x),
                y = Box.y,
                w = Box.w,
                h = Box.h
            };

            var hypoY = new Rect()
            {
                x = Box.x,
                y = (Box.y + MoveVec.y),
                w = Box.w,
                h = Box.h
            };

            OnGround = false;

            float moveX = MoveVec.x;
            Rect intersection = new Rect();
            for (int i = ((int)hypoX.x) / 32; i <= (int)(hypoX.x + hypoX.w) / 32; i++)
            {
                for (int j = ((int)hypoX.y) / 32; j <= (int)(hypoX.y + hypoX.h) / 32; j++)
                {
                    if (i < 0 || j < 0 || i >= GameManager.CurrentLevel.Width || j >= GameManager.CurrentLevel.Height)
                        continue;

                    var tile = GameManager.CurrentLevel.Tiles[i, j];
                    if(tile == null)
                    {
                        continue;
                    }

                    var xCollide = hypoX.Intersect(tile.Box);
                    
                    if (xCollide)
                    {
                        if(moveX > 0.0f)
                        {
                            hitDirection |= HIT_RIGHT;
                            Box.x = hypoX.x - ((hypoX.x + hypoX.w) - tile.Box.x);
                        }
                        else if(moveX < 0.0f)
                        {
                            hitDirection |= HIT_LEFT;
                            Box.x = tile.Box.x + tile.Box.w;
                        }
                        moveX = 0;
                        break;
                    }
                }
            }

            float moveY = MoveVec.y;
            for (int i = ((int)hypoY.x) / 32; i <= (int)(hypoY.x + hypoY.w) / 32; i++)
            {
                for (int j = ((int)hypoY.y) / 32; j <= (int)(hypoY.y + hypoY.h) / 32; j++)
                {
                    if (i < 0 || j < 0 || i >= GameManager.CurrentLevel.Width || j >= GameManager.CurrentLevel.Height)
                        continue;
                    var tile = GameManager.CurrentLevel.Tiles[i, j];
                    if (tile == null)
                    {
                        continue;
                    }

                    var yCollide = hypoY.Intersect(tile.Box);

                    if (yCollide)
                    {
                        if(moveY > 0.0f)
                        {
                            hitDirection |= HIT_BOTTOM;
                            OnGround = true;
                            Box.y = hypoY.y - ((hypoY.y + hypoY.h) - tile.Box.y);
                        } else if(moveY < 0.0f)
                        {
                            hitDirection |= HIT_TOP;
                            Box.y = tile.Box.y + tile.Box.h;
                        }
                        moveY = 0;
                        break;
                    }
                }
            }

            Box.x += moveX;
            Box.y += moveY;

            MoveVec.x = moveX;
            MoveVec.y = moveY;

            return hitDirection;
        }

        protected void Look()
        {
            if(MoveVec.x < 0)
            {
                RenderFlip = true;
            } else if(MoveVec.x > 0)
            {
                RenderFlip = false;
            }
        }

        private void CollectTiles()
        {

            for(int i = ((int)Box.x)/32; i <= (int)(Box.x + Box.w)/32; i++)
            {
                for (int j = ((int)Box.y)/32; j <= (int)(Box.y + Box.h)/32; j++)
                {

                }
            }
        }

        public void Configure(string configuration)
        {

        }
    }
}
