using System.Numerics;

namespace MainEngine.Rendering
{
    public class Rendertarget(int w, int h)
    {
        public readonly float3[] ColorBuffer = new float3[w * h];
        public readonly float[] DeapthBuffer = new float[w * h];
        public readonly object[] locks = new object[w * h];

        public readonly int Height = h;
        public readonly int Width = w;
        public readonly Vector2 size = new(w, h);

        public void clear()
        {
            for (int i = 0; i < DeapthBuffer.GetLength(0); i++)
            {
                DeapthBuffer[i] = float.PositiveInfinity;
                ColorBuffer[i] = new float3(0, 0, 0);
                locks[i] ??= new object();
            }
        }

        public float3[] UpscaledBuffer()
        {
            float3[] Uppsscaledbuffer = new float3[Width * RenderSettings.uppscale * Height * RenderSettings.uppscale];
            for (int i = 0; i < ColorBuffer.Length; i++)
            {
                Uppsscaledbuffer[i * (RenderSettings.uppscale * Width / Height) * (RenderSettings.uppscale * Height / Width)] = ColorBuffer[i];
            }

            return Uppsscaledbuffer;
        }
    }
}