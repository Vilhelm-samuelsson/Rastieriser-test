
namespace MainEngine
{
    public class Gameobject(string Oname = "New GameObject")
    {
        public string name = Oname;

        public Mesh mesh = new Mesh();

        public Transform transform = new Transform();
    }
}