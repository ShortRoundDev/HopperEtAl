using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SDL2;

namespace Hopper.Managers
{
    public static class InputManager
    {
        public static KeyState[] Keys { get; set; }= new KeyState[0xff];
        public static bool Quit { get; set; } = false;
        public static void Init()
        {
            for (int i = 0; i < Keys.Length; i++)
            {
                Keys[i] = new KeyState()
                {
                    Down = false,
                    TimeDown = 0,
                    Edge = false
                };
            }
        }

        public static void Update()
        {
            for (int i = 0; i < Keys.Length; i++)
            {
                Keys[i].Edge = false;
            }

            SDL.SDL_Event e;
            while(SDL.SDL_PollEvent(out e) != 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        Quit = true;
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        KeyDown(e.key.keysym.scancode);
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                        KeyUp(e.key.keysym.scancode);
                        break;
                }
            }
            
        }

        private static void KeyDown(SDL.SDL_Scancode scanCode)
        {
            if((int)scanCode > Keys.Count())
            {
                return;
            }
            var state = Keys[(int)scanCode];
            if(state.Down)
            {
                return;
            }
            state.Down = true;
            state.Edge = true;
            state.TimeDown = SDL.SDL_GetTicks();
            Keys[(int)scanCode] = state;
        }

        private static void KeyUp(SDL.SDL_Scancode scanCode)
        {
            if ((int)scanCode > Keys.Count())
            {
                return;
            }

            var state = Keys[(int)scanCode];
            if (!state.Down)
            {
                return;
            }
            state.Down = false;
            state.Edge = true;
            Keys[(int)scanCode] = state;
        }
    }

    public struct KeyState
    {
        public bool Down { get; set; }
        public UInt64 TimeDown { get; set; }
        public bool Edge { get; set; }
    }
}
