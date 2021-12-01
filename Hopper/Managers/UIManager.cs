using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hopper.Game.Entities;
using Hopper.Geometry;
using Hopper.Graphics;
using SDL2;

using Scancodes = SDL2.SDL.SDL_Scancode;

namespace Hopper.Managers
{
    public static class UIManager
    {
        public static Queue<(string Message, Point Position)> NumberMessages { get; set; } = new();

        private static IntPtr Renderer => GraphicsManager.Renderer;

        private static IntPtr MainMenu { get; set; }
        private static IntPtr Numbers { get; set; }
        private static IntPtr Font { get; set; }
        private static IntPtr BubbleTea { get; set; }
        private static IntPtr EmptyBubbleTea { get; set; }
        private static IntPtr DeathScreen { get; set; }
        private static IntPtr Selector { get; set; }
        private static IntPtr Recap { get; set; }
        private static IntPtr Screen { get; set; }

        private static IntPtr RedKey { get; set; }
        private static IntPtr GreenKey { get; set; }
        private static IntPtr BlueKey { get; set; }
        private static IntPtr Crosshair { get; set; }
        private static IntPtr Shotgun { get; set; }

        //Main Menu
        private static List<SDL.SDL_Point> StarField { get; set; } = new();
        private static int MainMenuCursor { get; set; } = 0;

        //Death Screen
        private static bool DeathScreenShowing { get; set; } = false;
        private static int DeathScreenSelector { get; set; } = 0;

        //Recap
        private static float RecapProgress { get; set; } = 0.0f;
        private static bool RecapShowing { get; set; } = false;
        private static bool ShowStats { get; set; } = false;

        // Screen messages
        private static string Message { get; set; } = string.Empty;
        private static bool ShowMessage { get; set; } = false;
        private static Dictionary<string, IntPtr> FontColors { get; set; } = new();

        //Level name
        private static string LevelName { get; set; }
        private static int ShowLevelNameCounter { get; set; } = 400;

        private static SDL.SDL_Color[] Colors = new SDL.SDL_Color[]
        {
            new SDL.SDL_Color(){ r = 0,   g = 0,   b = 0,   a = 255 },  // Black
            new SDL.SDL_Color(){ r = 130, g = 0,   b = 0,   a = 255 },  // Dark Red
            new SDL.SDL_Color(){ r = 0,   g = 130, b = 0,   a = 255 },  // Dark Green
            new SDL.SDL_Color(){ r = 130, g = 130, b = 0,   a = 255 },  // Dark Yellow
            new SDL.SDL_Color(){ r = 0,   g = 0,   b = 130, a = 255 },  // Dark Blue
            new SDL.SDL_Color(){ r = 130, g = 0,   b = 130, a = 255 },  // Dark Magenta
            new SDL.SDL_Color(){ r = 0,   g = 130, b = 130, a = 255 },  // Dark Teal
            new SDL.SDL_Color(){ r = 195, g = 195, b = 195, a = 255 },  // Light Grey
            new SDL.SDL_Color(){ r = 130, g = 130, b = 130, a = 255 },  // Dark Grey
            new SDL.SDL_Color(){ r = 255, g = 0,   b = 0,   a = 255 },  // Light Red
            new SDL.SDL_Color(){ r = 0,   g = 255, b = 0,   a = 255 },  // Light Green
            new SDL.SDL_Color(){ r = 255, g = 255, b = 0,   a = 255 },  // Light Yellow
            new SDL.SDL_Color(){ r = 0,   g = 0,   b = 255, a = 255 },  // Light Blue
            new SDL.SDL_Color(){ r = 255, g = 0,   b = 255, a = 255 },  // Light Magenta
            new SDL.SDL_Color(){ r = 0,   g = 255, b = 255, a = 255 },  // Light Teal
            new SDL.SDL_Color(){ r = 255, g = 255, b = 255, a = 255 },  // White
        };

