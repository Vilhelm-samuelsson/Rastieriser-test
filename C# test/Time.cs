using Raylib_cs;

namespace MainEngine
{
    public static class Time
    {
        public static float DeltaTime { get => deltaTime(); }

        static float deltaTime()
        {
            if (RenderSettings.rendertofile) 
            {
                return 1 / RenderSettings.fps;
            }
            else
            {
                return Raylib.GetFrameTime();
            }
        }
    }
}