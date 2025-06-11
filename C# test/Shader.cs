using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using MainEngine.Rendering;
using BigGustave;

namespace MainEngine
{
    public abstract class Shader
    {
        public bool smoothnormals = false;
        public bool dubblesided = false;
        public abstract float3 pixelcolor(float deapth, Vector3 normal, Vector2 texcord);
    }

    public class TextureShader : Shader
    {
        public Png MainTexture;
        public float3 color = new float3(1,1,1);

        public override float3 pixelcolor(float deapth, Vector3 normal, Vector2 texcord)
        {
            float3 TextureColor = new float3(255, 255, 255);

            if (MainTexture != null)
              TextureColor = SampleTextureAtUv(texcord);

            normal = Vector3.Normalize(normal);
            Lightdata.LightDirection = Vector3.Normalize(Lightdata.LightDirection);

            float Light = Helper.max(0, Vector3.Dot(normal, Lightdata.LightDirection) * Lightdata.lightintensity);

            Vector3 LightColor = new Vector3
            (
                1 * Light * Lightdata.MainLightcolor.X + Lightdata.ambientLight * Lightdata.ambientlightcolor.X,
                1 * Light * Lightdata.MainLightcolor.Y + Lightdata.ambientLight * Lightdata.ambientlightcolor.Y,
                1 * Light * Lightdata.MainLightcolor.Z + Lightdata.ambientLight * Lightdata.ambientlightcolor.Z
            );

            float3 finalcolor = new float3(TextureColor.x / 255 * color.x * LightColor.X, TextureColor.y / 255 * color.y * LightColor.Y, TextureColor.z / 255 * color.z * LightColor.Z);

            return finalcolor;
        }

        public float3 SampleTextureAtUv(Vector2 uv)
        {
            Pixel pixel = MainTexture.GetPixel(Math.Clamp((int)Math.Ceiling(uv.X * MainTexture.Width),0 ,MainTexture.Width - 1), Math.Clamp((int)Math.Ceiling(uv.Y * MainTexture.Height),0,MainTexture.Height - 1));
            return new float3(pixel.R, pixel.G, pixel.B);
        }
    }

    public class NormalShader : Shader
    {
        public override float3 pixelcolor(float deapth, Vector3 normal, Vector2 texcord)
        {
            normal = Vector3.Abs(normal);

            return new float3(normal.X * 255, normal.Y * 255, normal.Z * 255);
        }
    }
}