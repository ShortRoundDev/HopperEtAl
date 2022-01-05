using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hopper.Game;
using Hopper.Game.Entities;
using Hopper.Game.Attributes;
using System.Xml;
using SDL2;
using Hopper.Geometry;

namespace Hopper.Managers
{
    public static class GameManager
    {
        public static Level CurrentLevel { get; set; }
        public static float Gravity { get; set; } = 0.3f;
        private static Dictionary<UInt16, Type> TypeIds { get; set; } = new();
        private static Dictionary<UInt16, Type> TileTypeIds { get; set; } = new();
        private static Dictionary<string, IntPtr> AudioChunks { get; set; } = new();

        private static string SongName { get; set; } = null;
        private static IntPtr Music { get; set; } = IntPtr.Zero;

        private static List<Entity> ToDelete { get; set; } = new();
        private static List<Entity> ToAdd { get; set; } = new();

        public static Player MainPlayer { get; set; }
        public static string CurrentLevelPath { get; set; }
        public static string NextLevelPath { get; set; }

        public static GAME_STATE State { get; set; } = GAME_STATE.MAIN_MENU;
        public static bool Quit { get; set; } = false;

        public static int TotalCollectibles { get; set; }
        public static int TotalCollected { get; set; } = 0;

        public static int TotalEnemies { get; set; }
        public static int TotalKilled { get; set; } = 0;

        public static bool Pause { get; set; } = false;

        public static void Init()
        {
            SDL2.SDL_mixer.Mix_OpenAudio(44100, SDL2.SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048);

            InitEntityDefinitions();
            InitTileDefinitions();
            InitAudioChunks();
        }

        private static void InitFontColors()
        {
            
        }

        public static void Update()
        {
            if (State == GAME_STATE.IN_GAME && !Pause)
            {
                UIManager.UpdateStars();
                CurrentLevel.Update();
                ClearEntities();
                AttachEntities();
            }
        }

        public static void Draw()
        {
            if (State == GAME_STATE.IN_GAME)
            {
                UIManager.DrawStars();
                CurrentLevel.Draw();
            }
        }

        public static void AttachEntity(Entity entity)
        {
            
        }

        public static Entity MakeEntity(UInt16 code, int x, int y, string configuration)
        {
            if(!TypeIds.TryGetValue(code, out var type))
            {
                // error;
                return null;
            }
            var constructor = type.GetConstructor(new Type[] { typeof(int), typeof(int) });
            if(constructor == null)
            {
                // error;
                return null;
            }
            try
            {
                var entity = constructor.Invoke(new object[] { x, y }) as Entity;
                entity.Configure(configuration);
                return entity;
            }catch(Exception e)
            {
                // error;
            }
            return null;
        }

