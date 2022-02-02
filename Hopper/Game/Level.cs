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
using Hopper.Game.Tags;

namespace Hopper.Game
{
    public class Level
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public UInt16 Width { get; set; }
        public UInt16 Height { get; set; }
        public Tile[,] Background { get; set; }
        public Tile[,] Tiles { get; set; }
        public Tile[,] Water { get; set; }
        public byte[,] Zoom { get; set; }
        public List<Entity> Entities { get; set; } = new();
        public int DeathTimer = -1;

        public Level(string path)
        {
            LoadLevel(path);
            GameManager.CurrentLevelPath = path;
            DeathTimer = -1;
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

            foreach (var entity in Entities)
            {
                if (entity.Deleted || entity is Player)
                    continue;
                entity.Draw();
            }
            if (!GameManager.MainPlayer.Dead)
            {
                GameManager.MainPlayer.Draw();
            }

            for (int i = 0; i < Width; i++)
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
            SDL.SDL_SetRenderDrawBlendMode(GraphicsManager.Renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
            
            for(int i = 0; i < Width; i++)
            {
                for(int j = 0; j < Height; j++)
                {
                    var water = Water[i, j];
                    if(water != null)
                    {
                        SDL.SDL_SetTextureBlendMode(water.Texture, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
                        SDL.SDL_SetTextureAlphaMod(water.Texture, 128);
                        water.Draw();
                    }
                }
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

            if (DeathTimer > 0)
            {
                DeathTimer--;
            }
            else if (DeathTimer == 0)
            {
                DeathTimer = -1;
                UIManager.ShowDeathScreen();
            }
            for(int i = 0; i < Width; i++)
            {
                for(int j = 0; j < Height; j++)
                {
                    Tiles[i, j]?.Update();
                }
            }
        }

        private void LoadLevel(string path)
        {
            var tileMap = new TileMap();
            tileMap.LoadFile(path);

            Background = new Tile[tileMap.Level.Width, tileMap.Level.Height];
            Tiles = new Tile[tileMap.Level.Width, tileMap.Level.Height];
            Water = new Tile[tileMap.Level.Width, tileMap.Level.Height];
            Zoom = new byte[tileMap.Level.Width, tileMap.Level.Height];

            Width = tileMap.Level.Width;
            Height = tileMap.Level.Height;
            for (int i = 0; i < tileMap.Level.Width; i++)
            {
                for (int j = 0; j < tileMap.Level.Height; j++)
                {
                    if (tileMap.Level.Walls[i, j].WallType != 0)
                    {
                        Tiles[i, j] = GameManager.MakeTile(tileMap.Level.Walls[i, j].WallType, i, j);
                    }
                    if (tileMap.Level.Walls[i, j].Floor != 0)
                    {
                        Background[i, j] = GameManager.MakeTile(tileMap.Level.Walls[i, j].Floor, i, j);
                    }
                    if(tileMap.Level.Walls[i, j].Ceiling != 0)
                    {
                        Water[i, j] = GameManager.MakeTile(tileMap.Level.Walls[i, j].Ceiling, i, j);
                    }
                    var zoom = tileMap.Level.Walls[i, j].Zone;
                    if(zoom == 0)
                    {
                        zoom = 3;
                    }
                    Zoom[i, j] = zoom;
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

            Entities.Sort((a, b) =>
            {
                var _a = a is PseudoGeometry;
                var _b = b is PseudoGeometry;
                if (_a && !_b)
                {
                    return -1;
                }
                if (!_a && _b)
                {
                    return 1;
                }
                return 0;
            });
            
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
