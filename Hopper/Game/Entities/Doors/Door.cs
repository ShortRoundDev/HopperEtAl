using Hopper.Game.Tags;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Doors
{
    public abstract class Door : Entity, PseudoGeometry
    {
        int DoorType { get; set; } // 1 - r, 2 - g, 4 - b
        bool Open { get; set; } = false;
        int OpenProgress { get; set; } = -1;
        public Door(int x, int y, int doorType) : base(
            GraphicsManager.GetTexture(
                doorType switch
                {
                    1 => "RedDoor",
                    2 => "GreenDoor",
                    4 => "BlueDoor"
                }
            ),
            x, y,
            32, 64
        )
        {
            this.DoorType = doorType;
        }

        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Texture);
        }

        public override void Update()
        {
            if (Open)
            {
                return;
            }
            if(OpenProgress > -1)
            {
                OpenProgress++;
                Box.y--;
            }
            if(OpenProgress >= 64)
            {
                Open = true;
                return;
            }
            foreach (var e in GameManager.CurrentLevel.Entities)
            {
                if (e == this)
                    continue;
                if (e.Box.Intersect(Box))
                {
                    if (e is Player p)
                    {
                        if (OpenProgress == -1 && (p.Keys & DoorType) != 0)
                        {
                            OpenProgress = 0;
                            return;
                        }
                    }
                    if (e.Box.x < Box.x)
                    {
                        e.Box.x = Box.x - e.Box.w - 1;
                    }
                    else if(e.Box.x + e.Box.w > Box.x + Box.w)
                    {
                        e.Box.x = Box.x + Box.w + 1;
                    }
                    e.MoveVec.x = 0;
                }
            }
        }
    }
}
