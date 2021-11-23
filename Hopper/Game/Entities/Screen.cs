using Hopper.Game.Attributes;
using Hopper.Game.Tags;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities
{
    [EntityId(3009)]
    public class Screen : Entity, Usable
    {
        public float UseDistance { get; set; } = 48;
        public bool Used { get; set; } = false;
        public string Message { get; set; }

        public Screen(int x, int y) : base(GraphicsManager.GetTexture("Screen"), x, y, 48, 32)
        {

        }

        public override void Configure(string configuration)
        {
            this.Message = configuration;
        }

        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Texture);
            (this as Usable).UsableDraw();
        }

        public override void Update()
        {
            (this as Usable).UsableUpdate();
        }

        public void OnUse()
        {
            UIManager.DisplayMessage(Message);
        }
    }
}
