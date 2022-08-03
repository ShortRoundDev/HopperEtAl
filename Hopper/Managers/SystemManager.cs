using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using NLog;
using SDL2;

namespace Hopper.Managers
{
    public static class SystemManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static UInt16 Width { get => Vars.Width; }
        public static UInt16 Height { get => Vars.Height; }
        private static SystemVars Vars { get; set; }
        public static bool Debug { get; set; }
        public static int SfxVolume
        {
            get => _SfxVolume;
            set
            {
                _SfxVolume = value;
            }
        }

        private static int _SfxVolume = 10;
        public static int MusVolume
        {
            get => _MusVolume;
            set
            {
                _MusVolume = value;
                SDL_mixer.Mix_VolumeMusic((byte)(value / 10.0f * 128.0f));
            }
        }
        private static int _MusVolume = 10;

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
