using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using NLog;

namespace Hopper.Managers
{
    public static class SystemManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static UInt16 Width { get => Vars.Width; }
        public static UInt16 Height { get => Vars.Height; }
        private static SystemVars Vars { get; set; }
        public static bool Debug { get; set; }
        public static void Init()
        {
            Vars = new();
            LoadConfigFile("config.json");
        }

        private static void LoadConfigFile(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                Vars = JsonSerializer.Deserialize<SystemVars>(json);
            }catch(FileNotFoundException)
            {
                Log.Error("Could not load config.json. Got File Not Found");
            }catch(Exception e)
            {
                Log.Error("Could not load config.json. Unknown error: " + e.Message);
            }
        }
    }

    internal class SystemVars
    {
        public UInt16 Width { get; set; } = 1024;
        public UInt16 Height { get; set; } = 768;
    }
}
