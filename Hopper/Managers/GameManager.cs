using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hopper.Game;
using Hopper.Game.Entities;
using Hopper.Game.Attributes;

namespace Hopper.Managers
{
    public static class GameManager
    {
        public static Level CurrentLevel { get; set; }
        public static float Gravity { get; set; } = 0.3f;
        private static Dictionary<UInt16, Type> TypeIds { get; set; } = new();
        private static Dictionary<UInt16, Type> TileTypeIds { get; set; } = new();

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
            InitEntityDefinitions();
            InitTileDefinitions();            
        }

        private static void InitFontColors()
        {
            
        }

        public static void Update()
        {
            if (State == GAME_STATE.IN_GAME && !Pause)
            {
                CurrentLevel.Update();
                ClearEntities();
                AttachEntities();
            }
        }

        public static void Draw()
        {
            if (State == GAME_STATE.IN_GAME)
            {
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
                var tile = constructor.Invoke(new object[] { x, y }) as Tile;
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
                TypeIds.TryAdd(tileIdRef.Value, t);
            }
        }

        public static void RestartLevel()
        {
            ToDelete.Clear();
            ToAdd.Clear();
            CurrentLevel = new Level(CurrentLevelPath);
        }

        public static void NewGame()
        {
            CurrentLevel = new Level("Assets/Levels/waterleveltmp");
            State = GAME_STATE.IN_GAME;
        }

        public static void NewLevel(string newLevel)
        {
            TotalCollected = 0;
            TotalCollectibles = 0;
            TotalEnemies = 0;
            TotalKilled = 0;

            CurrentLevel = new Level(newLevel);
            State = GAME_STATE.IN_GAME;
        }
    }

    public enum GAME_STATE
    {
        IN_GAME = 0,
        MAIN_MENU = 1
    }
}
