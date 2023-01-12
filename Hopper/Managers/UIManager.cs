using System.Numerics;
using Hopper.Geometry;
using SDL2;

using static SDL2.SDL.SDL_Scancode;
using static Hopper.Managers.InputManager;

namespace Hopper.Managers
{
    public static class UIManager
    {
        public static Queue<(string Message, Point Position)> NumberMessages { get; set; } = new();

        private static IntPtr Renderer => GraphicsManager.Renderer;

        private static IntPtr MainMenu { get; set; }
        private static IntPtr HowTo { get; set; }
        private static IntPtr RightArrow { get; set; }
        private static IntPtr LeftArrow { get; set; }
        private static IntPtr Numbers { get; set; }
        private static IntPtr Font { get; set; }
        private static IntPtr Keyboard { get; set; }
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
        private static IntPtr DemoEnd { get; set; }

        //Main Menu
        private static List<SDL.SDL_Point> StarField { get; set; } = new();
        private static int MainMenuCursor { get; set; } = 0;

        // Options
        private static int OptionsCursor { get; set; } = 0;

        // Input
        private static int InputCursor { get; set; } = 0;
        private static bool KeyCapture { get; set; } = false;

        // Volume
        private static int VolumeCursor { get; set; } = 0;

        //Death Screen
        private static bool DeathScreenShowing { get; set; } = false;
        private static int DeathScreenSelector { get; set; } = 0;

        //Recap
        private static float RecapProgress { get; set; } = 0.0f;
        public static bool RecapShowing { get; set; } = false;
        private static bool ShowStats { get; set; } = false;

        // Screen messages
        private static string Message { get; set; } = string.Empty;
        private static bool ShowMessage { get; set; } = false;
        private static Dictionary<string, IntPtr> FontColors { get; set; } = new();

        private static List<(string Message, Point Pos, int Length)> TextBubbles = new();

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
            Keyboard = GraphicsManager.GetTexture("Scancodes");
            BubbleTea = GraphicsManager.GetTexture("BubbleTea");
            EmptyBubbleTea = GraphicsManager.GetTexture("BubbleTeaEmpty");
            DeathScreen = GraphicsManager.GetTexture("DeathScreen");
            Selector = GraphicsManager.GetTexture("Selector");
            MainMenu = GraphicsManager.GetTexture("MainMenu");
            HowTo = GraphicsManager.GetTexture("HowTo");
            RightArrow = GraphicsManager.GetTexture("RightArrow");
            LeftArrow = GraphicsManager.GetTexture("LeftArrow");
            Recap = GraphicsManager.GetTexture("Recap");
            Screen = GraphicsManager.GetTexture("MessageScreen");
            RedKey = GraphicsManager.GetTexture("RedKey");
            GreenKey = GraphicsManager.GetTexture("GreenKey");
            BlueKey = GraphicsManager.GetTexture("BlueKey");
            Crosshair = GraphicsManager.GetTexture("Crosshair");
            Shotgun = GraphicsManager.GetTexture("ShotGun");
            DemoEnd = GraphicsManager.GetTexture("DemoEnd");

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
            DrawHowTo();
            DrawOptions();
            DrawInputOptions();
            DrawVolume();
            DrawRecap();
            DrawMesssage();
            DrawLevelName();
            DrawTextBubbles();
            DrawPlayerUI();
            DrawDemoEnd();
        }

        public static void Update()
        {
            UpdateHowTo();
            UpdateOptions();
            UpdateInput();
            UpdateVolume();
            UpdateMainMenu();
            UpdateRecap();
            HandleDeathScreenInput();
            UpdateMessage();
            UpdateLevelName();
            UpdateDemoEnd();

            CheckTextBubbleDelete();
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
                    goto Next;
                }
                if (c < '!' || c > '}')
                {
                    goto Next;
                }
                
                SDL.SDL_Rect src = new SDL.SDL_Rect()
                {
                    x = (c - '!') * 12,
                    y = 0,
                    w = 12,
                    h = 12
                };
                SDL.SDL_RenderCopyF(GraphicsManager.Renderer, Font, ref src, ref r);

