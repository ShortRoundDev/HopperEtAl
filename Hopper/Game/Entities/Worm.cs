using Hopper.Game.Attributes;
using Hopper.Game.Tags;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Hopper.Graphics;
using SDL2;

namespace Hopper.Game.Entities
{
    [EntityId(3010)]
    public class Worm : Entity, Enemy, Killable
    {
        public int Damage { get; set; } = 1;
        public Type[] CollideWith { get; set; } = new Type[] { typeof(Player) };
        public int Health { get; set; } = 1;
        public int MaxHealth { get; set; } = 1;
        public int DamageBoost { get; set; } = 0;

        public Worm(int x, int y) : base(GraphicsManager.GetTexture("Worm"), x, y, 24, 24)
        {
            Animate = new Animator()
            {
                SrcRect = new SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = 24,
                    h = 24
                },
                Columns = 2,
                Rows = 1,
                Speed = 0.1f
            };
        }

        public override void Draw()
        {
            Look();
            Render.Box(Box.AsFRect(), Animate.GetUVMap(), Texture, SDLFlip);
            (this as Killable).KillableDraw();
        }

        public override void Update()
        {
            Animate.Update();
            MoveAndCollide();
            (this as Enemy).EnemyUpdate();
            if (InWater)
            {
                if (Distance(GameManager.MainPlayer) < 400)
                {

                    var diff = GameManager.MainPlayer.Origin() - Origin();
                    var vec2 = new Vector2(diff.x, diff.y);
                    vec2 = Vector2.Normalize(vec2);

                    MoveVec.x = vec2.X * 0.7f;
                    MoveVec.y = vec2.Y * 0.7f;
                }
            }
            else
            {
                MoveVec.y += GameManager.Gravity;
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
            return;
        }
    }
}
