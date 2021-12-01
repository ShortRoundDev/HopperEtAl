using Hopper.Game.Attributes;
using Hopper.Graphics;
using Hopper.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Entities.WorldVars
{
    [EntityId(4000)]
    public class LevelName : Entity
    {
        public LevelName(int x, int y) : base(IntPtr.Zero, x, y, 1, 1)
        {

        }

        public override void Configure(string configuration)
        {
            UIManager.ShowLevelName(configuration);
        }

        public override void Draw()
        {
            return;
        }

        public override void Update()
        {
            return;
        }
    }
}