        public static Tile MakeTile(UInt16 code, int x, int y)
        {
            if (!TileTypeIds.TryGetValue(code, out var type))
            {
                // error;
                return new Tile(x, y, code);
            }
            var constructor = type.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(UInt16) });
            if (constructor == null)
            {
                // error;
                return null;
            }
            try
            {
                var tile = constructor.Invoke(new object[] { x, y, code }) as Tile;
                return tile;
            }
            catch (Exception e)
            {
                // error;
            }
            return new Tile(x, y, code);
        }

        public static Tile GetTile(int x, int y)
        {
            if(x < 0 || y < 0 || x >= CurrentLevel.Width || y >= CurrentLevel.Height)
            {
                return null;
            }

            return CurrentLevel.Tiles[x, y];
        }

        public static Tile GetWater(int x, int y)
        {
            if (x < 0 || y < 0 || x >= CurrentLevel.Width || y >= CurrentLevel.Height)
            {
                return null;
            }
            return CurrentLevel.Water[x, y];
        }

        public static void DeleteEntity(Entity entity)
        {
            ToDelete.Add(entity);
        }

        public static void AddEntity(Entity entity)
        {
            ToAdd.Add(entity);
        }

        private static void AttachEntities()
        {
            CurrentLevel.Entities.AddRange(ToAdd);
            ToAdd.Clear();
        }

        private static void ClearEntities()
        {
            CurrentLevel.Entities.RemoveAll(e => ToDelete.Contains(e));
            ToDelete.Clear();
        }

        private static void InitEntityDefinitions()
        {
            var entities = typeof(GameManager)
                .Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(Entity)))
                .Where(t => t.GetCustomAttributes(typeof(EntityIdAttribute), false).Count() > 0);
            foreach(var t in entities)
            {
                var entityIdRef = (t.GetCustomAttributes(typeof(EntityIdAttribute), false).FirstOrDefault() as EntityIdAttribute)?.Id;
                if (!entityIdRef.HasValue)
                {
                    // Todo: warn
                    continue;
                }
                TypeIds.TryAdd(entityIdRef.Value, t);
            }
        }

        private static void InitTileDefinitions()
        {
            var tiles = typeof(GameManager)
                .Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(Tile)))
                .Where(t => t.GetCustomAttributes(typeof(TileIdAttribute), false).Count() > 0);
            foreach (var t in tiles)
            {
                var tileIdRef = (t.GetCustomAttributes(typeof(TileIdAttribute), false).FirstOrDefault() as TileIdAttribute)?.Code;
                if (!tileIdRef.HasValue)
                {
                    // Todo: warn
                    continue;
                }
                TileTypeIds.TryAdd(tileIdRef.Value, t);
            }
        }

        private static void InitAudioChunks()
        {
            var doc = new XmlDocument();
            try
            {
                doc.Load("Assets/AudioPrecache.xml");
            }catch(Exception e)
            {
                return;
            }

            var audioPrecacheNodes = doc
                ?.ChildNodes
                ?.Cast<XmlNode>()
                .FirstOrDefault(node => node.Name == "Chunks")
                ?.ChildNodes
                .Cast<XmlNode>()
                .Where(node => node is XmlElement)
                .Select(node => node as XmlElement)
                .Select(element => new AudioPrecacheNode()
                {
                    Name = element
                        .ChildNodes
                        .Cast<XmlNode>()
                        .FirstOrDefault(node => node is XmlElement e && e.Name == "Name")
                        ?.InnerText,
                    Path = element
                        .ChildNodes
                        .Cast<XmlNode>()
                        .FirstOrDefault(node => node is XmlElement e && e.Name == "Path")
                        ?.InnerText,
                });
            if(audioPrecacheNodes == null)
            {
                return;
            }
            foreach(var node in audioPrecacheNodes)
            {
                var audio = SDL_mixer.Mix_LoadWAV(node.Path);
                if(audio == IntPtr.Zero)
                {
                    // error
                    Console.WriteLine(SDL.SDL_GetError());
                    continue;
                }
                AudioChunks[node.Name] = audio;
            }
        }

        public static IntPtr GetAudio(string name)
        {
            if(!AudioChunks.TryGetValue(name, out IntPtr value))
            {
                return IntPtr.Zero;
            }
            return value;
        }

        public static int PlayChunk(string Name)
        {
            var chunk = GetAudio(Name);
            if (chunk != IntPtr.Zero)
            {
                int channel = SDL_mixer.Mix_PlayChannel(-1, chunk, 0);
                SDL_mixer.Mix_SetDistance(channel, 0);
            }
            return -1;
        }

        public static int PlayChunkAtt(string Name, int x, int y)
        {
            var chunk = GetAudio(Name);
            if (chunk != IntPtr.Zero)
            {
                var a = new Point(x, y);
                var distance = a.Distance(new Point(MainPlayer.Box.x, MainPlayer.Box.y));
                if(distance > 512)
                {
                    return -1;
                }
                int channel = SDL_mixer.Mix_PlayChannel(-1, chunk, 0);
                SDL_mixer.Mix_SetDistance(channel, (byte)(Math.Max(0, Math.Min(distance / 2.0f, 255))));
            }
            return -1;

        }

        public static int PlayChunkAtt(string Name, Entity source)
        {
            return PlayChunkAtt(Name, (int)source.Box.x, (int)source.Box.y);
        }

        public static void PlayMusic(string name)
        {
            if(name == SongName)
            {
                return;
            }
            if(Music != IntPtr.Zero)
            {
                SDL_mixer.Mix_HaltMusic();
            }
            SongName = name;
            Music = SDL_mixer.Mix_LoadMUS(name);
            SDL_mixer.Mix_PlayMusic(Music, -1);
        }

        public static int PlayRandomChunkAtt(string Name, int min, int max, int x, int y, out int rand)
        {
            var a = new Point(x, y);
            var distance = (a.Distance(new Point(MainPlayer.Box.x, MainPlayer.Box.y)));
            if (distance > 512)
            {
                rand = -1;
                return -1;
            }
            var channel = PlayRandomChunk(Name, min, max, out rand);
            if(channel != -1)
            {
                SDL_mixer.Mix_SetDistance(channel, (byte)(Math.Max(0, Math.Min(distance / 2.0f, 255))));
            }
            return channel;
        }


        public static int PlayRandomChunkAtt(string Name, int min, int max, Entity e, out int rand)
        {
            return PlayRandomChunkAtt(Name, min, max, (int)e.Box.x, (int)e.Box.y, out rand);
        }

        public static int PlayRandomChunk(string Name, int min, int max)
        {
            return PlayRandomChunk(Name, min, max, out _);
        }

        public static int PlayRandomChunk(string Name, int min, int max, out int rand)
        {
            var r = new Random();
            rand = r.Next(min, max);
            var chunk = GetAudio(Name + rand);
            if (chunk != IntPtr.Zero)
            {
                return SDL_mixer.Mix_PlayChannel(-1, chunk, 0);
            }
            return -1;
        }

        public static void RestartLevel()
        {
            ToDelete.Clear();
            ToAdd.Clear();
            TotalCollectibles = 0;
            TotalCollected = 0;

            CurrentLevel = new Level(CurrentLevelPath);
        }

        public static void NewGame()
        {
            //CurrentLevel = new Level("Assets/Levels/Spacecamp");
            CurrentLevel = new Level("Assets/Levels/Kierkegaard");
            State = GAME_STATE.IN_GAME;
        }

        public static void NewLevel(string newLevel)
        {
            TotalCollected = 0;
            TotalCollectibles = 0;
            TotalEnemies = 0;
            TotalKilled = 0;

            if(string.IsNullOrEmpty(newLevel))
            {
                CurrentLevel = null;
                State = GAME_STATE.DEMO_END;
                return;
            }

            CurrentLevel = new Level(newLevel);
            State = GAME_STATE.IN_GAME;
        }
        private struct AudioPrecacheNode
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }
    }

    public enum GAME_STATE
    {
        IN_GAME = 0,
        MAIN_MENU = 1,
        DEMO_END = 2
    }
}
