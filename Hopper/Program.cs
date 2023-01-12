using Hopper.Managers;

using SDL2;

namespace Hopper
{
    public class Program
    {
        static uint tick = 0;
        public static long TickTime = DateTime.Now.Ticks;

        public static long Acc = 0;

        public static long TickRate = 166666;
        public static void Main(string[] args)
        {
            //tick = SDL.SDL_GetTicks();
            Init(args);
            while (!InputManager.Quit)
            {
                long NewTime = DateTime.Now.Ticks;
                long FrameTime = NewTime - TickTime;
                if (FrameTime >= 2500000)
                    FrameTime = 2500000;
                TickTime = NewTime;
                Acc += FrameTime;
                while (Acc >= 161290)
                {
                    Update();
                    Acc -= TickRate;
                }
                Draw();
                if (GameManager.Quit)
                {
                    return;
                }
            }

        }

        private static void Init(string[] args)
        {
            SystemManager.Init(args);
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO);
            GraphicsManager.Init(SystemManager.Width, SystemManager.Height);
            InputManager.Init();
            GameManager.Init();
            UIManager.Init();
        }

        private static void Draw()
        {
            SDL.SDL_SetRenderDrawColor(GraphicsManager.Renderer, 0, 0, 0, 0);
            SDL.SDL_RenderClear(GraphicsManager.Renderer);

            GameManager.Draw();
            GraphicsManager.Draw();
            UIManager.Draw();

            SDL.SDL_RenderPresent(GraphicsManager.Renderer);
        }

        private static void Update()
        {
            InputManager.Update();
            GameManager.Update();
            GraphicsManager.Update();
            UIManager.Update();
        }
    }
}