using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hopper.Game.Attributes;
using Hopper.Managers;
using SDL2;

namespace Hopper.Game.Entities.WorldVars
{
    [EntityId(3013)]
    public class Song : Entity
    {
        public Song(int x, int y) : base(IntPtr.Zero, 0, 0, 1, 1)
        {
        }

        public override void Configure(string configuration)
        {
            if (!File.Exists(configuration))
            {
                Console.Error.WriteLine("Failed to load song [" + configuration + "]! FNF!");
                return;
            }

            configuration = configuration.Replace("\\", "/");

            GameManager.PlayMusic(configuration);
        }

        public override void Draw()
        {
            
        }

        public override void Update()
        {
        }
    }
}
