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
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities
{
    [EntityId(3014)]
    public class OctoDude : Entity, Enemy, Killable
    {
        public int Damage { get; set; } = 1;
        public int Health { get; set; } = 2;
        public int MaxHealth { get; set; } = 2;
        public int DamageBoost { get; set; } = 0;
        public Type[] CollideWith { get; set; } = new Type[] { typeof(Player) };

        private const float COS_PI_4 = 0.707106f; // sqrt(2)/2, same as sin so just precompute it

        protected int ShootCooldown { get; set; } = 0;

        private static string[] DeathMessages = new string[]
        {
            "I blame Usurpuron-6!",
            "Damn you, biped!",
            "May your tentacles rot!",
            "I ascend to Octo-heaven!",
            "I die for the tentacled one!",
            "Your Octopotence is weak, infidel!",
            "Your flagella is showing!",
            "You will fail, pink infidel!",
            "Burn in Octo-hell, pink one!",
            "Arrgh!",
            "Mutti!",
            "Tell my wives... I loved them..."
        };

        public OctoDude(int x, int y) : base(GraphicsManager.GetTexture("OctoDude"), x, y, 32, 32)
        {
            Animate = new()
            {
                SrcRect = new()
                {
                    x = 0,
                    y = 0,
                    w = 32,
                    h = 32
                },
                Rows = 1,
                Columns = 2,
                Speed = 0.05f
            };
            GameManager.TotalEnemies++;
        }

        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Animate.GetUVMap(), Texture, SDLFlip);
            (this as Killable).KillableDraw();
        }

        public override void Update()
        {
            MoveAndCollide();
            (this as Enemy).EnemyUpdate();
            if (InWater)
            {

                if (Distance(GameManager.MainPlayer) < 600)
                {
                    Animate.Update();
                    var diff = GameManager.MainPlayer.Origin() - Origin();
                    var vec2 = new Vector2(diff.x, diff.y);
                    vec2 = Vector2.Normalize(vec2);

                    MoveVec.x = vec2.X * 0.7f;
                    MoveVec.y = vec2.Y * 0.7f;


                    if (ShootCooldown > 0)
                    {
                        ShootCooldown--;
                        return;
                    }

                    //shoot left
                    if (GameManager.MainPlayer.Box.x < Box.x)
                    {
                        GameManager.AddEntity(new Ink((int)Box.x - 8, (int)(Box.y + (Box.h / 2)), new Point(-COS_PI_4, COS_PI_4) * 1.5f)); // down left
                        GameManager.AddEntity(new Ink((int)Box.x - 8, (int)(Box.y + (Box.h / 2)), new Point(-1.5f, 0))); // left
                        GameManager.AddEntity(new Ink((int)Box.x - 8, (int)(Box.y + (Box.h / 2)), new Point(-COS_PI_4, -COS_PI_4) * 1.5f)); // up left
                    }
                    else // shoot right
                    {
                        GameManager.AddEntity(new Ink((int)(Box.x + Box.w), (int)(Box.y + (Box.h / 2)), new Point(COS_PI_4, COS_PI_4) * 1.5f)); // down right
                        GameManager.AddEntity(new Ink((int)(Box.x + Box.w), (int)(Box.y + (Box.h / 2)), new Point(1.5f, 0))); // right
                        GameManager.AddEntity(new Ink((int)(Box.x + Box.w), (int)(Box.y + (Box.h / 2)), new Point(COS_PI_4, -COS_PI_4) * 1.5f)); // up right
                    }
                    ShootCooldown = 200;
                }
            } else
            {
                MoveVec.y += GameManager.Gravity;
            }
        }

        public void OnDamageHandler(Entity e, int damage)
        {
            GameManager.PlayRandomChunk("AlienHurt", 1, 3);
            int dir = (e.Box.x - Box.x) > 0 ? -1 : 1;

            Random r = new Random();
            for (int i = 0; i < 3; i++)
            {
                GameManager.AddEntity(new Gib(
                    this.Box.x,
                    this.Box.y,
                    new()
                    {
                        x = (float)((r.NextDouble() * 2.0) + 2) * dir,
                        y = -(float)((r.NextDouble()) + 1.0)
                    },
                    UIManager.LT_MAG
                ));
            }

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
                        y = -(float)((r.NextDouble() * 2.0) + 2.0),
                    },
                    UIManager.LT_MAG
                ));
            }

            UIManager.TextBubble(DeathMessages[r.Next(DeathMessages.Length)], Top, 100);

            GameManager.TotalKilled++;

            return;
        }
    }
}
