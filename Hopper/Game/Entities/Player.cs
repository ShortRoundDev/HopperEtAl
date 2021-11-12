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

using Scancodes = SDL2.SDL.SDL_Scancode;

namespace Hopper.Game.Entities
{
    [EntityId('P')]
    public class Player : Entity, Killable
    {

        bool Walking { get; set; } = false;
        bool Jumping { get; set; } = false;

        int DamageBoost { get; set; } = 0;
        public int Ammo { get; set; } = 0;
        public int Score { get; set; } = 0;
        public int Keys { get; set; }
        public int Health { get; set; } = 3;
        public int MaxHealth { get; set; } = 3;

        //animations
        private Dictionary<string, int> animations = new()
        {
            { "ShootingJumping",    0 },
            { "ShootingRunning",    1 },
            { "ShootingStanding",   2 },
            { "DefaultJumping",     3 },
            { "DefaultRunning",     4 },
            { "DefaultStanding",    5 }
        };

        int shootTimer = 0;

        public Player(int x, int y) : base(
            GraphicsManager.GetTexture("Player"),
            x, y,
            32,
            48
        )
        {
            GameManager.MainPlayer = this;
            GraphicsManager.MainCamera.Track(this);
            Animate = new Animator()
            {
                SrcRect = new SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = 32,
                    h = 48
                },
                Columns = 4,
                Rows = 6,
                Speed = 0.1f
            };
        }

        public override void Draw()
        {
            Look();
            if(DamageBoost == 0 || (DamageBoost/3) % 2 != 0)
                Render.Box(Box.AsFRect(), Animate.GetUVMap(), Texture, SDLFlip);

            var ammoBoxSrc = new SDL.SDL_Rect()
            {
                x = 0, y = 0,
                w = 12, h = 12
            };

            var ammoBox = new SDL.SDL_Rect()
            {
                x = 10,
                y = SystemManager.Height - 56,
                w = 42,
                h = 42
            };

            SDL.SDL_RenderCopy(GraphicsManager.Renderer, GraphicsManager.GetTexture("Ammo"), ref ammoBoxSrc, ref ammoBox);

            var blackBox = new SDL.SDL_Rect()
            {
                x = 60,
                y = SystemManager.Height - 48,
                w = 64,
                h = 36
            };
            SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, 0, 0, 0, 0xff);
            SDL.SDL_RenderFillRect(GraphicsManager.Renderer, ref blackBox);

            UIManager.DrawNumbers($"{Ammo}", new Point(64, SystemManager.Height - 46));
        }

        public override void Update()
        {
            Animate.Update();
            MoveAndCollide();

            if (DamageBoost > 0)
            {
                DamageBoost--;
            }
            else
            {
                foreach(var entity in GameManager.CurrentLevel.Entities)
                {
                    if(entity is Enemy)
                    {
                        if (entity.Box.Intersect(Box))
                        {
                            DamageBoost = 100;
                            MoveVec.x = -4;
                            MoveVec.y = -4;
                        }
                    }
                }
            }

            var moveVec = MoveVec;
            moveVec.y += GameManager.Gravity;
            bool shooting = false;
            if (shootTimer > 0)
            {
                shooting = true;
                shootTimer--;
            }
            string animation = "Standing";
            
            if (Walking && OnGround && MoveVec.x != 0.0f)
            {
                animation = "Running";
            }

            Walking = false;

            if (OnGround)
            {
                moveVec.x *= 0.1f;
            }
            
            if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_RIGHT].Down)
            {
                moveVec.x = 2.0f;
                Walking = true;
            }

            if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_LEFT].Down)
            {
                moveVec.x = -2.0f;
                Walking = true;
            }
            if(InputManager.Keys[(int)Scancodes.SDL_SCANCODE_SPACE].Down && OnGround)
            {
                Jumping = true;
                moveVec.y = -8.0f;
            }
            if (Ammo > 0 && InputManager.Keys[(int)Scancodes.SDL_SCANCODE_LCTRL].Down && shootTimer <= 10)
            {
                Ammo--;
                GameManager.AddEntity(
                    new PlayerBullet(new()
                        {
                            x = Box.x + (RenderFlip ? -16 : 16),
                            y = Box.y + 12
                        },
                        RenderFlip
                    )
                );
                shootTimer = 20;
            }

            if(!OnGround)
            {
                animation = "Jumping";
            }

            Animate.Animation = animations[(shooting ? "Shooting" : "Default") + animation];

            MoveVec = moveVec;
        }
    }
}
