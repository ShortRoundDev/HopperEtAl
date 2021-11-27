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
    [EntityId(3011)]
    public class Barnacles : Entity, Enemy
    {
        public int Damage { get; set; } = 1;
        public Type[] CollideWith { get; set; } = new Type[] { typeof(Player) };

        public Barnacles(int x, int y) : base(GraphicsManager.GetTexture("Barnacles"), x, y, 32, 32)
        {
            Random r = new Random();
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
                Frame = r.Next(4)
            };
        }

        public override void Draw()
        {
            SDL.SDL_RendererFlip flip = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
            if(GameManager.CurrentLevel.Tiles[(int)Box.x/32, (int)(Box.y/32) - 1] == null)
            {
                flip = SDL.SDL_RendererFlip.SDL_FLIP_VERTICAL;
            }
            Render.Box(Box.AsFRect(), Animate.GetUVMap(), Texture, flip);
        }

        public override void Update()
        {
            (this as Enemy).EnemyUpdate();
            Animate.Update();
        }
    }
}
