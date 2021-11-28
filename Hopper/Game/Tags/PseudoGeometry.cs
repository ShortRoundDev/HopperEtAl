using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Tags
{
    public interface PseudoGeometry
    {
        public byte CollisionDirectionMask { get; set; }
    }
}