        public static SDL.SDL_Color BLACK   => Colors[0];
        public static SDL.SDL_Color DK_RED  => Colors[1];
        public static SDL.SDL_Color DK_GRN  => Colors[2];
        public static SDL.SDL_Color DK_YLW  => Colors[3];
        public static SDL.SDL_Color DK_BLU  => Colors[4];
        public static SDL.SDL_Color DK_MAG  => Colors[5];
        public static SDL.SDL_Color DK_TEAL => Colors[6];
        public static SDL.SDL_Color LT_GREY => Colors[7];
        public static SDL.SDL_Color DK_GREY => Colors[8];
        public static SDL.SDL_Color LT_RED  => Colors[9];
        public static SDL.SDL_Color LT_GRN  => Colors[10];
        public static SDL.SDL_Color LT_YLW  => Colors[11];
        public static SDL.SDL_Color LT_BLU  => Colors[12];
        public static SDL.SDL_Color LT_MAG  => Colors[13];
        public static SDL.SDL_Color LT_TEAL => Colors[14];
        public static SDL.SDL_Color WHITE   => Colors[15];

        public static void Init()
        {
            Numbers = GraphicsManager.GetTexture("Numbers");
            Font = GraphicsManager.GetTexture("Font");
            BubbleTea = GraphicsManager.GetTexture("BubbleTea");
            EmptyBubbleTea = GraphicsManager.GetTexture("BubbleTeaEmpty");
            DeathScreen = GraphicsManager.GetTexture("DeathScreen");
            Selector = GraphicsManager.GetTexture("Selector");
            MainMenu = GraphicsManager.GetTexture("MainMenu");
            Recap = GraphicsManager.GetTexture("Recap");
            Screen = GraphicsManager.GetTexture("MessageScreen");
            RedKey = GraphicsManager.GetTexture("RedKey");
            GreenKey = GraphicsManager.GetTexture("GreenKey");
            BlueKey = GraphicsManager.GetTexture("BlueKey");
            Crosshair = GraphicsManager.GetTexture("Crosshair");
            Shotgun = GraphicsManager.GetTexture("ShotGun");

            var r = new Random();
            for(int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    StarField.Add(new SDL.SDL_Point()
                    {
                        x = i * (SystemManager.Width / 10) + (SystemManager.Width / 40) + ((j % 2) * (SystemManager.Width / 40)) + (r.Next(SystemManager.Width/13)),
                        y = j * (SystemManager.Height / 10) + (SystemManager.Height / 40) + ((j % 2) * (SystemManager.Height / 40)) + (r.Next(SystemManager.Height / 13))
                    });
                }
            }
        }

        public static void Draw()
        {

            while(NumberMessages.Count > 0)
            {
                var message = NumberMessages.Dequeue();
                DrawNumberString(message.Message, message.Position);
            }
            DrawDeathScreen();
            DrawMainMenu();
            DrawRecap();
            DrawMesssage();
            DrawLevelName();
            DrawPlayerUI();
        }

        public static void Update()
        {
            UpdateMainMenu();
            UpdateRecap();
            HandleDeathScreenInput();
            UpdateMessage();
            UpdateLevelName();
        }

        private static void InitFontColors()
        {

        }

        public static void DrawNumbers(string message, Point position)
        {
            NumberMessages.Enqueue((message, position));
        }

        public static void DrawString(string message, Point position, float size = 1.0f)
        {
            var r = new SDL.SDL_FRect()
            {
                x = position.x,
                y = position.y,
                w = 12 * size,
                h = 12 * size
            };
            var originalX = position.x;
            for (int i = 0; i < message.Length; i++)
            {
                char c = message[i];
                if(c == '\n')
                {
                    r.x = originalX;
                    r.y += 10 * size;
                    continue;
                }
                r.x += 10 * size;
                if (c < '!' || c > '}')
                {
                    continue;
                }
                
                SDL.SDL_Rect src = new SDL.SDL_Rect()
                {
                    x = (c - '!') * 12,
                    y = 0,
                    w = 12,
                    h = 12
                };
                SDL.SDL_RenderCopyF(GraphicsManager.Renderer, Font, ref src, ref r);
                //Render.Box(r, src, Numbers, SDL.SDL_RendererFlip.SDL_FLIP_NONE);
            }
        }

