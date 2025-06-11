using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.Xml;
using System.IO.Compression;
using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Globalization;

namespace MainEngine
{


    public static class OBJLoader
    {
        public static IEnumerable<T> DuplicateItems<T>(this IEnumerable<T> items, int factor)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (factor <= 0) throw new ArgumentException("factor must be >= 1", "factor");
            if (factor == 1) return items;

            List<T> list = new List<T>();
            using (var enumerator = items.GetEnumerator())
            {
                for (int i = 0; i < factor; i++)
                {
                    while (enumerator.MoveNext())
                        list.Add(enumerator.Current);
                    enumerator.Reset();
                }
            }
            return list;
        }

        public static Mesh LoadObjFile(string file)
        {

            List<Vector3> Verts = new List<Vector3>();
            List<int> tries = new List<int>();
            List<Vector3> FaceNormals = new List<Vector3>();
            List<Vector2> FaceUvs = new List<Vector2>();

            int triline = 0;
            int offset = 0;
            foreach (string line in file.Split("\n"))
            {

                if (line.StartsWith("v "))
                {
                    string[] axesS = line[2..].Split(' ');
                    Vector3 axes = new Vector3(float.Parse(axesS[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse(axesS[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(axesS[2], CultureInfo.InvariantCulture.NumberFormat));
                    Verts.Add(axes);
                }
                else if (line.StartsWith("vn "))
                {
                    string[] axses = line[2..].Split(' ');
                    FaceNormals.Add(new Vector3(float.Parse(axses[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(axses[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse(axses[3], CultureInfo.InvariantCulture.NumberFormat)));
                }
                else if (line.StartsWith("vt "))
                {
                    string[] Axses = line[2..].Split(" ");
                    int UvOffset = 0;
                    if (Axses[0] == "") { UvOffset = 1; }

                    FaceUvs.Add(new Vector2(float.Parse(Axses[0 + UvOffset], CultureInfo.InvariantCulture.NumberFormat), float.Parse(Axses[1 + UvOffset], CultureInfo.InvariantCulture.NumberFormat)));
                }
                else if (line.StartsWith("f "))
                {
                    string[] faceindiesgroup = line[2..].Split(' ');
                    for (int i = 0; i < faceindiesgroup.Length; i++)
                    {

                        int[] groups = faceindiesgroup[i].Split("/").Select(int.Parse).ToArray();
                        int pointindex = groups[0] - 1;
                        if (i >= 3)
                        {
                            tries.Add(tries[^(3 * i - 6)]);
                            tries.Add(tries[^2]);

                            FaceNormals.Insert(Math.Clamp(triline + offset, 0, FaceNormals.Count), FaceNormals[Math.Clamp(triline + offset, 0, FaceNormals.Count - 1)]);
                            offset++;
                        }

                        tries.Add(pointindex);
                    }
                    triline++;
                }

            }

            Mesh mesh = new Mesh();
            mesh.Verts = Verts.ToArray();
            mesh.indecies = tries.ToArray();
            mesh.TriColor = new float3[mesh.Verts.Length / 3];
            mesh.normals = FaceNormals.ToArray();

            FaceUvs.DuplicateItems((int)MathF.Ceiling((Verts.Count / FaceUvs.Count) + 1));    
            mesh.UVS = FaceUvs.ToArray();

            Debug.Print(mesh.Verts.Length.ToString());

            for (int i = 0; i < FaceUvs.Count; i++)
            {
                Debug.Print(FaceUvs[i].ToString());
            }

            return mesh;
        }

        public static Mesh LoadMesh(string name)
        {
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), "models", name + ".obj");
            string OBJData = File.ReadAllText(filepath);
            Mesh mesh = LoadObjFile(OBJData);

            mesh.shader = new TextureShader();

            for (int i = 0; i < mesh.TriColor.Length; i++)
            {
                mesh.TriColor[i] = new float3(Helper.randomfloat(0, 255), Helper.randomfloat(0, 255), Helper.randomfloat(0, 255));
            }

            return mesh;
        }
    }
}