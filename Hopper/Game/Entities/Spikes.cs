using Hopper.Game.Attributes;
using Hopper.Game.Tags;
using Hopper.Geometry;
using Hopper.Graphics;
using Hopper.Managers;
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
            Render.Box(Box.AsFRect(), Texture);
        }

        public override void Update()
        {
            (this as Enemy).EnemyUpdate(CollisionBox);
        }
    }
}
