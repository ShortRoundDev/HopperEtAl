using Hopper.Geometry;
using Hopper.Graphics;
using Hopper.Managers;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Tags
{
    public interface Killable
    {
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int DamageBoost { get; set; }

        public void OnDamage(Entity entity, int damage)
        {
            var _this = this as Entity;
            Health -= damage;
            OnDamageHandler(entity, damage);
            if (_this == null)
            {
                return;
            }
            if (Health <= 0)
            {
                GameManager.DeleteEntity(_this);
                OnDie();
                return;
            }
            _this.MoveVec = new Point()
            {
                x = Math.Sign(entity.MoveVec.x) * 3,
                y = -3
            };
        }

        public void KillableDraw()
        {
            var _this = this as Entity;
            if(_this == null)
            {
                return;
            }
            SDL.SDL_FRect healthBar = new()
            {
                x = (_this.Box.x + (_this.Box.w / 2)) - 8,
                y = (_this.Box.y - 4),
                w = (int)(((float)Health) / ((float)MaxHealth) * 16),
                h = 4
            };
            Render.BoxFill(healthBar, new SDL.SDL_Color()
            {
                r = 0x00,
                g = 0xff,
                b = 0x00,
                a = 0x00
            });
        }

        public void OnDamageHandler(Entity e, int damage)
        {
            return;
        }

        public void OnDie()
        {
            return;
        }
    }
}
