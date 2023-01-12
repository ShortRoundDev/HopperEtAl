using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static UInt16 Width { get => Vars.Width ?? 1024; }
        public static UInt16 Height { get => Vars.Height ?? 768; }
        public static String? StartLevel { get => Vars.Level; }
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

        public static void Init(string[] args)
        {
            Vars = new();
            
            SystemVars cliVars = LoadCLIArgs(args);
            LoadConfigFile("config.json");
            ResolveConfigDiff(cliVars, Vars);
        }

        private static SystemVars LoadCLIArgs(string[] args)
        {
            SystemVars cli = new()
            {
                Width = null,
                Height = null,
                Level = null
            };
            string? key = null;
            foreach(var arg in args)
            {
                if (key == null)
                {
                    key = arg;
                }
                else
                {
                    var property = cli.GetType().GetProperty(key.Replace("+", ""));
                    if (property == null)
                    {
                        Console.WriteLine($"Unknown system var '{key}'!");
                    } else
                    {
                        property.SetValue(cli, Convert.ChangeType(arg, property.PropertyType));
                    }
                    key = null;
                    continue;
                }

                if (key.StartsWith("-"))
                {
                    var property = cli.GetType().GetProperty(key.Replace("-", ""));
                    if(property == null) {
                        Console.WriteLine($"Unknown system var '{key}'!");
                        key = null;
                    } else
                    {
                        property.SetValue(cli, true);
                    }
                }
            }
            return cli;
        }

        private static void ResolveConfigDiff(SystemVars cli, SystemVars file)
        {
            foreach(var property in typeof(SystemVars).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = property.GetValue(cli);
                if(value != null)
                {
                    property.SetValue(file, value);
                }
            }
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

    public class SystemVars
    {
        public UInt16? Width { get; set; } = 1024;
        public UInt16? Height { get; set; } = 768;
        public String? Level { get; set; } = null;
    }
}
