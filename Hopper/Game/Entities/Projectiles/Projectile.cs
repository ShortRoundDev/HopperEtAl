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
    public abstract class Projectile : Entity
    {
        protected Type[] CollideWith { get; set; }
        protected int Damage { get; set; }
        protected Projectile(IntPtr Texture, Rect Box, Point MoveVec, Type[] CollideWith) : base(Texture, Box)
        {
            this.MoveVec = MoveVec;
            this.CollideWith = CollideWith;
        }

        public override void Update()
        {
            byte hitDirection = MoveAndCollide();
            if(hitDirection != 0)
            {
                GameManager.DeleteEntity(this);
            }
            if (this.CollideWith == null || this.CollideWith.Count() == 0)
                return;
            foreach (var entity in GameManager.CurrentLevel.Entities)
            {
                foreach (var type in CollideWith)
                {
                    if (entity.GetType().IsAssignableTo(type))
                    {
                        if (entity.Box.Intersect(Box))
                        {
                            if(entity is Killable k)
                            {
                                k.OnDamage(this, Damage);
                            }
                            GameManager.DeleteEntity(this);
                            return;
                        }
                    }
                }
            }
        }
    }
}