            Next:
                r.x += 10 * size;
                //Render.Box(r, src, Numbers, SDL.SDL_RendererFlip.SDL_FLIP_NONE);
            }
        }

        public static void DrawButton(string message, Point position, SDL.SDL_Color color, float size = 1.0f, bool outline = true)
        {
            var box = new SDL.SDL_Rect()
            {
                x = (int)(position.x) - (int)(2 * size),
                y = (int)position.y - (int)(2 * size),
                w = (12 * (message.Length - 1)) * (int)size + (int)(4 * size),
                h = (int)(12 * size) + (int)(4 * size),
            };
            SDL.SDL_SetRenderDrawColor(Renderer, color.r, color.g, color.b, color.a);
            if(outline)
            {
                SDL.SDL_RenderDrawRect(Renderer, ref box);
            } else
            {
                SDL.SDL_RenderFillRect(Renderer, ref box);
            }
            DrawString(message, position, size);
        }

        public static void DrawScancode(SDL.SDL_Scancode code, Point position, SDL.SDL_Color color, float size = 1.0f)
        {
            var dst = new SDL.SDL_FRect()
            {
                x = position.x - 2,
                y = position.y - 2,
                w = 12.0f * size + 4,
                h = 12.0f * size + 4
            };
            SDL.SDL_Rect src;

            var codeNum = (int)code;
            if(codeNum >= 4 && codeNum <= 82)
            {
                src = new SDL.SDL_Rect()
                {
                    x = 12 * codeNum,
                    y = 0,
                    w = 12,
                    h = 12
                };
            } else if(codeNum >= 224 && codeNum <= 231)
            {
                int offset = codeNum <= 226 ? 224 : 228;
                src = new SDL.SDL_Rect()
                {
                    x = 12 * ((codeNum - offset) + 83),
                    y = 0,
                    w = 12,
                    h = 12
                };
            }else
            {
                return;
            }


            //draw black bg

            SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, color.r, color.g, color.b, color.a);
            SDL.SDL_RenderFillRectF(GraphicsManager.Renderer, ref dst);

            dst.x += 2;
            dst.y += 2;
            dst.w -= 4;
            dst.h -= 4;

            SDL.SDL_RenderCopyF(GraphicsManager.Renderer, Keyboard, ref src, ref dst);

            dst.x -= 4;
            dst.y -= 4;
            dst.w += 8;
            dst.h += 8;
            SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, color.r, color.g, color.b, color.a);
            SDL.SDL_RenderDrawRectF(GraphicsManager.Renderer, ref dst);
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
            if (InputManager.Keys[(int)SDL_SCANCODE_DOWN].Down && InputManager.Keys[(int)SDL_SCANCODE_DOWN].Edge)
            {
                DeathScreenSelector++;
                DeathScreenSelector %= 2;
            }
            else if (InputManager.Keys[(int)SDL_SCANCODE_UP].Down && InputManager.Keys[(int)SDL_SCANCODE_UP].Edge)
            {
                DeathScreenSelector--;
                if (DeathScreenSelector < 0)
                {
                    DeathScreenSelector = 1;
                }
            }
            else if (InputManager.Keys[(int)SDL_SCANCODE_RETURN].Down && InputManager.Keys[(int)SDL_SCANCODE_RETURN].Edge)
            {
                DeathScreenShowing = false;
                if (DeathScreenSelector == 0) // continue
                {
                    GameManager.RestartLevel();
                } else if (DeathScreenSelector == 1) // quit
                {
                    GameManager.CurrentLevel = null;
                    GameManager.PlayMusic("Assets/Music/CreepyWhistle.ogg");
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
                y = dst.y + 520 + (MainMenuCursor * 44),
                w = selectorSrc.w * 3,
                h = selectorSrc.h * 3
            };

            SDL.SDL_RenderCopy(GraphicsManager.Renderer, BubbleTea, ref selectorSrc, ref selectorDst);
        }

        private static void DrawHowTo()
        {
            if (GameManager.State != GAME_STATE.HOW_TO)
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
                x = SystemManager.Width / 2 - (src.w * 3) / 2,
                y = SystemManager.Height / 2 - (src.h * 3) / 2,
                w = src.w * 3,
                h = src.h * 3
            };
            SDL.SDL_RenderCopy(GraphicsManager.Renderer, HowTo, ref src, ref dst);
        }

        public static void DrawOptions()
        {
            if(GameManager.State != GAME_STATE.OPTIONS)
            {
                return;
            }
            DrawStars();

            DrawString("Input", new(SystemManager.Width / 2 - 380, SystemManager.Height / 2 - 64), 4.0f);
            DrawString("Volume", new(SystemManager.Width / 2 - 380, SystemManager.Height / 2 + 48), 4.0f);

            SDL.SDL_Rect selectorSrc = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 12,
                h = 16
            };

            SDL.SDL_Rect selectorDst = new SDL.SDL_Rect()
            {
                x = SystemManager.Width / 2,
                y = SystemManager.Height / 2 - 64 + (OptionsCursor * 112),
                w = selectorSrc.w * 3,
                h = selectorSrc.h * 3
            };

            SDL.SDL_RenderCopy(GraphicsManager.Renderer, BubbleTea, ref selectorSrc, ref selectorDst);

        }

        public static void DrawInputOptions()
        {
            if (GameManager.State != GAME_STATE.INPUT_OPTIONS)
            {
                return;
            }
            DrawStars();

            if (KeyCapture)
            {
                DrawString("Press key...", new Point(SystemManager.Width / 2 - 400, SystemManager.Height / 2 - 300), 6.0f);
            }

            DrawButton(
                "Back",
                new Point(SystemManager.Width / 2 - 400, SystemManager.Height / 2 - 128),
                (InputCursor == 0 ? LT_GRN : WHITE),
                4.0f,
                false
            );

            (string action, SDL.SDL_Scancode key)[] inputs =
            {
                ("Left", Input.Left),
                ("Right", Input.Right),
                ("Jump", Input.Jump),
                ("Shoot", Input.Shoot),
                ("Use", Input.Use)
            };

            for (int i = 0; i < inputs.Length; i++)
            {
                int lr = (i / 3);
                int row = (i % 3);
                int x = SystemManager.Width / 2 - 380 + (400 * lr);
                int y = SystemManager.Height / 2 - 64 + (64 * row);

                var pos = new Point(x, y);
                DrawString(inputs[i].action, pos, 4.0f);
                pos.x += 300;
                DrawScancode(inputs[i].key, pos, (i + 1) == InputCursor ? LT_GRN : WHITE, 4.0f);
            }

            //DrawString("Classic")
            DrawButton(
                "Classic",
                new Point(SystemManager.Width / 2 - 400, SystemManager.Height / 2 + 256),
                (InputCursor == 6 ? LT_GRN : WHITE),
                4.0f,
                false
            );
            DrawButton(
                "Modern",
                new Point(SystemManager.Width / 2 , SystemManager.Height / 2 + 256),
                (InputCursor == 7 ? LT_GRN : WHITE),
                4.0f,
                false
            );
        }

        public static void DrawVolume()
        {
            if(GameManager.State != GAME_STATE.VOLUME)
            {
                return;
            }

            DrawStars();

            var src = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 32,
                h = 32
            };

            var dst = new SDL.SDL_Rect()
            {
                x = SystemManager.Width / 2 + 380,
                y = SystemManager.Height / 2 - 128,
                w = 32 * 3,
                h = 32 * 3
            };

            SDL.SDL_RenderCopy(GraphicsManager.Renderer, RightArrow, ref src, ref dst);

            dst.x = SystemManager.Width / 2 - 380 - (32 * 3);
            SDL.SDL_RenderCopy(GraphicsManager.Renderer, LeftArrow, ref src, ref dst);

            dst.x += 32 * 4;
            dst.w = 64;
            dst.h = 96;
            dst.y -= 4;

            DrawString("SFX Volume", new(dst.x, dst.y - 64), 4.0f);
            if (VolumeCursor == 0)
            {
                SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, 0x00, 0xff, 0x00, 0xff);
            }
            else
            {
                SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, 0xff, 0xff, 0xff, 0xff);
            }
            for (int i = 0; i < SystemManager.SfxVolume; i++)
            {
                SDL.SDL_RenderFillRect(GraphicsManager.Renderer, ref dst);
                dst.x += 70;
            }

            dst = new SDL.SDL_Rect()
            {
                x = SystemManager.Width / 2 + 380,
                y = SystemManager.Height / 2 + 128,
                w = 32 * 3,
                h = 32 * 3
            };

            SDL.SDL_RenderCopy(GraphicsManager.Renderer, RightArrow, ref src, ref dst);

            dst.x = SystemManager.Width / 2 - 380 - (32 * 3);
            SDL.SDL_RenderCopy(GraphicsManager.Renderer, LeftArrow, ref src, ref dst);

            dst.x += 32 * 4;
            dst.w = 64;
            dst.h = 96;
            dst.y -= 4;

            DrawString("Mus Volume", new(dst.x, dst.y - 64), 4.0f);
            if (VolumeCursor == 1)
            {
                SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, 0x00, 0xff, 0x00, 0xff);
            }
            else
            {
                SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, 0xff, 0xff, 0xff, 0xff);
            }
            for (int i = 0; i < SystemManager.MusVolume; i++)
            {
                SDL.SDL_RenderFillRect(GraphicsManager.Renderer, ref dst);
                dst.x += 70;
            }

        }

        public static void UpdateMainMenu()
        {
            if(GameManager.State != GAME_STATE.MAIN_MENU)
            {
                return;
            }

            if(InputManager.Keys[(int)SDL_SCANCODE_UP].Down && InputManager.Keys[(int)SDL_SCANCODE_UP].Edge)
            {
                MainMenuCursor--;
                if(MainMenuCursor < 0)
                {
                    MainMenuCursor = 2;
                }
            }
            else if (InputManager.Keys[(int)SDL_SCANCODE_DOWN].Down && InputManager.Keys[(int)SDL_SCANCODE_DOWN].Edge)
            {
                MainMenuCursor++;
                MainMenuCursor %= 4;
            }
            else if(InputManager.Keys[(int)SDL_SCANCODE_RETURN].Down && InputManager.Keys[(int)SDL_SCANCODE_RETURN].Edge)
            {
                switch (MainMenuCursor)
                {
                    case 0:
                        GameManager.NewGame();
                        break;
                    case 1:
                        GameManager.State = GAME_STATE.HOW_TO;
                        break;
                    case 2:
                        GameManager.State = GAME_STATE.OPTIONS;
                        break;
                    case 3:
                        GameManager.Quit = true;
                        break;
                }
            }

            UpdateStars();
        }

        private static int HandleCursorUpDown(int cursor, int max, int min)
        {
            if (InputManager.Keys[(int)SDL_SCANCODE_DOWN].Down && InputManager.Keys[(int)SDL_SCANCODE_DOWN].Edge)
            {
                cursor++;
            }
            else if (InputManager.Keys[(int)SDL_SCANCODE_UP].Down && InputManager.Keys[(int)SDL_SCANCODE_UP].Edge)
            {
                cursor--;
            }
            if(cursor >= max)
            {
                cursor = min;
            }
            else if(cursor < min)
            {
                cursor = max - 1;
            }
            return cursor;
        }

        public static void UpdateOptions()
        {
            if (GameManager.State != GAME_STATE.OPTIONS)
                return;
            UpdateStars();

            OptionsCursor = HandleCursorUpDown(OptionsCursor, 2, 0);

            if (InputManager.EdgeDown(SDL_SCANCODE_RETURN))
            {
                if (OptionsCursor == 0)
                {
                    GameManager.State = GAME_STATE.INPUT_OPTIONS;
                    ClearFrame();
                }
                else
                {
                    GameManager.State = GAME_STATE.VOLUME;
                    ClearFrame();
                }
            }
            else if (EdgeDown(SDL_SCANCODE_ESCAPE))
            {
                GameManager.State = GAME_STATE.MAIN_MENU;
            }
        }

        public static void UpdateInput()
        {
            if(GameManager.State != GAME_STATE.INPUT_OPTIONS)
            {
                return;
            }
            UpdateStars();

            static bool MapKey(ref SDL.SDL_Scancode input)
            {
                if (InputManager.LastKeyDown is (SDL_SCANCODE_ESCAPE or SDL_SCANCODE_RETURN))
                {
                    KeyCapture = false;
                    return true;
                } else if (InputManager.LastKeyDown != null) {
                    input = (SDL.SDL_Scancode)InputManager.LastKeyDown;
                    KeyCapture = false;
                    return true;
                }
                return false;
            }

            if(KeyCapture)
            {
                switch (InputCursor)
                {
                    case 1: MapKey(ref Input.Left); break;
                    case 2: MapKey(ref Input.Right); break;
                    case 3: MapKey(ref Input.Jump); break;
                    case 4: MapKey(ref Input.Shoot); break;
                    case 5: MapKey(ref Input.Use); break;
                }
                return;
            }

            InputCursor = HandleCursorUpDown(InputCursor, 8, 0);

            if(EdgeDown(SDL_SCANCODE_RETURN))
            {
                if(InputCursor == 0)
                {
                    GameManager.State = GAME_STATE.OPTIONS;
                    ClearFrame();
                } else if(InputCursor >= 1 && InputCursor <= 5) {
                    KeyCapture = true;
                } else if(InputCursor == 6) // classic
                {
                    Input = new();
                } else if(InputCursor == 7)
                {
                    Input = new()
                    {
                        Left = SDL_SCANCODE_A,
                        Right = SDL_SCANCODE_D,
                        Jump = SDL_SCANCODE_SPACE,
                        Shoot = SDL_SCANCODE_F,
                        Use = SDL_SCANCODE_E,
                    };
                }
            }
        }

        public static void UpdateVolume()
        {
            if (GameManager.State != GAME_STATE.VOLUME)
                return;
            UpdateStars();

            if (InputManager.Keys[(int)SDL_SCANCODE_DOWN].Down && InputManager.Keys[(int)SDL_SCANCODE_DOWN].Edge)
            {
                VolumeCursor++;
                VolumeCursor %= 2;
            } else if (InputManager.Keys[(int)SDL_SCANCODE_UP].Down && InputManager.Keys[(int)SDL_SCANCODE_UP].Edge)
            {
                VolumeCursor--;
                if(VolumeCursor < 0)
                {
                    VolumeCursor = 1;
                }
            }

            if (InputManager.Keys[(int)SDL_SCANCODE_LEFT].Down && InputManager.Keys[(int)SDL_SCANCODE_LEFT].Edge)
            {
                if(VolumeCursor == 0 && SystemManager.SfxVolume > 0)
                {
                    SystemManager.SfxVolume--;
                    GameManager.PlayRandomChunk("AlienHurt", 1, 3);
                }
                else if(VolumeCursor == 1 && SystemManager.MusVolume > 0)
                {
                    SystemManager.MusVolume--;
                }
            }
            else if (InputManager.Keys[(int)SDL_SCANCODE_RIGHT].Down && InputManager.Keys[(int)SDL_SCANCODE_RIGHT].Edge)
            {
                if (VolumeCursor == 0 && SystemManager.SfxVolume < 10)
                {
                    SystemManager.SfxVolume++;
                    GameManager.PlayRandomChunk("AlienHurt", 1, 3);
                }
                else if (VolumeCursor == 1 && SystemManager.MusVolume < 10)
                {
                    SystemManager.MusVolume++;
                }
            }

            if (InputManager.Keys[(int)SDL_SCANCODE_RETURN].Down && InputManager.Keys[(int)SDL_SCANCODE_RETURN].Edge)
            {
                GameManager.State = GAME_STATE.MAIN_MENU;
                InputManager.Keys[(int)SDL_SCANCODE_RETURN].Edge = false; //ugh
            }
        }

        public static void UpdateHowTo()
        {
            if(GameManager.State != GAME_STATE.HOW_TO)
            {
                return;
            }
            if (InputManager.Keys[(int)SDL_SCANCODE_RETURN].Down && InputManager.Keys[(int)SDL_SCANCODE_RETURN].Edge)
            {
                GameManager.State = GAME_STATE.MAIN_MENU;

                InputManager.Keys[(int)SDL_SCANCODE_RETURN].Edge = false; //ugh
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
            DrawString($"{collected}/{collectible}", new Point(dst.x + 155 * 3, dst.y + 30), 5.0f);

            string killed = $"{GameManager.TotalKilled}".PadRight(2, ' ');
            string killable = $"{GameManager.TotalEnemies}".PadLeft(2, ' ');
            DrawString($"{killed}/{killable}", new Point(dst.x + 155 * 3, dst.y + 150), 5.0f);

            DrawString("Press E to Continue", new Point(dst.x + 118, dst.y + dst.h - 50), 3.0f);
        }

        public static void UpdateRecap()
        {
            if (!ShowStats)
                return;
            if (InputManager.Keys[(int)SDL_SCANCODE_E].Down)
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

            DrawString($"{GameManager.Frame}", new Point(1024 - 128, 768 - 32), 2.0f);
        }

        private static void DrawDemoEnd()
        {
            if(GameManager.State != GAME_STATE.DEMO_END)
            {
                return;
            }

            var src = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = 768,
                h = 768
            };
            var dst = new SDL.SDL_Rect()
            {
                x = SystemManager.Width / 2 - 768 / 2,
                y = 0,
                w = 768,
                h = 768
            };

            SDL.SDL_RenderCopy(Renderer, DemoEnd, ref src, ref dst);
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
            if (InputManager.Keys[(int)SDL_SCANCODE_RETURN].Down)
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

        private static void UpdateDemoEnd()
        {
            if(GameManager.State != GAME_STATE.DEMO_END)
            {
                return;
            }

            if (InputManager.Keys[(int)SDL_SCANCODE_RETURN].Down)
            {
                GameManager.PlayMusic("Assets/Music/CreepyWhistle.ogg");

                GameManager.State = GAME_STATE.MAIN_MENU;
            }
        }

        public static float EaseIn(float t)
        {
            return -((t - 1) * (t - 1)) + 1.0f;
        }

        public static void TextBubble(string text, Point position, int length)
        {
            TextBubbles.Add((text, position, length));
        }

        private static void CheckTextBubbleDelete()
        {
            for(int i = 0; i < TextBubbles.Count; i++)
            {
                var bubble = TextBubbles[i];
                bubble.Length--;
                TextBubbles[i] = bubble;
            }
            TextBubbles.RemoveAll(bubble => bubble.Length <= 0);
        }

        private static void DrawTextBubbles()
        {
            foreach(var bubble in TextBubbles)
            {

                var width = bubble.Message.Length * (10.0f / GraphicsManager.MainCamera.Scale.x);

                var pos = new Vector2(bubble.Pos.x, bubble.Pos.y);
                pos.X -= width / 2;
                pos.X -= GraphicsManager.MainCamera.Position.x - (SystemManager.Width / (2 * GraphicsManager.MainCamera.Scale.x));
                pos.Y -= GraphicsManager.MainCamera.Position.y - (SystemManager.Height / (2 * GraphicsManager.MainCamera.Scale.y));

                pos *= new Vector2(
                    GraphicsManager.MainCamera.Scale.x,
                    GraphicsManager.MainCamera.Scale.y
                );

                var position = new Point(pos.X, pos.Y);

                DrawString(bubble.Message, position, 1.0f);
            }
        }
    }
}
