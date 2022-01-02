using Hopper.Game.Attributes;
using Hopper.Game.Entities.Particle;
using Hopper.Game.Entities.Projectiles;
using Hopper.Game.Tags;
using Hopper.Geometry;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities
{
    [EntityId(3006)]
    public class Squid : Entity, Enemy, Killable
    {
        public int Damage { get; set; } = 1;
        public Type[] CollideWith { get; set; } = new Type[] { typeof(Player), typeof(PseudoGeometry) };

        public int StandCoolDown { get; set; } = 200;
        public int WalkCoolDown { get; set; } = 500;
        public int Direction { get; set; } = 1;
        public int Health { get; set; } = 3;
        public int MaxHealth { get; set; } = 3;
        public int DamageBoost { get; set; } = 0;

        public int VolleyCoolDown { get; set; } = 0;
        public int ShotCooldown { get; set; } = 25;
        public int BulletsFired { get; set; } = 0;
        public bool Shooting { get; set; } = false;

        private static string[] DeathMessages = new string[]
        {
            "Fleeglbrogl!",
            "Akhr'a'ghlacha'nach'ra'glach!",
            "Damn you infidel!",
            "Whyyyy",
            "Nooooo",
            "Ack!",
            "I die",
            "Raglachnach!",
            "Jirinkleblach!",
            "GLORCHNOP-!"
        };

        public Squid(int x, int y) : base(GraphicsManager.GetTexture("Squid"), x, y, 32, 48)
        {
            Animate = new Animator()
            {
                SrcRect = new()
                {
                    x = 0,
                    y = 0,
                    w = 32,
                    h = 48
                },
                Rows = 2,
                Columns = 4,
                Animation = 0
            };

            GameManager.TotalEnemies++;
        }

        public override void Draw()
        {
            Look();
            Render.Box(Box.AsFRect(), Animate.GetUVMap(), Texture, SDLFlip);
            (this as Killable).KillableDraw();
        }

        public override void Update()
        {
            (this as Enemy).EnemyUpdate();
            Animate.Update();
            CheckShoot();
            var hit = MoveAndCollide();
            if((hit & HIT_LEFT) != 0 && OnGround)
            {
                if (GameManager.CurrentLevel.Tiles[(int)(Box.x / 32) - 1, (int)(Box.y / 32)] == null)
                {
                    MoveVec.y = -6;
                } else
                {
                    Direction *= -1;
                }
            }
            else if ((hit & HIT_RIGHT) != 0 && OnGround)
            {
                if (GameManager.CurrentLevel.Tiles[(int)(Box.x / 32) + 1, (int)(Box.y / 32)] == null)
                {
                    MoveVec.y = -6;
                }
                else
                {
                    Direction *= -1;
                }
            }

            float gravity = GameManager.Gravity;
            MoveVec.y += gravity;

            Think();
        }

        private void Think()
        {
            if(StandCoolDown > 0)
            {
                Animate.Animation = 0;
                StandCoolDown--;
                MoveVec.x = 0;
                if(StandCoolDown == 0)
                {
                    WalkCoolDown = 500;
                    var r = new Random();
                    Direction = (r.Next(2) * 2) - 1;
                    MoveVec.x = Direction;
                }
            }
            if (WalkCoolDown > 0)
            {
                Animate.Animation = 1;
                MoveVec.x = Direction;
                WalkCoolDown--;
                if(WalkCoolDown == 0)
                {
                    StandCoolDown = 200;
                }
            }
        }

        private void CheckShoot()
        {
            if (!Shooting && VolleyCoolDown <= 0)
            {
                var direction = Math.Sign(GameManager.MainPlayer.Box.x - Box.x);
                if ((direction == 1 && RenderFlip == true) || (direction == -1 && RenderFlip == false))
                {
                    return;
                }
                int i = (int)(Box.x / 32);
                int j = (int)(Box.y / 32);

                Rect r = new Rect(i * 32, j * 32, 32, 48);
                while (i != (int)(GameManager.MainPlayer.Box.x / 32))
                {
                    i += direction;
                    if (GameManager.CurrentLevel.Tiles[i, j] != null)
                    {
                        return;
                    }
                    r.x = i * 32;
                    if (GameManager.MainPlayer.Box.Intersect(r))
                    {
                        Console.WriteLine($"Hit");
                        VolleyCoolDown = 0;
                        ShotCooldown = 0;
                        BulletsFired = 0;
                        Shooting = true;
                    }
                }
            } else
            {
                VolleyCoolDown--;
            }

            if (Shooting)
            {
                if (BulletsFired < 1)
                {
                    if (ShotCooldown <= 0)
                    {
                        GameManager.AddEntity(new EnemyBullet(
                            new Point(RenderFlip ? Box.x : Box.x + Box.w, Box.y + 8),
                            RenderFlip
                        ));
                        BulletsFired++;
                        ShotCooldown = 25;
                    }
                    else
                    {
                        ShotCooldown--;
                    }
                }
                else
                {
                    VolleyCoolDown = 175;
                    Shooting = false;
                }
                return;
            }
            else if (VolleyCoolDown > 0)
            {
                Animate.Animation = 0;
                VolleyCoolDown--;
                return;
            }
        }

        public void OnDamageHandler(Entity e, int damage)
        {
            GameManager.PlayRandomChunk("AlienHurt", 1, 3);
            return;
        }

        public void OnDie()
        {
            GameManager.PlayRandomChunk("AlienDeath", 1, 3);

            Random r = new Random();
            for (int i = 0; i < 10; i++)
            {
                float dir = (float)(2 * (r.Next() % 2)) - 1;
                GameManager.AddEntity(new Gib(
                    this.Box.x,
                    this.Box.y,
                    new()
                    {
                        x = (float)(r.NextDouble() * 2.0) * dir,
                        y = -(float)((r.NextDouble() * 2.0) + 2.0)
                    }
                ));
            }

            UIManager.TextBubble(DeathMessages[r.Next(DeathMessages.Length)], Top, 100);

            GameManager.TotalKilled++;
            return;
        }

    }
}
