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
        public static Mesh LoadObjFile(string file)
        {
            List<Vector3> Verts = new List<Vector3>();
            List<int> tries = new List<int>();
            List<Vector3> FaceNormals = new List<Vector3>();

            int triline = 0;
            int offset = 0;
            foreach (string line in Helper.Splitbyline(file))
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
                            FaceNormals.Insert(triline + offset, FaceNormals[Math.Clamp(triline + offset,0, FaceNormals.Count)]);
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