using Hopper.Game.Attributes;
using Hopper.Game.Entities.Particle;
using Hopper.Game.Tags;
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
    [EntityId(3002)]
    public class Brainiac : Entity, Enemy, Killable
    {
        public int Health { get; set; } = 2;
        public int MaxHealth { get; set; } = 2;
        public Type[] CollideWith { get; set; } = new Type[] { typeof(Player) };
        public int Damage { get; set; } = 1;
        public int DamageBoost { get; set; } = 0;

        private static string[] DeathMessages = new string[]
        {
            "Et tu, brute?",
            "Remember me as a hero",
            "How could this happen to me?",
            "Blork!",
            "Frunklebean!",
            "FUCK",
            "Goodbye, cruel world",
            "Aggydaggy!"
        };

        public Brainiac(int x, int y) : base(GraphicsManager.GetTexture("Brainiac"), x, y, 24, 48)
        {
            MoveVec.x = 1;
            Animate = new Animator()
            {
                Rows = 1,
                Columns = 4,
                Speed = 0.08f,
                SrcRect = new SDL2.SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = 48,
                    h = 48
                }
            };
            GameManager.TotalEnemies++;
        }
        public override void Draw()
        {
            Look();
            var dst = new SDL.SDL_FRect()
            {
                x = Box.x - 12,
                y = Box.y,
                w = 48,
                h = 48
            };
            Render.Box(dst, Animate.GetUVMap(), Texture, SDLFlip);
            Killable k = this;
            k.KillableDraw();
        }

        public override void Update()
        {
            Animate.Update();
            MoveVec.y += GameManager.Gravity;
            var hitSide = MoveAndCollide();
            Think(hitSide);
            (this as Enemy).EnemyUpdate();

    }

    public void Think(byte hitSide)
        {
            if((hitSide & (HIT_LEFT)) != 0) {
                MoveVec.x = 1;
            }

            if ((hitSide & (HIT_RIGHT)) != 0) {
                MoveVec.x = -1;
            }
            if (OnGround)
            {
                MoveVec.x = Math.Sign(MoveVec.x);
            }
        }

        public void OnDamageHandler(Entity e, int Damage)
        {
            int y = (int)e.Box.y;
            var rand = new Random();
            int x = (int)Box.x + rand.Next((int)Box.w);
            var pop = new GreenBubble(x, y);
            GameManager.AddEntity(pop);
            GameManager.PlayRandomChunk("AlienHurt", 1, 3, out _);
        }

        public void OnDie()
        {
            GameManager.TotalKilled++;

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
            GameManager.PlayRandomChunk("AlienDeath", 1, 3, out _);
            UIManager.TextBubble(DeathMessages[r.Next(0, DeathMessages.Count())], Top, 100);
        }
    }
}
