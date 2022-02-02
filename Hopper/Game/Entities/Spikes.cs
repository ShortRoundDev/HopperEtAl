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
    [EntityId(3000)]
    public class Spikes : Entity, Enemy
    {

        public int Damage { get; set; } = 3;
        public Type[] CollideWith { get; set; } = new Type[] { typeof(Killable) };
        public Rect CollisionBox { get; set; }

        public Spikes(int x, int y) : base(GraphicsManager.GetTexture("Spikes"), x, y, 32, 32)
        {
            CollisionBox = new Rect(x * 32 + 7, y * 32 + 3, 18, 2);
        }

        public override void Draw()
        {
            SDL.SDL_RendererFlip flip = SDL.SDL_RendererFlip.SDL_FLIP_VERTICAL;
            if (GameManager.CurrentLevel.Tiles[(int)Box.x / 32, (int)(Box.y / 32) + 1] != null)
            {
                flip = SDL.SDL_RendererFlip.SDL_FLIP_NONE;
            }

            SDL.SDL_Rect uv = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 32,
                h = 32
            };

            Render.Box(Box.AsFRect(), uv, Texture, flip);
        }

        public override void Update()
        {
            (this as Enemy).EnemyUpdate(CollisionBox);
        }
    }
}
