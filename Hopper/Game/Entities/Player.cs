﻿using Hopper.Game.Attributes;
using Hopper.Game.Entities.Geometry;
using Hopper.Game.Entities.Particle;
using Hopper.Game.Entities.Projectiles;
using Hopper.Game.Tags;
using Hopper.Geometry;
using Hopper.Graphics;
using Hopper.Managers;
using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using static Hopper.Managers.InputManager;

namespace Hopper.Game.Entities
{
    [EntityId(3003)]
    public class Player : Entity, Killable
    {

        bool Walking { get; set; } = false;
        public bool Jumping { get; set; } = false;

        public int DamageBoost { get; set; } = 0;
        public int Ammo { get; set; } = 0;
        public int Score { get; set; } = 0;
        public int Keys { get; set; }
        public int Health { get; set; } = 3;
        public int MaxHealth { get; set; } = 3;
        private byte LastZoom { get; set; } = 3;
        public bool Shotgun { get; set; } = false;
        public bool CrossHair { get; set; } = false;
        public int JetPack { get; set; } = -1;
        private IntPtr JetPackTexture { get; set; } = IntPtr.Zero;
        private IntPtr PistolArm { get; set; } = IntPtr.Zero;
        private IntPtr ShotgunArm { get; set; } = IntPtr.Zero;

        private CameraTracker Tracker { get; set; }
        private float TrackerProgress { get; set; }

        private int FallingScream { get; set; } = -1;
        public Platform PlatformParent { get; set; }

        public bool Dead { get; set; } = false;

        private double rot = 0.0;

        private const float COS_ANGLE = 0.965925f;
        private const float SIN_ANGLE = 0.258819f;

        private static string[] DeathMessages = new string[]
        {
            "Mommy!",
            "I'm too young to die!",
            "I didn't fill out my life insurance!",
            "Someone feed my cat!",
            "I die a virgin!",
            "Ow",
            "Science damn it!",
            "Don't blame me, I voted for Kodos!",
            "Por que!",
            "Ach! Mein Leben!",
            "Clear my browser history!"
        };

        //animations
        private Dictionary<string, int> animations = new()
        {
            /*{ "ShootingPistolJumping",    0  },
            { "ShootingPistolRunning",    1  },
            { "ShootingPistolStanding",   2  },
            { "DefaultPistolJumping",     3  },
            { "DefaultPistolRunning",     4  },
            { "DefaultPistolStanding",    5  },
            { "DefaultPistolSwimming",    6  },
            { "PistolWading",             7  },
            { "ShootingPistolSwimming",   8  },

            { "ShootingShotgunRunning",   9  },
            { "DefaultShotgunRunning",    9  },

            { "ShootingShotgunStanding",  10 },
            { "DefaultShotgunStanding",   10 },

            { "ShootingShotgunJumping",   11 },
            { "DefaultShotgunJumping",    11 },

            { "ShootingShotgunSwimming",  12 },
            { "DefaultShotgunSwimming",   12 },

            { "ShotgunWading",            13 },*/
            { "Jumping",  0 },
            { "Running",  1 },
            { "Standing", 2 }
        };

        int shootTimer = 0;

        public Player(int x, int y) : base(
            GraphicsManager.GetTexture("Player"),
            x, y,
            30, 48
        )
        {
            GameManager.MainPlayer = this;
            Tracker = new((int)Box.x, (int)Box.y);
            GraphicsManager.MainCamera.Track(Tracker);
            
            JetPackTexture = GraphicsManager.GetTexture("JetPack");
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
            PistolArm = GraphicsManager.GetTexture("PistolArm");
            ShotgunArm = GraphicsManager.GetTexture("ShotgunArm");
        }

        public override void Draw()
        {
            if (Dead)
            {
                return;
            }
            //Look();
            var dst = new SDL.SDL_FRect()
            {
                x = Box.x - 8,
                y = Box.y,
                w = 48,
                h = 48
            };
            if (DamageBoost == 0 || (DamageBoost / 3) % 2 != 0)
            {
                DrawJetPack();
                Render.Box(dst, Animate.GetUVMap(), Texture, SDLFlip);
                DrawArm((int)dst.x, (int)dst.y);
            }
        }

        private Point GetArmPos()
        {
            return new Point(
                Box.x - 8 + 18 + (RenderFlip ? 9 : 0),
                Box.y + 11
            );
        }

        private Vector2 GetMouseDiff()
        {
            var armPos = GetArmPos();
            var transformed = Render.FRectPerspective(new SDL.SDL_FRect()
            {
                x = armPos.x,
                y = armPos.y
            });

            return new Vector2(MousePos.X, MousePos.Y) - new Vector2(transformed.x, transformed.y);
        }

        private SDL.SDL_Point GetArmPivot()
        {
            return new SDL.SDL_Point()
            {
                x = (int)(1.0f * GraphicsManager.MainCamera.Scale.x),
                y = (int)(5.0f * GraphicsManager.MainCamera.Scale.y)
            };
        }

