﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.Linq;

using Hopper.Graphics;
using NLog;
using SDL2;

namespace Hopper.Managers
{
    public static class GraphicsManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static IntPtr Renderer { get; set; }
        public static IntPtr Window { get; set; }
        public static Camera MainCamera { get; set; }
        public static Dictionary<string, IntPtr> Textures { get; set; } = new();

        public static void Init(UInt16 width, UInt16 height)
        {
            IntPtr  _Window     = IntPtr.Zero,
                    _Renderer   = IntPtr.Zero;
            SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_DRIVER, "opengl");
            SDL.SDL_CreateWindowAndRenderer(width, height, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL, out _Window, out _Renderer);
            Window = _Window;
            Renderer = _Renderer;
            MainCamera = new Camera()
            {
                Position = new() { x = 32.0f, y = 32.0f },
                Scale = new() { x = 3.0f, y = 3.0f }
            };

            InitTextures("Assets/TexturePrecache.xml");
        }

        public static void Draw()
        {
        }

        public static void Update()
        {
            MainCamera.Update();
        }

        public static IntPtr GetTexture(string name)
        {
            if(!(Textures.TryGetValue(name, out var texture) || Textures.TryGetValue("Missing", out texture)))
            {
                Log.Error($"Failed to load either {name} or missing texture placeholder!");
                return IntPtr.Zero;
            }
            return texture;
        }

        public static void InitTextures(string cacheFile)
        {
            string text = null;
            try
            {
                text = File.ReadAllText(cacheFile);
            }
            catch (Exception e)
            {
                Log.Error($"Couldn't load texture precache file! Got {e.Message}");
                return;
            }
            var doc = new XmlDocument();
            try
            {
                doc.LoadXml(text);
            }
            catch (Exception e)
            {
                Log.Error($"Couldn't parse texture precache file! Got {e.Message}");
                return;
            }

            var texturePrecacheNodes = doc
                ?.ChildNodes
                .Cast<XmlNode>()
                .FirstOrDefault(node => node.Name == "Textures")
                ?.ChildNodes
                .Cast<XmlNode>()
                .Where(node => node is XmlElement)
                .Select(node => node as XmlElement)
                .Select(element => new TexturePrecacheNode()
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
                            ?.InnerText
                    }
                );
            if(texturePrecacheNodes == null)
            {
                Log.Error($"Could not find precache texture nodes in {cacheFile}!");
                return;
            }

            foreach (var node in texturePrecacheNodes)
            {
                var texture = SDL_image.IMG_LoadTexture(Renderer, node.Path);
                if(texture == IntPtr.Zero)
                {
                    Log.Warn($"Could not precache texture at [{node.Path}]!");
                    continue;
                }
                Textures[node.Name] = texture;
            }
        }

        private struct TexturePrecacheNode
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }
    }
}
