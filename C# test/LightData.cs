using System.Numerics;

namespace MainEngine.Rendering
{
    public static class Lightdata
    {
        public static Vector3 LightDirection = new Vector3(0.3f, -0.5f, -0.3f);
        public static float lightintensity = 1.3f;
        public static Vector3 MainLightcolor = new Vector3(0.4f, 0.35f, 0.33f);

        public static float ambientLight = 0.4f;
        public static Vector3 ambientlightcolor = new Vector3(0.35f, 0.35f, 0.5f);
    }
}