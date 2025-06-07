using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace MainEngine
{
    public abstract class Shader
    {
        public abstract float3 pixelcolor(float deapth, Vector3 normal, Vector2 texcord);
    }

    public class TextureShader : Shader
    {
        public static Vector3 LightDirection = new Vector3(-0.13f, 0.5f, 0.33f);
        public float lightintensity = 1.3f;
        public Vector3 MainLightcolor = new Vector3(0.4f, 0.35f, 0.33f);

        public float ambientLight = 0.4f;
        public Vector3 ambientlightcolor = new Vector3(0.35f,0.35f,0.5f);

        public float3 color;

        public override float3 pixelcolor(float deapth, Vector3 normal, Vector2 texcord)
        {
            normal = Vector3.Normalize(normal);
            LightDirection = Vector3.Normalize(LightDirection);

            float Light = Helper.max(0,Vector3.Dot(normal, LightDirection) * lightintensity);

            Vector3 LightColor = new Vector3
            (
                1 * Light * MainLightcolor.X + ambientLight * ambientlightcolor.X,
                1 * Light * MainLightcolor.Y + ambientLight * ambientlightcolor.Y,
                1 * Light * MainLightcolor.Z + ambientLight * ambientlightcolor.Z
            );

            float3 finalcolor = new float3(color.x * LightColor.X, color.y * LightColor.Y, color.z * LightColor.Z);
           
            return finalcolor;
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