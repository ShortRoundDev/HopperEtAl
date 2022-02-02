using Hopper.Game.Attributes;
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

namespace Hopper.Game.Entities
{
    [EntityId(3022)]
    public class LilGuy : Entity, Enemy
    {
        int flipCounter { get; set; }  = 0;
        int Enemy.Damage { get; set; } = 0;
        Type[] Enemy.CollideWith { get; set; } = new Type[] { typeof(Player) };

        int dir = -1; // -1 = up, 1 = down
        public LilGuy(int x, int y) : base(GraphicsManager.GetTexture("LilGuy"), new Rect(x * 32.0f + 4, y * 32.0f + 4, 24, 24))
        {
        }

        public override void Draw()
        {
            flipCounter++;

            var renderFlip = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
            renderFlip |= (dir == 1 ? SDL.SDL_RendererFlip.SDL_FLIP_VERTICAL : SDL.SDL_RendererFlip.SDL_FLIP_NONE);
            renderFlip |= ((flipCounter / 500) % 2 == 0 ? SDL.SDL_RendererFlip.SDL_FLIP_HORIZONTAL : SDL.SDL_RendererFlip.SDL_FLIP_NONE);

            Render.Box(Box.AsFRect(), Texture, renderFlip);
        }

        public override void Update()
        {
            (this as Enemy).EnemyUpdate();

            int tileY = (int)((dir == 1
                ? Box.y + Box.h + dir
                : Box.y + dir)/32.0f);
            
            if(GameManager.GetTile((int)(Box.x/32.0f), tileY) != null)
            {
                dir *= -1;
            }
            Box.y += dir;
        }
    }
}