        private void GetAngles(Vector2 diff, out double rads, out double angle)
        {
            rads = Math.Atan2(diff.Y, diff.X);
            angle = rads * (180.0 / Math.PI);

        }

        private void DrawArm(int x, int y)
        {
            var pistolSrc = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = Shotgun ? 35 : 22,
                h = 10
            };

            var armPos = GetArmPos();

            var pistolDst = new SDL.SDL_FRect()
            {
                x = armPos.x,
                y = armPos.y,
                w = Shotgun ? 35 : 22,
                h = 10
            };
            var center = GetArmPivot();

            var pdst = Render.FRectPerspective(pistolDst);
            var pdstI = new SDL.SDL_Rect()
            {
                x = (int)pdst.x,
                y = (int)pdst.y,
                w = (int)pdst.w,
                h = (int)pdst.h,
            };

            GetAngles(GetMouseDiff(), out _, out double angle);

            SDL.SDL_RenderCopyEx(GraphicsManager.Renderer, Shotgun ? ShotgunArm : PistolArm, ref pistolSrc, ref pdstI, angle, ref center, RenderFlip ? SDL.SDL_RendererFlip.SDL_FLIP_VERTICAL : SDL.SDL_RendererFlip.SDL_FLIP_NONE);

        }

        void DrawJetPack()
        {
            if(JetPack > 0)
            {
                SDL.SDL_FRect jetPackBox = new()
                {
                    x = Box.x,
                    y = Box.y,
                    w = 32,
                    h = 32
                };
                Render.Box(jetPackBox, JetPackTexture, !RenderFlip ? SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL : SDL.SDL_RendererFlip.SDL_FLIP_NONE);
            }
        }

        private void UpdateCamera()
        {
            if(JetPack > 0)
            {
                if(TrackerProgress < 1.0f)
                {
                    TrackerProgress += 0.1f;
                    if(TrackerProgress > 1.0f)
                    {
                        TrackerProgress = 1.0f;
                    }
                }
            } else
            {
                if(TrackerProgress > 0.0f)
                {
                    TrackerProgress -= 0.01f;
                    if (TrackerProgress < 0.0f)
                    {
                        TrackerProgress = 0.0f;
                    }
                }
            }

            Tracker.Box.y = Box.y + + Box.h/2.0f + (MoveVec.y * 10.0f * TrackerProgress);

            Tracker.Box.x = Box.x + Box.w/2;
        }

        public override void Update()
        {
            if (UIManager.RecapShowing)
            {
                return;
            }
            Animate.Update();
            MoveAndCollide();
            UpdateJetPack();
            UpdateCamera();

            CheckZoom();

            HandlePlatform();

            if (OnGround)
            {
                FallingScream = -1;
            }

            if(MoveVec.y > 15 && FallingScream == -1)
            {
                FallingScream = GameManager.PlayRandomChunk("Fall", 1, 3);
            }

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

            var gun = Shotgun ? "Shotgun" : "Pistol";

            //Animate.Animation = animations[((shooting ? "Shooting" : "Default") + gun) + animation];
            if(animations.TryGetValue(animation, out var anim))
            {
                Animate.Animation = anim;
            } else
            {
                Animate.Animation = 2;
            }
            
        }

        private void UpdateJetPack()
        {
            if(JetPack < 0)
            {
                return;
            }

            JetPack--;
            OnGround = false;
            MoveVec.y -= 0.5f;
            if(MoveVec.y < -10)
            {
                MoveVec.y = -10;
            }

            if(JetPack % 7 == 0)
            {
                GameManager.AddEntity(new Smoke((int)Box.x + (RenderFlip ? 20 : 0), (int)Box.y + 32));
            }
        }

