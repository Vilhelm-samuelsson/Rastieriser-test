using System;
using System.Numerics;
using System.Runtime.CompilerServices;


namespace MainEngine
{
    public static class Helper
    {
        public static float min(float a, float b)
        {
            if (a > b) { return b; } else { return a; }
            ;
        }

        public static float max(float a, float b)
        {
            if (a < b) { return b; } else { return a; }
            ;
        }
        public static Random random = new Random();
        public static float randomfloat(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        public static Vector2 randomvec2(float min, float max)
        {
            return new Vector2(randomfloat(min, max), randomfloat(min, max));
        }

        public static Vector3 randomvec3(float min, float max)
        {
            return new Vector3(randomfloat(min, max), randomfloat(min, max), randomfloat(min, max));
        }

        public static string[] Splitbyline(string Text)
        {
            return Text.Split(Environment.NewLine);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float dot(Vector2 a, Vector2 b) => a.X * b.X + a.Y * b.Y;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
         public static float dot(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }
}