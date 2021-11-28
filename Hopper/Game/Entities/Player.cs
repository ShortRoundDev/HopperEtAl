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
    [EntityId(3003)]
    public class Player : Entity, Killable
    {

        bool Walking { get; set; } = false;
        bool Jumping { get; set; } = false;

        public int DamageBoost { get; set; } = 0;
        public int Ammo { get; set; } = 0;
        public int Score { get; set; } = 0;
        public int Keys { get; set; }
        public int Health { get; set; } = 3;
        public int MaxHealth { get; set; } = 3;
        private byte LastZoom { get; set; } = 3;
        public bool Shotgun { get; set; } = false;
        public bool CrossHair { get; set; } = false;

        private const float COS_ANGLE = 0.965925f;
        private const float SIN_ANGLE = 0.258819f;

        //animations
        private Dictionary<string, int> animations = new()
        {
            { "ShootingJumping",    0 },
            { "ShootingRunning",    1 },
            { "ShootingStanding",   2 },
            { "DefaultJumping",     3 },
            { "DefaultRunning",     4 },
            { "DefaultStanding",    5 },
            { "DefaultSwimming",    6 },
            { "Wading",             7 },
            { "ShootingSwimming",   8 },
        };

        int shootTimer = 0;

        public Player(int x, int y) : base(
            GraphicsManager.GetTexture("Player"),
            x, y,
            30, 48
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
                    w = 48,
                    h = 48
                },
                Columns = 4,
                Rows = 9,
                Speed = 0.1f
            };
        }

        public override void Draw()
        {
            Look();
            var dst = new SDL.SDL_FRect()
            {
                x = Box.x - 8,
                y = Box.y,
                w = 48,
                h = 48
            };
            if(DamageBoost == 0 || (DamageBoost/3) % 2 != 0)
                Render.Box(dst, Animate.GetUVMap(), Texture, SDLFlip);

            var ammoBoxSrc = new SDL.SDL_Rect()
            {
                x = 0, y = 0,
                w = 12, h = 12
            };

            var ammoBox = new SDL.SDL_Rect()
            {
                x = 11,
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

            var snackBoxSrc = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 24,
                h = 24
            };

            var snackBoxDst = new SDL.SDL_Rect()
            {
                x = SystemManager.Width - 256,
                y = 5,
                w = 72,
                h = 72
            };
            SDL.SDL_RenderCopy(GraphicsManager.Renderer, GraphicsManager.GetTexture("Snacks"), ref snackBoxSrc, ref snackBoxDst);
            blackBox.x = SystemManager.Width - 256 + 100;
            blackBox.w = 128;
            blackBox.y = 24;
            SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, 0, 0, 0, 0xff);
            SDL.SDL_RenderFillRect(GraphicsManager.Renderer, ref blackBox);
            UIManager.DrawString($"{(Score)}", new Point(SystemManager.Width - 256 + 84, 24), 3.0f);
        }

        public override void Update()
        {
            Animate.Update();
            MoveAndCollide();
            CheckZoom();

            string animation = "Standing";
            bool shooting = false;

            HandleDamageBoost();
            HandleShooting(out shooting);
            HandleJump();
            HandleOnGroundInput(out animation);
            animation = HandleSwimming() ?? animation;
            HandleInAir();

            HandleFriction();
            
            if(!OnGround && !InWater)
            {
                animation = "Jumping";
            }

            Animate.Animation = animations[(shooting ? "Shooting" : "Default") + animation];
        }

        private void HandleDamageBoost()
        {
            if (DamageBoost > 0)
            {
                DamageBoost--;
            }
        }
        
        private string HandleSwimming()
        {
            string animation = null;
            if (InWater)
            {
                //this line sucks 
                animation = "Swimming";

                Walking = false;
                if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_LEFT].Down) {
                    MoveVec.x -= 0.1f;
                    Walking = true;
                }
                
                if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_RIGHT].Down)
                {
                    MoveVec.x += 0.1f;
                    Walking = true;
                }
                
                if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_SPACE].Down && InputManager.Keys[(int)Scancodes.SDL_SCANCODE_SPACE].Edge)
                {
                    MoveVec.y -= 2.0f;
                }

                if (MoveVec.x > 2.0f)
                {
                    MoveVec.x = 2.0f;
                }
                
                if (MoveVec.x < -2.0f)
                {
                    MoveVec.x = -2.0f;
                }
                
                if (MoveVec.y < -6.0f)
                {
                    MoveVec.y = -6.0f;
                }

                if (MoveVec.y > 1.5f)
                {
                    MoveVec.y = 1.5f;
                }
            }
            else if (FeetInWater && !OnGround)
            {
                if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_SPACE].Down && InputManager.Keys[(int)Scancodes.SDL_SCANCODE_SPACE].Edge)
                {
                    Console.WriteLine("Jump out of water");
                    MoveVec.y -= 4.0f;
                }
            }
            return animation;
        }

        private void HandleInAir()
        {
            if (!OnGround && !InWater)
            {
                if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_LEFT].Down)
                {
                    MoveVec.x = -2.0f;
                    Walking = true;

                }
                if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_RIGHT].Down)
                {
                    MoveVec.x = 2.0f;
                    Walking = true;

                }
            }
        }

        private void HandleShooting(out bool shooting)
        {
            shooting = false;
            if (shootTimer > 0)
            {
                shooting = true;
                shootTimer--;
            }

            if (Ammo > 0 && InputManager.Keys[(int)Scancodes.SDL_SCANCODE_LCTRL].Down && InputManager.Keys[(int)Scancodes.SDL_SCANCODE_LCTRL].Edge && shootTimer <= 10)
            {
                Ammo--;
                if (!Shotgun)
                {
                    GameManager.AddEntity(
                        new PlayerBullet(new()
                        {
                            x = Box.x + (RenderFlip ? -16 : 16),
                            y = Box.y + 12
                        },
                            RenderFlip
                        )
                    );
                } else
                {
                    float dir = RenderFlip ? -1 : 1;
                    GameManager.AddEntity(
                        new PlayerBullet(
                            new()
                            {
                                x = Box.x + (RenderFlip ? -16 : 16),
                                y = Box.y + 12
                            },
                            new Point(COS_ANGLE * dir, -SIN_ANGLE) * 8
                        )
                    );
                    GameManager.AddEntity(
                        new PlayerBullet(
                            new()
                            {
                                x = Box.x + (RenderFlip ? -16 : 16),
                                y = Box.y + 12
                            },
                            new Point(dir, 0) * 8
                        )
                    );
                    GameManager.AddEntity(
                        new PlayerBullet(
                            new()
                            {
                                x = Box.x + (RenderFlip ? -16 : 16),
                                y = Box.y + 12
                            },
                            new Point(COS_ANGLE * dir, SIN_ANGLE) * 8
                        )
                    );
                }
                shootTimer = 20;
            }

        }

        private void HandleJump()
        {
            float gravity = !InWater
                ? GameManager.Gravity
                : GameManager.Gravity / 4.0f;
            if (Jumping && MoveVec.y < -4 && !InputManager.Keys[(int)Scancodes.SDL_SCANCODE_SPACE].Down)
            {
                MoveVec.y = -4;
//                gravity *= 3.0f;
            }

            MoveVec.y += gravity;
        }

        private void HandleOnGroundInput(out string animation)
        {
            animation = "Standing";

            if (!OnGround || InWater)
            {
                return;
            }


            if (Walking && MoveVec.x != 0.0f)
            {
                animation = "Running";
            }

            Walking = false;

            if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_RIGHT].Down)
            {
                MoveVec.x = 2.5f;
                Walking = true;
            }

            if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_LEFT].Down)
            {
                MoveVec.x = -2.5f;
                Walking = true;
            }
            if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_SPACE].Down && InputManager.Keys[(int)Scancodes.SDL_SCANCODE_SPACE].Edge)
            {
                Jumping = true;
                Console.WriteLine("Jump");
                MoveVec.y = -8.0f;
            }
        }

        private void HandleFriction()
        {
            if (!Walking)
            {
                var friction = 1.0f - GetFriction();
                 MoveVec.x *= friction;
            }
        }

        public void OnDamageHandler(Entity e, int Damage)
        {
            DamageBoost = 100;
            if (!InWater)
            {
                MoveVec.x = -4;
                MoveVec.y = -4;
            }
        }
        public void OnDie()
        {
            UIManager.ShowDeathScreen();
            //GameManager.RestartLevel();
        }

        private void CheckZoom()
        {
            var zoom = GameManager.CurrentLevel.Zoom[(int)(Box.x / 32), (int)(Box.y / 32)];
            GraphicsManager.MainCamera.ScaleTarget = ((float)zoom, (float)zoom);
        }
    }
}
