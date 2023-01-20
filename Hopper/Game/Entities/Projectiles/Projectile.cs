using Hopper.Game.Tags;
using Hopper.Geometry;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.Projectiles
{
    public abstract class Projectile : Entity, Enemy
    {
        public Type[] CollideWith { get; set; }
        public int Damage { get; set; }

        protected Projectile(IntPtr Texture, Rect Box, Point MoveVec, Type[] CollideWith) : base(Texture, Box)
        {
            this.MoveVec = MoveVec;
            this.CollideWith = CollideWith;
        }

        public override void Update()
        {
            byte hitDirection = MoveAndCollide();
            if(hitDirection != 0 && !(this.CollideWith?.Any(t => t.IsSubclassOf(typeof(Projectile))) ?? false))
            {
                GameManager.DeleteEntity(this);
            }
            if (this.CollideWith == null || this.CollideWith.Count() == 0)
                return;
            if((this as Enemy).EnemyUpdate()){
                GameManager.DeleteEntity(this);
            }
        }
    }
}
