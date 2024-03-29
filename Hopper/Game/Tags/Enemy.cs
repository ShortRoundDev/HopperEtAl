﻿using Hopper.Geometry;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Tags
{
    public interface Enemy
    {
        public int Damage { get; set; }
        protected Type[] CollideWith { get; set; }

        public bool EnemyUpdate(bool IgnoreBoost = false)
        {
            var _this = this as Entity;
            return EnemyUpdate(_this.Box, IgnoreBoost);
        }
        public bool EnemyUpdate(Rect box, bool IgnoreBoost = false)
        {
            var _this = this as Entity;
            foreach (var entity in GameManager.CurrentLevel.Entities)
            {
                if(entity == this || (entity.GetType() == this.GetType()))
                {
                    continue;
                }
                foreach (var type in CollideWith)
                {
                    if (entity.GetType().IsAssignableTo(type))
                    {
                        if (entity.Box.Intersect(_this.Box))
                        {
                            if (entity is Killable k && (k.DamageBoost == 0 || IgnoreBoost))
                            {
                                k.OnDamage(_this, Damage);
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
