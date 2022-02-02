using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hopper.Game.Tags;
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
        public bool InWater { get; set; } = false;
        public bool FeetInWater { get; set; } = false;
        protected Animator Animate { get; set; }
        protected float FrameFriction { get; set; }
        protected List<Entity> HitEntitiesThisFrame { get; set; } = new();
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

        public Point Top => new Point(Box.x + Box.w / 2, Box.y);
        public Point MoveVec { get; set; }
        public Point Impulse { get; set; } = new(0, 0);

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
            CheckInWater();
            CheckFeetInWater();
            HitEntitiesThisFrame.Clear();

            FrameFriction = 0.0f;

            var _MoveVec = MoveVec + Impulse;
            Impulse.x = 0;
            Impulse.y = 0;

            var hypoX = new Rect()
            {
                x = (Box.x + _MoveVec.x),
                y = Box.y,
                w = Box.w,
                h = Box.h
            };

            var hypoY = new Rect()
            {
                x = Box.x,
                y = (Box.y + _MoveVec.y),
                w = Box.w,
                h = Box.h
            };

            var hypoBoth = new Rect()
            {
                x = Box.x + _MoveVec.x,
                y = Box.y + _MoveVec.y,
                w = Box.w,
                h = Box.h
            };

            OnGround = false;

            float moveX = _MoveVec.x;
            float moveY = _MoveVec.y;

            bool xHit = false;
            bool yHit = false;

            Rect intersection = new Rect();

            //========== Check pseudoGeometry

            foreach(var entity in GameManager.CurrentLevel.Entities)
            {
                if(entity is not PseudoGeometry p || entity == this)
                {
                    continue;
                }

                if(xHit && yHit)
                {
                    break;
                }

                if (xHit)
                {
                    goto CheckEntityYCollide;
                }
                var xCollide = hypoX.Intersect(entity.Box);

                if (xCollide)
                {
                    if (moveX > 0.0f && (Box.x + Box.w <= entity.Box.x) && (p.CollisionDirectionMask & HIT_RIGHT) != 0)
                    {
                        HitEntitiesThisFrame.Add(entity);
                        hitDirection |= HIT_RIGHT;
                        Box.x = hypoX.x - ((hypoX.x + hypoX.w) - entity.Box.x);
                        moveX = 0;
                        xHit = true;
                    }
                    else if (moveX < 0.0f && (Box.x >= entity.Box.x + entity.Box.w) && (p.CollisionDirectionMask & HIT_LEFT) != 0)
                    {
                        HitEntitiesThisFrame.Add(entity);
                        hitDirection |= HIT_LEFT;
                        Box.x = entity.Box.x + entity.Box.w;
                        moveX = 0;
                        xHit = true;
                    }
                }

            CheckEntityYCollide:
                if(yHit)
                {
                    goto CheckEntityBothCollide;
                }
                var yCollide = hypoY.Intersect(entity.Box);

                if (yCollide)
                {
                    if (moveY > 0.0f && (Box.y + Box.h <= entity.Box.y) && (p.CollisionDirectionMask & HIT_BOTTOM) != 0)
                    {
                        HitEntitiesThisFrame.Add(entity);
                        hitDirection |= HIT_BOTTOM;
                        Box.y = hypoY.y - ((hypoY.y + hypoY.h) - entity.Box.y);
                        moveY = 0;
                        OnGround = true;
                        FrameFriction = 1.0f;
                        yHit = true;
                    }
                    else if (moveY < 0.0f && (Box.y > entity.Box.y + entity.Box.h) && (p.CollisionDirectionMask & HIT_TOP) != 0)
                    {
                        HitEntitiesThisFrame.Add(entity);
                        hitDirection |= HIT_TOP;
                        Box.y = entity.Box.y + entity.Box.h;
                        moveY = 0;
                        yHit = true;
                    }
                }
            CheckEntityBothCollide:
                if(xHit || yHit)
                {
                    break;
                }
                var bothCollide = hypoBoth.Intersect(entity.Box);

                if (bothCollide)
                {
                    /*if (moveY > 0.0f && (Box.y + Box.h <= entity.Box.y) && (p.CollisionDirectionMask & HIT_BOTTOM) != 0)
                    {
                        hitDirection |= HIT_BOTTOM;
                        Box.y = hypoY.y - ((hypoY.y + hypoY.h) - entity.Box.y);
                        moveY = 0;
                        OnGround = true;
                        FrameFriction = 1.0f;
                        yHit = true;
                    }
                    else if (moveY < 0.0f && (Box.y > entity.Box.y + entity.Box.h) && (p.CollisionDirectionMask & HIT_TOP) != 0)
                    {
                        hitDirection |= HIT_TOP;
                        Box.y = entity.Box.y + entity.Box.h;
                        moveY = 0;
                        yHit = true;
                    }*/
                    if (moveX > 0.0f && (Box.x + Box.w <= entity.Box.x) && (p.CollisionDirectionMask & HIT_RIGHT) != 0)
                    {
                        hitDirection |= HIT_RIGHT;
                        Box.x = hypoX.x - ((hypoX.x + hypoX.w) - entity.Box.x);
                        moveX = 0;
                        xHit = true;
                    }
                    else if (moveX < 0.0f && (Box.x >= entity.Box.x + entity.Box.w) && (p.CollisionDirectionMask & HIT_LEFT) != 0)
                    {
                        hitDirection |= HIT_LEFT;
                        Box.x = entity.Box.x + entity.Box.w;
                        moveX = 0;
                        xHit = true;
                    }
                }
            }

            for (int i = ((int)hypoX.x) / 32; i <= (int)(hypoX.x + hypoX.w) / 32; i++)
            {
                for (int j = ((int)hypoX.y) / 32; j <= (int)(hypoX.y + hypoX.h) / 32; j++)
                {
                    if (i < 0 || j < 0 || i >= GameManager.CurrentLevel.Width || j >= GameManager.CurrentLevel.Height)
                        continue;

                    var tile = GameManager.CurrentLevel.Tiles[i, j];
                    if (tile == null)
                    {
                        continue;
                    }

                    var xCollide = hypoX.Intersect(tile.Box);

                    if (xCollide)
                    {
                        if (moveX > 0.0f)
                        {
                            hitDirection |= HIT_RIGHT;
                            Box.x = hypoX.x - ((hypoX.x + hypoX.w) - tile.Box.x);
                        }
                        else if (moveX < 0.0f)
                        {
                            hitDirection |= HIT_LEFT;
                            Box.x = tile.Box.x + tile.Box.w;
                        }
                        moveX = 0;
                        break;
                    }
                }
            }

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
                        if (moveY > 0.0f)
                        {
                            hitDirection |= HIT_BOTTOM;
                            OnGround = true;
                            Box.y = hypoY.y - ((hypoY.y + hypoY.h) - tile.Box.y);
                        }
                        else if (moveY < 0.0f)
                        {
                            hitDirection |= HIT_TOP;
                            Box.y = tile.Box.y + tile.Box.h;
                        }
                        moveY = 0;
                        break;
                    }
                }
            }

            if(moveX == 0 || moveY == 0)
            {
                goto End;
            }

            for (int i = ((int)hypoBoth.x) / 32; i <= (int)(hypoBoth.x + hypoBoth.w) / 32; i++)
            {
                for (int j = ((int)hypoBoth.y) / 32; j <= (int)(hypoBoth.y + hypoBoth.h) / 32; j++)
                {
                    if (i < 0 || j < 0 || i >= GameManager.CurrentLevel.Width || j >= GameManager.CurrentLevel.Height)
                        continue;
                    var tile = GameManager.CurrentLevel.Tiles[i, j];
                    if (tile == null)
                    {
                        continue;
                    }

                    var bothCollide = hypoBoth.Intersect(tile.Box);

                    if (bothCollide)
                    {

                        /*if (moveY > 0.0f)
                        {
                            hitDirection |= HIT_BOTTOM;
                            OnGround = true;
                            Box.y = hypoBoth.y - ((hypoBoth.y + hypoBoth.h) - tile.Box.y);
                        }
                        else if (moveY < 0.0f)
                        {
                            hitDirection |= HIT_TOP;
                            Box.y = tile.Box.y + tile.Box.h;
                        }
                        moveY = 0;*/
                        if (moveX > 0.0f)
                        {
                            hitDirection |= HIT_RIGHT;
                            Box.x = hypoX.x - ((hypoX.x + hypoX.w) - tile.Box.x);
                        }
                        else if (moveX < 0.0f)
                        {
                            hitDirection |= HIT_LEFT;
                            Box.x = tile.Box.x + tile.Box.w;
                        }
                        moveX = 0;

                        break;
                    }
                }
            }

        /*foreach (var entity in GameManager.CurrentLevel.Entities)
         {
             if (entity is not PseudoGeometry p || entity == this)
             {
                 continue;
             }

             var yCollide = hypoY.Intersect(entity.Box);

             if (yCollide)
             {
                 if (moveY > 0.0f && (Box.y + Box.h <= entity.Box.y) && (p.CollisionDirectionMask & HIT_BOTTOM) != 0)
                 {
                     hitDirection |= HIT_BOTTOM;
                     Box.y = hypoY.y - ((hypoY.y + hypoY.h) - entity.Box.y);
                     moveY = 0;
                     OnGround = true;
                     FrameFriction = 1.0f;
                     break;
                 }
                 else if (moveY < 0.0f && (Box.y > entity.Box.y + entity.Box.h) && (p.CollisionDirectionMask & HIT_TOP) != 0)
                 {
                     hitDirection |= HIT_TOP;
                     Box.y = entity.Box.y + entity.Box.h;
                     moveY = 0;
                     break;
                 }
             }
         }
        */
        /*for (int i = ((int)hypoY.x) / 32; i <= (int)(hypoY.x + hypoY.w) / 32; i++)
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
        }*/

        End:
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

        public virtual void Configure(string configuration)
        {

        }

        /* Friction -> movevec.x -= (movevec.x * friction) */
        public float GetFriction()
        {
            float friction = 0.0f;
            if (!OnGround)
            {
                friction = FrameFriction;
                goto CheckWater;
            }

            int tileX = (int)((Box.x) / 32);
            int tileW = (int)((Box.x + Box.w) / 32);
            int tileY = (int)((Box.y + Box.h + 1.0f) / 32);
            if (tileY >= GameManager.CurrentLevel.Height)
            {
                friction = FrameFriction;
                goto CheckWater;
            }
            for (int i = tileX; i <= tileW; i++)
            {
                friction = Math.Max(GameManager.CurrentLevel.Tiles[i, tileY]?.Friction ?? FrameFriction, friction);
            }

        CheckWater:
            if (InWater)
            {
                friction *= 0.1f;
            }

            return friction;
        }
        public void CheckInWater()
        {
            int tileX = (int)((Box.x + Box.w / 2) / 32);
            int tileY = (int)((Box.y + Box.h / 2) / 32);

            InWater = GameManager.GetWater(tileX, tileY) != null;
        }

        public void CheckFeetInWater()
        {
            int tileX = (int)((Box.x + Box.w / 2) / 32);
            int tileY = (int)((Box.y + Box.h) / 32);

            FeetInWater = GameManager.GetWater(tileX, tileY) != null;
        }

        public Point Midpoint()
        {
            return new(Box.x + Box.w / 2, Box.y + Box.h / 2);
        }

        public Point Origin()
        {
            return new(Box.x, Box.y);
        }

        public float Distance(Entity a)
        {
            return Midpoint().Distance(a.Midpoint());
        }
        public bool AtEdge(int dir)
        {
            if(dir < 1)
            {
                dir = 0;
            }
            int x = (int)Box.x / 32;
            int y = (int)((Box.y + Box.h) / 32);

            var tile = GameManager.GetTile(x + dir, y);

            return tile == null;
        }
    }
}
