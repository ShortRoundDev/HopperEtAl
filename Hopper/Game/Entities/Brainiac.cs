﻿using Hopper.Game.Attributes;
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
        }

        public void OnDie()
        {
            GameManager.TotalKilled++;
        }
    }
}
