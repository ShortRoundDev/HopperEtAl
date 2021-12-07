using Hopper.Game.Attributes;
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
    [EntityId(3007)]
    public class Furnace : Entity, PseudoGeometry
    {
        public int CountDown { get; set; }
        public int MaxCountDown { get; set; } = 500;

        public bool Shooting { get; set; } = false;
        public int FiresSet { get; set; } = 0;
        public int MaxFires { get; set; } = 3;
        public int FireTimer { get; set; } = 0;
        public int Direction { get; set; } = 1;
        public int ResetTimer { get; set; } = 500;
        public int FireLength { get; set; } = 100;
        public byte CollisionDirectionMask { get; set; } = 15; // all

        public Furnace(int x, int y) : base(GraphicsManager.GetTexture("Furnace"), x, y, 32, 32)
        {
            Animate = new()
            {
                SrcRect = new SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = 32,
                    h = 32
                },
                Rows = 1,
                Columns = 4,
                Speed = 0.1f,
                Loop = false
            };
        }
        /*
         * Direction
         * Initial Countdown
         * Total fireballs
         * Max countdown
         * Lifetime of fireball
         */
        public override void Configure(string configuration)
        {
            if(configuration == null)
            {
                return;
            }
            string[] parts = configuration.Split("\n");
            if (parts.Count() < 4)
            {
                return;
            }
            if(parts[0].ToLower() == "left")
            {
                Direction = -1;
            }
            else
            {
                Direction = 1;
            }

            if(Int32.TryParse(parts[1], out Int32 countDown))
            {
                CountDown = countDown;
            }
            else
            {
                CountDown = 0;
            }

            if(Int32.TryParse(parts[2], out Int32 maxFires))
            {
                MaxFires = maxFires;
            } else
            {
                maxFires = 3;
            }

            if(Int32.TryParse(parts[3], out Int32 fireInterval))
            {
                MaxCountDown = fireInterval;
                ResetTimer = MaxCountDown;
            }else
            {
                MaxCountDown = 500;
            }

            FireLength = MaxCountDown - (MaxFires * 10);

            if (parts.Length > 4)
            {
                if (Int32.TryParse(parts[4], out Int32 fireTimer))
                {
                    FireLength = fireTimer;
                }
            }
        }

        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Animate.GetUVMap(), Texture, SDLFlip);
        }

        public override void Update()
        {
            if (!Shooting)
            {
                if(CountDown > 0)
                {
                    if(CountDown < 50)
                    {
                        Animate.Update();
                    }
                    CountDown--;
                    return;
                }
                else
                {
                    Console.WriteLine("Shooting " + Direction);
                    Shooting = true;
                }
            }
            else
            {
                if (ResetTimer > 0)
                {
                    if (FiresSet < MaxFires)
                    {
                        if (FireTimer > 0)
                        {
                            FireTimer--;
                            return;
                        }
                        else
                        {
                            FireTimer = 10;
                            FiresSet++;
                            GameManager.AddEntity(new Fire(Box.x + (Direction * FiresSet * 32), Box.y, FireLength));
                        }
                    }
                    ResetTimer--;
                }
                else
                {
                    Animate.Frame = 0;
                    Animate.Finished = false;

                    Console.WriteLine("Stopping " + Direction);

                    ResetTimer = MaxCountDown;
                    Shooting = false;
                    CountDown = MaxCountDown;
                    FiresSet = 0;
                    Animate.Finished = false;
                    Animate.Frame = 0;
                }
            }
        }
    }
}
