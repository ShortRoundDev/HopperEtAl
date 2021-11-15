using Hopper.Game.Attributes;
using Hopper.Game.Entities.Projectiles;
using Hopper.Game.Tags;
using Hopper.Geometry;
using Hopper.Graphics;
using Hopper.Managers;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities
{
    [EntityId(3001)]
    public class Turret : Entity, Enemy, Killable
    {
        public int Health { get; set; } = 1;
        public int MaxHealth { get; set; } = 1;

        bool Shooting { get; set; } = false;
        int VolleyCoolDown { get; set; } = 0;
        int ShotCooldown { get; set; } = 0;
        int BulletsFired { get; set; } = 0;
        public int Damage { get; set; } = 0;
        public Type[] CollideWith { get; set; } = null;
        public int DamageBoost { get; set; } = 0;

        public Turret(int x, int y) : base(GraphicsManager.GetTexture("Turret"), x, y, 32, 32)
        {
            Animate = new Animator()
            {
                Rows = 2,
                Columns = 4,
                Speed = 0.1f,
                SrcRect = new SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = 32,
                    h = 32
                }
            };
            Animate.Animation = 0;
        }

        public override void Draw()
        {
            if(GameManager.MainPlayer.Box.x < Box.x)
            {
                RenderFlip = true;
            } else
            {
                RenderFlip = false;
            }

            Render.Box(Box.AsFRect(), Animate.GetUVMap(), Texture, SDLFlip);
        }

        public override void Update()
        {
            Animate.Update();

            if (Shooting)
            {
                if (BulletsFired < 3)
                {
                    if(ShotCooldown <= 0)
                    {
                        GameManager.AddEntity(new EnemyBullet(
                            new Point(RenderFlip ? Box.x : Box.x + Box.w, Box.y + 8),
                            RenderFlip
                        ));
                        BulletsFired++;
                        ShotCooldown = 10;
                    }else
                    {
                        ShotCooldown--;
                    }
                }
                else
                {
                    VolleyCoolDown = 50;
                    Shooting = false;
                }
                return;
            }
            else if(VolleyCoolDown > 0)
            {
                Animate.Animation = 0;
                VolleyCoolDown--;
                return;
            }

            var direction = Math.Sign(GameManager.MainPlayer.Box.x - Box.x);
            int i = (int)(Box.x / 32);
            int j = (int)(Box.y / 32);
            Rect r = new Rect(i * 32, j * 32, 32, 32);
            while(i != (int)(GameManager.MainPlayer.Box.x / 32))
            {
                i += direction;
                if(GameManager.CurrentLevel.Tiles[i, j] != null)
                {
                    return;
                }
                r.x = i * 32;
                if (GameManager.MainPlayer.Box.Intersect(r))
                {
                    Animate.Animation = 1;
                    VolleyCoolDown = 0;
                    ShotCooldown = 0;
                    BulletsFired = 0;
                    Shooting = true;
                    return;
                }
            }
        }
    }
}
