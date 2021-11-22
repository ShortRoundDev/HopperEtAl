using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TileIdAttribute : Attribute
    {
        public UInt16 Code { get; set; }
        public TileIdAttribute(UInt16 code)
        {
            Code = code;
        }
    }
}
