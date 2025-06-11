using System.Numerics;

namespace MainEngine
{
    public class Mesh()
    {
        public Vector3[] Verts = new Vector3[0];

        public int[] indecies = new int[0];

        public float3[] TriColor = new float3[0];

        public Shader shader = new TextureShader();

        public Vector3[] normals = new Vector3[0];

        public Vector2[] UVS = new Vector2[0];
    }
}