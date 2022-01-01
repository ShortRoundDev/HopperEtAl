using Hopper.Game.Tags;
using Hopper.Geometry;
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
        private Rect OpenBox { get; set; }
        public byte CollisionDirectionMask { get; set; } = 0b1111;

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
            OpenBox = new Rect(Box.x - 16, Box.y, Box.w + 32, Box.h);
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
            if (GameManager.MainPlayer.Box.Intersect(OpenBox))
            {
                if (OpenProgress == -1 && (GameManager.MainPlayer.Keys & DoorType) != 0)
                {
                    GameManager.PlayChunk("DoorOpen");
                    OpenProgress = 0;
                    return;
                }
            }
        }
    }
}
