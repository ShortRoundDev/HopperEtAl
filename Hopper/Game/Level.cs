using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SDL2;

using NLog;
using Hopper.Managers;
using LibJohn;
using Hopper.Game.Entities;

namespace Hopper.Game
{
    public class Level
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public UInt16 Width { get; set; }
        public UInt16 Height { get; set; }
        public Tile[,] Background { get; set; }
        public Tile[,] Tiles { get; set; }
        public List<Entity> Entities { get; set; } = new();

        public Level(string path)
        {
            LoadLevel(path);
            GameManager.CurrentLevelPath = path;
        }

        public void Draw()
        {
            SDL.SDL_SetRenderDrawColor(
                GraphicsManager.Renderer,
                0xff, 0, 0, 0xff
            );
            for(int i = 0; i < Width; i++)
            {
                for(int j = 0; j < Height; j++)
                {
                    if(Background[i, j] != null)
                    {
                        Background[i, j].Draw();
                    }
                }
            }
            for(int i = 0; i < Width; i++)
            {
                for(int j = 0; j < Height; j++)
                {
                    var tile = Tiles[i, j];
                    if (tile != null)
                    {
                        tile.Draw();
                    }
                }
            }
            foreach (var entity in Entities)
            {
                if (entity.Deleted || entity is Player)
                    continue;
                entity.Draw();
            }
        }

        public void Update()
        {
            foreach(var entity in Entities)
            {
                if (entity.Deleted)
                    continue;
                entity.Update();
            }
        }

        private void LoadLevel(string path)
        {
            var tileMap = new TileMap();
            tileMap.LoadFile(path);

            Background = new Tile[tileMap.Level.Width, tileMap.Level.Height];
            Tiles = new Tile[tileMap.Level.Width, tileMap.Level.Height];
            Width = tileMap.Level.Width;
            Height = tileMap.Level.Height;
            for (int i = 0; i < tileMap.Level.Width; i++)
            {
                for (int j = 0; j < tileMap.Level.Height; j++)
                {
                    if (tileMap.Level.Walls[i, j].WallType != 0)
                    {
                        Tiles[i, j] = new Tile(i, j, tileMap.Level.Walls[i, j].WallType); ;
                    }
                    if (tileMap.Level.Walls[i, j].Floor != 0)
                    {
                        Background[i, j] = new Tile(i, j, tileMap.Level.Walls[i, j].Floor);
                    }
                }
            }

            Entities = new List<Entity>((int)tileMap.Level.TotalEntities);
            for(UInt64 i = 0; i < tileMap.Level.TotalEntities; i++)
            {
                var entity = GameManager.MakeEntity(
                    tileMap.Level.Entities[i].EntityId,
                    tileMap.Level.Entities[i].X,
                    tileMap.Level.Entities[i].Y,
                    tileMap.Level.Entities[i].Config
                );
                if (entity != null)
                {
                    Entities.Add(entity);
                }
                else
                {
                    Console.Out.WriteLine("Warning: Couldn't instantiate entity!");
                }
            }
            
            return;
        }

        private void _LoadLevel(string path)
        {
            string text = null;
            try
            {
                text = File.ReadAllText(path);
            }
            catch (DirectoryNotFoundException)
            {
                Log.Error($"Could not find {path}");
                return;
            }
            catch (FileNotFoundException)
            {
                Log.Error($"Could not find {path}");
                return;
            }
            catch (Exception e)
            {
                Log.Error($"Couldn't load Level! Unknown error: {e.Message}");
                return;
            }

            var size = CalculateMapSize(text);
            if(size.width == 0 || size.height == 0)
            {
                Log.Error("Couldn't load header!");
                return;
            }
            Width = size.width;
            Height = size.height;

            Tiles = new Tile[Width, Height];

            //int mapStart = text.IndexOf("\n") + 1;
            int mapStart = text.IndexOf("\n") + 1;
            int c = -1; // start low
            for (int i = text.IndexOf("\n") + 1; i < text.Length; i++)
            {
                var code = text[i];

                if (code != '\r' && code != '\n')
                {
                    c++; // iterator
                }
                else
                {
                    continue;
                }

                var x = c % Width;
                var y = c / Width;
                if(code == ' ')
                {
                    Tiles[x, y] = null;
                } else
                {
                    var entity = GameManager.MakeEntity(code, x, y, "");
                    if (entity == null)
                    {
                        Tiles[x, y] = new Tile(x, y, code);
                    }else
                    {
                        Entities.Add(entity);
                    }
                }
            }
        }

        private (UInt16 width, UInt16 height) CalculateMapSize(string map)
        {
            int firstLine = map.IndexOf("\n");
            var header = map.Substring(0, firstLine);
            var parts = header.Split(",").Select(p => p.Trim()).ToArray();
            if(parts.Count() != 2)
            {
                Log.Error($"Could not get level header!");
                return (0, 0);
            }
            if(!UInt16.TryParse(parts[0], out var width))
            {
                Log.Error($"Could not calculate map width!");
                return (0, 0);
            }
            if (!UInt16.TryParse(parts[1], out var height))
            {
                Log.Error($"Could not calculate map width!");
                return (0, 0);
            }

            return (width, height);
        }

    }
}
