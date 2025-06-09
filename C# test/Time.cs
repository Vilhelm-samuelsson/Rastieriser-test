using Raylib_cs;

namespace MainEngine
{
    public static class Time
    {
        public static float DeltaTime { get => deltaTime(); }

        static float deltaTime()
        {
            return Raylib.GetFrameTime();
        }
    }
}