        private static void DrawNumberString(string message, Point position, float size = 1.0f)
        {
            var r = new SDL.SDL_FRect()
            {
                x = position.x,
                y = position.y,
                w = 28 * size,
                h = 28 * size
            };
            for (int i = 0; i < message.Length; i++)
            {
                char c = message[i];
                if (c < '0' || c > '9')
                {
                    continue;
                }
                r.x = position.x + i * 20 * size;
                SDL.SDL_Rect src = new SDL.SDL_Rect()
                {
                    x = (c - '0') * 28,
                    y = 0,
                    w = 28,
                    h = 28
                };
                SDL.SDL_RenderCopyF(GraphicsManager.Renderer, Numbers, ref src, ref r);
                //Render.Box(r, src, Numbers, SDL.SDL_RendererFlip.SDL_FLIP_NONE);
            }
        }

        private static void DrawHealthBar()
        {
            if (GameManager.State != GAME_STATE.IN_GAME)
                return;
            var player = GameManager.MainPlayer;

            var src = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 12,
                h = 16
            };
            
            for(int i = 0; i < player.Health; i++)
            {
                var dst = new SDL.SDL_Rect()
                {
                    x = 10 + i * 36,
                    y = 10,
                    w = 36,
                    h = 48
                };

                SDL.SDL_RenderCopy(GraphicsManager.Renderer, BubbleTea, ref src, ref dst);
            }
            for (int i = player.Health; i < player.MaxHealth; i++)
            {
                var dst = new SDL.SDL_Rect()
                {
                    x = 10 + i * 36,
                    y = 10,
                    w = 36,
                    h = 48
                };

                SDL.SDL_RenderCopy(GraphicsManager.Renderer, EmptyBubbleTea, ref src, ref dst);
            }
        }

        private static void DrawDeathScreen()
        {
            if (!DeathScreenShowing)
                return;
            SDL.SDL_Rect src = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 768,
                h = 512
            };

            SDL.SDL_Rect dst = new SDL.SDL_Rect()
            {
                x = SystemManager.Width/2 - (768/2),
                y = SystemManager.Height/2 - (512/2),
                w = 768,
                h = 512
            };

            SDL.SDL_RenderCopy(GraphicsManager.Renderer, DeathScreen, ref src, ref dst);

            SDL.SDL_Rect selectorSrc = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 12,
                h = 16
            };

            SDL.SDL_Rect selectorDst = new SDL.SDL_Rect()
            {
                x = dst.x + 438,
                y = dst.y + 310 + (DeathScreenSelector * 82),
                w = selectorSrc.w * 3,
                h = selectorSrc.h * 3
            };

            SDL.SDL_RenderCopy(GraphicsManager.Renderer, BubbleTea, ref selectorSrc, ref selectorDst);
        }

        private static void HandleDeathScreenInput()
        {
            if (!DeathScreenShowing)
                return;
            if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_DOWN].Down && InputManager.Keys[(int)Scancodes.SDL_SCANCODE_DOWN].Edge)
            {
                DeathScreenSelector++;
                DeathScreenSelector %= 2;
            }
            else if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_UP].Down && InputManager.Keys[(int)Scancodes.SDL_SCANCODE_UP].Edge)
            {
                DeathScreenSelector--;
                if (DeathScreenSelector < 0)
                {
                    DeathScreenSelector = 1;
                }
            }
            else if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_RETURN].Down && InputManager.Keys[(int)Scancodes.SDL_SCANCODE_RETURN].Edge)
            {
                DeathScreenShowing = false;
                if (DeathScreenSelector == 0) // continue
                {
                    GameManager.RestartLevel();
                } else if (DeathScreenSelector == 1) // quit
                {
                    GameManager.CurrentLevel = null;
                    GameManager.State = GAME_STATE.MAIN_MENU;
                }

            }
        }

        public static void DrawStars()
        {
            foreach (var star in StarField)
            {
                var dot = new SDL.SDL_Rect()
                {
                    x = star.x,
                    y = star.y,
                    w = 3,
                    h = 3
                };
                SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, 0xff, 0xff, 0xff, 0xff);
                SDL.SDL_RenderFillRect(GraphicsManager.Renderer, ref dot);
            }
        }

        private static void DrawMainMenu()
        {
            if (GameManager.State != GAME_STATE.MAIN_MENU)
                return;

            DrawStars();

            var src = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 256,
                h = 256
            };
            var dst = new SDL.SDL_Rect()
            {
                x = SystemManager.Width / 2 - (src.w * 3)/2,
                y = SystemManager.Height / 2 - (src.h * 3)/2,
                w = src.w * 3,
                h = src.h * 3
            };
            SDL.SDL_RenderCopy(GraphicsManager.Renderer, MainMenu, ref src, ref dst);

            SDL.SDL_Rect selectorSrc = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 12,
                h = 16
            };

            SDL.SDL_Rect selectorDst = new SDL.SDL_Rect()
            {
                x = dst.x + 219,
                y = dst.y + 525 + (MainMenuCursor * 76),
                w = selectorSrc.w * 3,
                h = selectorSrc.h * 3
            };

            SDL.SDL_RenderCopy(GraphicsManager.Renderer, BubbleTea, ref selectorSrc, ref selectorDst);
        }

        public static void UpdateMainMenu()
        {
            if(GameManager.State != GAME_STATE.MAIN_MENU)
            {
                return;
            }

            if(InputManager.Keys[(int)Scancodes.SDL_SCANCODE_UP].Down && InputManager.Keys[(int)Scancodes.SDL_SCANCODE_UP].Edge)
            {
                MainMenuCursor--;
                if(MainMenuCursor < 0)
                {
                    MainMenuCursor = 2;
                }
            }
            else if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_DOWN].Down && InputManager.Keys[(int)Scancodes.SDL_SCANCODE_DOWN].Edge)
            {
                MainMenuCursor++;
                MainMenuCursor %= 3;
            }
            else if(InputManager.Keys[(int)Scancodes.SDL_SCANCODE_RETURN].Down && InputManager.Keys[(int)Scancodes.SDL_SCANCODE_RETURN].Edge)
            {
                switch (MainMenuCursor)
                {
                    case 0:
                        GameManager.NewGame();
                        break;
                    case 1:
                        break;
                    case 2:
                        GameManager.Quit = true;
                        break;
                }
            }

            UpdateStars();
        }
        public static void UpdateStars()
        {
            for (int i = 0; i < StarField.Count; i++)
            {
                var star = StarField[i];
                star.x -= 1;
                star.y += 1;
                if (star.x < -3)
                {
                    star.x = SystemManager.Width;
                }
                if (star.y > SystemManager.Height + 3)
                {
                    star.y = -3;
                }
                StarField[i] = star;
            }
        }

        public static void ShowDeathScreen()
        {
            DeathScreenShowing = true;
        }

        public static void ShowRecap(string nextLevel)
        {
            GameManager.NextLevelPath = nextLevel;
            RecapShowing = true;
        }

        private static void DrawRecap()
        {
            if (!RecapShowing)
                return;
            if(RecapProgress < 1.0f)
            {
                RecapProgress += 0.01f; // speed of increase
            } else if(RecapProgress > 1.0f)
            {
                RecapProgress = 1.0f;
                ShowStats = true;
            }

            var src = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 256,
                h = 94
            };

            var dest = SystemManager.Width / 2 - ((256 * 3)/2);
            var maxDiff = dest - (-256 * 3);

            var dst = new SDL.SDL_FRect()
            {
                x = (-256 * 3) + EaseIn(RecapProgress) * maxDiff,
                y = SystemManager.Height / 2 - (94 * 3) / 2,
                w = 256 * 3,
                h = 94 * 3
            };

            SDL.SDL_RenderCopyF(GraphicsManager.Renderer, Recap, ref src, ref dst);
            if (!ShowStats)
            {
                return;
            }

            string collected = $"{GameManager.TotalCollected}".PadRight(2, ' ');
            string collectible = $"{GameManager.TotalCollectibles}".PadLeft(2, ' ');
            DrawString($"{collected}/{collectible}", new Point(dst.x + 165 * 3, dst.y + 30), 5.0f);

            string killed = $"{GameManager.TotalKilled}".PadRight(2, ' ');
            string killable = $"{GameManager.TotalEnemies}".PadLeft(2, ' ');
            DrawString($"{killed}/{killable}", new Point(dst.x + 165 * 3, dst.y + 150), 5.0f);

            DrawString("Press E to Continue", new Point(dst.x + 118, dst.y + dst.h - 50), 3.0f);
        }

        public static void UpdateRecap()
        {
            if (!ShowStats)
                return;
            if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_E].Down)
            {
                ShowStats = false;
                RecapProgress = 0.0f;
                RecapShowing = false;

                GameManager.NewLevel(GameManager.NextLevelPath);
            }
        }

        private static void DrawMesssage()
        {
            if (!ShowMessage)
            {
                return;
            }

            SDL.SDL_Rect src = new()
            {
                x = 0,
                y = 0,
                w = 768,
                h = 384
            };

            SDL.SDL_Rect r = new()
            {
                x = SystemManager.Width / 2 - (768 / 2),
                y = SystemManager.Height / 2 - (384 / 2),
                w = 768,
                h = 384
            };

            SDL.SDL_RenderCopy(GraphicsManager.Renderer, Screen, ref src, ref r);

            DrawString(Message, new(r.x, r.y + 24), 4.0f);
        }

        private static void DrawPlayerUI()
        {
            if (GameManager.State != GAME_STATE.IN_GAME)
                return;

            DrawHealthBar();
            DrawAmmoUI();
            DrawScoreUI();
            DrawKeysUI();
            DrawPowerupUI();
        }

        private static void DrawAmmoUI()
        {
            var ammoBoxSrc = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 12,
                h = 12
            };

            var ammoBox = new SDL.SDL_Rect()
            {
                x = 11,
                y = SystemManager.Height - 56,
                w = 42,
                h = 42
            };

            SDL.SDL_RenderCopy(GraphicsManager.Renderer, GraphicsManager.GetTexture("Ammo"), ref ammoBoxSrc, ref ammoBox);

            var blackBox = new SDL.SDL_Rect()
            {
                x = 60,
                y = SystemManager.Height - 48,
                w = 64,
                h = 36
            };
            SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, 0, 0, 0, 0xff);
            SDL.SDL_RenderFillRect(GraphicsManager.Renderer, ref blackBox);

            DrawString($"{GameManager.MainPlayer.Ammo}", new Point(36, SystemManager.Height - 48), 3.0f);
        }

        private static void DrawScoreUI()
        {
            var snackBoxSrc = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 24,
                h = 24
            };

            var snackBoxDst = new SDL.SDL_Rect()
            {
                x = SystemManager.Width - 256,
                y = 5,
                w = 72,
                h = 72
            };
            SDL.SDL_RenderCopy(GraphicsManager.Renderer, GraphicsManager.GetTexture("Snacks"), ref snackBoxSrc, ref snackBoxDst);
            var blackBox = new SDL.SDL_Rect()
            {
                x = SystemManager.Width - 256 + 100,
                y = 24,
                w = 128,
                h = 36
            };
            SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, 0, 0, 0, 0xff);
            SDL.SDL_RenderFillRect(GraphicsManager.Renderer, ref blackBox);
            DrawString($"{(GameManager.MainPlayer.Score)}", new Point(SystemManager.Width - 256 + 84, 24), 3.0f);
        }

        private static void DrawKeysUI()
        {
            var player = GameManager.MainPlayer;
            var keys = player.Keys;

            var keySrc = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 8,
                h = 8
            };

            var keyDst = new SDL.SDL_FRect()
            {
                x = 16,
                y = 0,
                w = 24,
                h = 24
            };

            //Red key
            if((keys & 1) != 0)
            {
                keyDst.y = 96;
                SDL.SDL_RenderCopyF(GraphicsManager.Renderer, RedKey, ref keySrc, ref keyDst);
            }
            if((keys & 2) != 0)
            {
                keyDst.y = 128;
                SDL.SDL_RenderCopyF(GraphicsManager.Renderer, GreenKey, ref keySrc, ref keyDst);
            }
            if((keys & 4) != 0)
            {
                keyDst.y = 160;
                SDL.SDL_RenderCopyF(GraphicsManager.Renderer, BlueKey, ref keySrc, ref keyDst);
            }
        }

        private static void DrawPowerupUI()
        {
            var player = GameManager.MainPlayer;
            if (player.CrossHair)
            {
                var src = new SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = 32,
                    h = 32
                };
                var dst = new SDL.SDL_Rect()
                {
                    x = (SystemManager.Width/2) - 80,
                    y = SystemManager.Height - 80,
                    w = 64,
                    h = 64
                };
                SDL.SDL_RenderCopy(Renderer, Crosshair, ref src, ref dst);
            }

            if (player.Shotgun)
            {
                var src = new SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = 64,
                    h = 24
                };
                var dst = new SDL.SDL_Rect()
                {
                    x = (SystemManager.Width / 2) + 16,
                    y = SystemManager.Height - 64,
                    w = 128,
                    h = 48
                };
                SDL.SDL_RenderCopy(Renderer, Shotgun, ref src, ref dst);
            }
        }

        private static void UpdateMessage()
        {
            if (!ShowMessage)
            {
                return;
            }
            if (InputManager.Keys[(int)Scancodes.SDL_SCANCODE_RETURN].Down)
            {
                Message = "";
                ShowMessage = false;
                GameManager.Pause = false;
            }
        }

        public static void DisplayMessage(string message)
        {
            GameManager.Pause = true;
            ShowMessage = true;
            Message = message;
        }

        public static void ShowLevelName(string name)
        {
            LevelName = name;
            ShowLevelNameCounter = 400;
        }

        public static void DrawLevelName()
        {
            if(GameManager.State != GAME_STATE.IN_GAME)
            {
                return;
            }
            if (string.IsNullOrEmpty(LevelName) || ShowLevelNameCounter <= 0)
            {
                return;
            }
            float size = 5.0f;
            var width = LevelName.Length * 10.0f * size;

            float _alpha = Math.Min(1.0f, ((float)ShowLevelNameCounter) / 100.0f);
            byte alpha = (byte)(_alpha * 255.0f);

            SDL.SDL_SetTextureAlphaMod(Font, alpha);
            DrawString(
                LevelName,
                new Point(SystemManager.Width/2 - width / 2, SystemManager.Height / 4),
                size
            );
            SDL.SDL_SetTextureAlphaMod(Font, 255);
        }

        public static void UpdateLevelName()
        {
            if(GameManager.State != GAME_STATE.IN_GAME)
            {
                return;
            }
            if (ShowLevelNameCounter > 0)
            {
                ShowLevelNameCounter--;
            }
            else
            {
                LevelName = null;
            }
        }

        public static float EaseIn(float t)
        {
            return -((t - 1) * (t - 1)) + 1.0f;
        }
    }
}
