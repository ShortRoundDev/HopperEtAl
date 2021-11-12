using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Game.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EntityIdAttribute : Attribute
    {
        public UInt16 Id { get; set; }
        public EntityIdAttribute(UInt16 Id)
        {
            this.Id = Id;
        }
    }
}
