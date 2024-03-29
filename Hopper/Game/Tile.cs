﻿using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SDL2;
using Hopper.Graphics;
using Hopper.Geometry;

namespace Hopper.Game
{
    public class Tile : Entity
    {
        public bool Solid { get; set; } = true;
        public float Friction { get; set; } = 1.0f;
        public bool IsWater { get; set; } = false;

        public Tile(int x, int y, UInt16 tileNum) : base(
            GraphicsManager.GetTexture($"{tileNum}"),
            new Rect()
            {
                x = (float)x * 32.0f,
                y = (float)y * 32.0f,
                w = 32.0f,
                h = 32.0f
            }
        )
        {

        }

        public override void Update()
        {

        }
        public override void Draw()
        {
            Render.Box(Box.AsFRect(), Texture);
        }
    }
}
