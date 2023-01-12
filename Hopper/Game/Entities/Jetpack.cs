using Hopper.Game.Attributes;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities
{
    [EntityId(1006)]
    public class Jetpack : Entity
    {
        bool Visible = true;
        int Cooldown = -1;
        public Jetpack(int x, int y) : base(GraphicsManager.GetTexture("JetPack"), x, y, 32, 32)
        {

        }
        public override void Draw()
        {
            if (!Visible)
            {
                return;
            }
            Render.Box(Box.AsFRect(), Texture);
        }

        public override void Update()
        {
            if (Visible)
            {
                if (Box.Intersect(GameManager.MainPlayer.Box))
                {
                    GameManager.MainPlayer.JetPack = 100;
                    Visible = false;
                    Cooldown = 200;
                }
            }else
            {
                Cooldown--;
                if(Cooldown < 0)
                {
                    Visible = true;
                }
            }
        }
    }
}
