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
            return Gobject;
        }

    }

}