using Hopper.Game.Attributes;
using Hopper.Game.Tags;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Teleporter
{
    [EntityId(3004)]
    internal class Teleporter : Entity, Usable
    {
        public float UseDistance { get; set; } = 64.0f;
        public bool Used { get; set; } = false;
        private string NextLevelPath { get; set; } = null;

        public Teleporter(int x, int y) : base(GraphicsManager.GetTexture("Teleporter"), x, y, 32, 64)
        {
            Animate = new Animator()
            {
                SrcRect = new SDL2.SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = 32,
                    h = 64
                },
                Rows = 2,
                Columns = 8,
                Speed = 0.1f,
                Loop = false,
                Animation = 1
            };
        }
        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Animate.GetUVMap(), Texture, SDLFlip);
            (this as Usable).UsableDraw();
        }

        public override void Update()
        {
            Animate.Update();
            (this as Usable).UsableUpdate();
            if(Animate.Animation == 0 && Animate.Finished)
            {
                UIManager.ShowRecap(NextLevelPath);
            }
        }

        public override void Configure(string configuration)
        {
            this.NextLevelPath = configuration;
        }

        public void OnUse()
        {
            Animate.Animation = 0;
            Animate.Frame = 0;
            Animate.Finished = false;
        }
    }
}
