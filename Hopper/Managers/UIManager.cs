﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hopper.Geometry;
using Hopper.Graphics;
using SDL2;

using Scancodes = SDL2.SDL.SDL_Scancode;

namespace Hopper.Managers
{
    public static class UIManager
    {
        public static Queue<(string Message, Point Position)> NumberMessages { get; set; } = new();

        private static IntPtr MainMenu { get; set; }
        private static IntPtr Numbers { get; set; }
        private static IntPtr Font { get; set; }
        private static IntPtr BubbleTea { get; set; }
        private static IntPtr EmptyBubbleTea { get; set; }
        private static IntPtr DeathScreen { get; set; }
        private static IntPtr Selector { get; set; }
        private static IntPtr Recap { get; set; }
        private static IntPtr Screen { get; set; }

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
            DrawHealthBar();
            DrawDeathScreen();
            DrawMainMenu();
            DrawRecap();
            DrawMesssage();
        }

        public static void Update()
        {
            UpdateMainMenu();
            UpdateRecap();
            HandleDeathScreenInput();
            UpdateMessage();
        }

        public static void DrawNumbers(string message, Point position)
        {
            NumberMessages.Enqueue((message, position));
        }

        private static void DrawString(string message, Point position, float size = 1.0f)
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

        private static void DrawMainMenu()
        {
            if (GameManager.State != GAME_STATE.MAIN_MENU)
                return;

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

            for (int i = 0; i < StarField.Count; i++)
            {
                var star = StarField[i];
                star.x -= 1;
                star.y += 1;
                if(star.x < -3)
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

        public static float EaseIn(float t)
        {
            return -((t - 1) * (t - 1)) + 1.0f;
        }
    }
}
