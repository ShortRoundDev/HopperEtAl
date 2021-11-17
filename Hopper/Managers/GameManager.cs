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

        private static List<Entity> ToDelete { get; set; } = new();
        private static List<Entity> ToAdd { get; set; } = new();

        public static Player MainPlayer { get; set; }
        public static string CurrentLevelPath { get; set; }

        public static GAME_STATE State { get; set; } = GAME_STATE.MAIN_MENU;
        public static bool Quit { get; set; } = false;

        public static void Init()
        {
            InitEntityDefinitions();
        }

        public static void Update()
        {
            if (State == GAME_STATE.IN_GAME)
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
                MainPlayer.Draw();
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

        public static void RestartLevel()
        {
            ToDelete.Clear();
            ToAdd.Clear();
            CurrentLevel = new Level("Assets/Levels/FirstMap.map");
        }

        public static void NewGame()
        {
            CurrentLevel = new Level("Assets/Levels/FirstMap.map");
            State = GAME_STATE.IN_GAME;
        }
    }

    public enum GAME_STATE
    {
        IN_GAME = 0,
        MAIN_MENU = 1
    }
}
