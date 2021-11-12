using Hopper.Managers;

using SDL2;

namespace Hopper
{
    public class Program
    {
        static uint tick = 0;
        public static void Main(string[] args)
        {
            tick = SDL.SDL_GetTicks();
            Init();
            while (!InputManager.Quit)
            {
                if(SDL.SDL_GetTicks() - tick < 16){
                    continue;
                }
                tick = SDL.SDL_GetTicks();
                Update();
                Draw();
            }

        }

        private static void Init()
        {
            SystemManager.Init();
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
        }
    }
}