using Raylib_cs;
using BigGustave;
using System.Diagnostics;

namespace MainEngine
{
    public class Scene
    {
        public List<Gameobject> gameobjects = new List<Gameobject>();
        public Camera camera = new Camera();

        public Gameobject AddObjectByMeshName(string FileName)
        {
            Mesh mesh = OBJLoader.LoadMesh(FileName);
            Gameobject Gobject = new Gameobject(FileName);
            Gobject.mesh = mesh;

            Gobject.transform.pitch = 0;
            Gobject.transform.jaw = 0;

            gameobjects.Add(Gobject);

            Debug.Print("Added New object to scene:" + FileName);

            return Gobject;
        }

        public TextureShader ImportTextureToShader(string Texture)
        {
            TextureShader shader = new TextureShader();
            shader.color = new float3(1, 1, 1);

            var stream = File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "Tex", Texture + ".png"));
            Png tex = Png.Open(stream);

            shader.MainTexture = tex;

            return shader;
        }

    }

}