        private void HandlePlatform()
        {
            var platform = HitEntitiesThisFrame.Where(ent => ent is Platform).FirstOrDefault() as Platform;

            //remove platform
            if (platform == null && PlatformParent != null)
            {
                PlatformParent.Captured.Remove(this);
                PlatformParent = null;
                Impulse.x = 0;
                Impulse.y = 0;
            } else if(platform != null && PlatformParent == null)
            {
                OnGround = true;
                platform.Captured.Add(this);
                PlatformParent = platform;
            } else if(platform != null && PlatformParent != null && platform != PlatformParent)
            {
                OnGround = true;
                PlatformParent.Captured.Remove(this);
                platform.Captured.Add(this);
                PlatformParent = platform;
            }
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
                if (IsDown(Input.Left) && MoveVec.x > -2.0f) {
                    MoveVec.x -= 0.1f;
                    Walking = true;
                    RenderFlip = true;
                }
                
                if (IsDown(Input.Right) && MoveVec.x < 2.0f)
                {
                    MoveVec.x += 0.1f;
                    Walking = true;
                    RenderFlip = false;
                }
                
                if (EdgeDown(Input.Jump))
                {
                    MoveVec.y -= 2.0f;
                }
                /*
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
                }*/

                if (MoveVec.y > 1.5f)
                {
                    MoveVec.y = 1.5f;
                }
            }
            else if (FeetInWater && !OnGround)
            {
                if (EdgeDown(Input.Jump))
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
                if (IsDown(Input.Left) && MoveVec.x > -2.0f)
                {
                    MoveVec.x -= 0.4f;
                    Walking = true;
                    RenderFlip = true;
                }
                if (IsDown(Input.Right) && MoveVec.x < 2.0f)
                {
                    Console.WriteLine(MoveVec.x);
                    MoveVec.x += 0.4f;
                    Walking = true;
                    RenderFlip = false;
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

            if (Ammo > 0 && InputManager.MouseDown && InputManager.MouseEdge && shootTimer <= 10)
            {
                var dst = new SDL.SDL_FRect()
                {
                    x = Box.x - 8,
                    y = Box.y,
                    w = 48,
                    h = 48
                };


                var pistolDst = new SDL.SDL_FRect()
                {
                    x = dst.x + 18 + (RenderFlip ? 9 : 0),
                    y = dst.y + 11 + (RenderFlip ? 3 : 0),
                    w = 22,
                    h = 6
                };
                var pdst = Render.FRectPerspective(pistolDst);

                Ammo--;

                var armPos = GetArmPos();
                GetAngles(GetMouseDiff(), out double rads, out _);
                var end = new Point((float)Math.Cos(rads) * (Shotgun ? 35.0f : 22.0f) + armPos.x, (float)Math.Sin(rads) * (Shotgun ? 35.0f : 22.0f) + armPos.y);

                if (!Shotgun)
                {
                    GameManager.PlayChunk("Shoot");
                    GameManager.AddEntity(
                        new PlayerBullet(
                            end,
                            new Point()
                            {
                                x = (float)Math.Cos(rads) * 8.0f,
                                y = (float)Math.Sin(rads) * 8.0f
                            }
                        )
                    );
                } else
                {
                    GameManager.PlayChunk("ShotgunShoot");

                    float dir = RenderFlip ? -1 : 1;
                    MoveVec.x -= dir * 3;
                    for (int i = -1; i < 2; i++)
                    {
                        Console.WriteLine(Math.Sin(rads + (((float)i) * 0.2f)));
                        GameManager.AddEntity(
                            new PlayerBullet(
                                end,
                                new Point()
                                {
                                    x = (float)Math.Cos(rads + (((float)i) * 0.2f)) * 8.0f,
                                    y = (float)Math.Sin(rads + (((float)i) * 0.2f)) * 8.0f
                                }
                            )
                        );
                    }
                }
                shootTimer = 20;
            }
        }

        private void HandleJump()
        {
            float gravity = !InWater
                ? GameManager.Gravity
                : GameManager.Gravity / 4.0f;
            if (JetPack < 0 && Jumping && MoveVec.y < -4 && !IsDown(Input.Jump))
            {
                Console.WriteLine("Jumping");
                MoveVec.y = -4;
//                gravity *= 3.0f;
            }

            if (JetPack < 0)
            {
                MoveVec.y += gravity;
            }
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

            if (IsDown(Input.Right))
            {
                MoveVec.x = 2.5f;
                Walking = true;
                Tracker.Box.x = (Box.x + Box.w/2) + 64;
                RenderFlip = false;
                Console.WriteLine(MoveVec.x);
            }

            if (IsDown(Input.Left))
            {
                MoveVec.x = -2.5f;
                Walking = true;
                Tracker.Box.x = (Box.x + Box.w / 2) - 64;
                RenderFlip = true;
            }
            if (EdgeDown(Input.Jump))
            {
                Jumping = true;
                Console.WriteLine("Jump");
                MoveVec.y = -8.0f;
                GameManager.PlayRandomChunk("Grunt", 1, 4);
            }
        }

        private void HandleFriction()
        {
            float modifier = 0.0f;
            if(Shotgun && shootTimer > 0)
            {
                modifier = 0.8f;
            }

            if (!Walking)
            {
                var friction = 1.0f - Math.Max(0, (GetFriction() - modifier));
                MoveVec.x *= friction;
            }
        }

        public void OnDamageHandler(Entity e, int Damage)
        {
            DamageBoost = 100;
            if (Health > 0)
            {
                GameManager.PlayRandomChunk("Hurt", 1, 3);
            }
            if (!InWater)
            {
                
                MoveVec.x = -Math.Sign(MoveVec.x) * 4;
                MoveVec.y = -4;
            }
        }
        public void OnDie()
        {
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
            Dead = true;
            UIManager.TextBubble(DeathMessages[r.Next(DeathMessages.Length)], Top, 150);
            GameManager.CurrentLevel.DeathTimer = 150;
            GameManager.PlayRandomChunk("Death", 1, 3);
        }

        private void CheckZoom()
        {
            var zoom = GameManager.CurrentLevel.Zoom[(int)(Box.x / 32), (int)(Box.y / 32)];
            GraphicsManager.MainCamera.ScaleTarget = ((float)zoom, (float)zoom);
        }
    }
}
