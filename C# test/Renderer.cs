using System.Numerics;
using System.Collections.Generic;
using System.Diagnostics;
//using Raylib_cs;
using Microsoft.AspNetCore.Connections;
using MainEngine.Rendering;

namespace MainEngine.Rendering
{
    public static class ImageRenderer
    {
        public static Vector2 Rotate90(Vector2 vec) => new Vector2(vec.Y, -vec.X);

        public static float SighnedTrianlgearea(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 ap = p - a;
            Vector2 abper = Rotate90(b - a);
            return Helper.dot(ap, abper) / 2;
        }

        public static bool pointintriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p, out Vector3 weights)
        {
            float AreaABp = SighnedTrianlgearea(a, b, p);
            float AreaBCp = SighnedTrianlgearea(b, c, p);
            float AreaCAp = SighnedTrianlgearea(c, a, p);
            bool intri = AreaABp >= 0 && AreaBCp >= 0 && AreaCAp >= 0;
            if (intri)
            {
                float invareasum = 1 / (AreaABp + AreaBCp + AreaCAp);
                float WeightA = AreaBCp * invareasum;
                float WeightB = AreaCAp * invareasum;
                float WeightC = AreaABp * invareasum;

                weights = new Vector3(WeightA, WeightB, WeightC);


                return intri;
            }
            else
            {
                weights = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
                return intri;
            }


        }
        public static Vector3 TransformToscreenSpace(Vector3 point, Vector2 size, Transform transform, Camera camera)
        {
            point = transform.ToWorldPoint(point);
            point = camera.transform.ToLocalPoint(point);

            float screenheight_world = MathF.Tan(camera.fov / 2) * 2;
            float pixelsperworldunit = size.Y / screenheight_world / point.Z;

            Vector2 pixeloffset = new Vector2(point.X, point.Y) * pixelsperworldunit;
            return new Vector3(size / 2 + pixeloffset, point.Z);
        }

        public static void Render(Scene scene, Rendertarget target)
        {
            RenderImage(target, scene);

            RenderSettings.frame++;
        }

        public static void RenderImage(Rendertarget rendertarget, Scene scene)
        {
            for (int i = 0; i < rendertarget.DeapthBuffer.GetLength(0); i++)
            {
                rendertarget.DeapthBuffer[i] = float.PositiveInfinity;
                rendertarget.ColorBuffer[i] = new float3(0, 0, 0);
            }

            for (int o = 0; o < scene.gameobjects.Count; o++)
            {
                Gameobject objecttorender = scene.gameobjects[o];
                Vector3[] Verts = objecttorender.mesh.Verts;
                int[] indecis = objecttorender.mesh.indecies;

                #region  oldloop
                /* for (int i = 0; i < indecis.Length; i += 3)
                 {
                     Vector3 a3 = TransformToscreenSpace(Verts[indecis[i]], rendertarget.size, objecttorender.transform, scene.camera);
                     Vector3 b3 = TransformToscreenSpace(Verts[indecis[i + 1]], rendertarget.size, objecttorender.transform, scene.camera);
                     Vector3 c3 = TransformToscreenSpace(Verts[indecis[i + 2]], rendertarget.size, objecttorender.transform, scene.camera);

                     if (a3.Z <= 0 || b3.Z <= 0 || c3.Z <= 0) continue;

                     Vector2 a = new Vector2(a3.X, a3.Y);
                     Vector2 b = new Vector2(b3.X, b3.Y);
                     Vector2 c = new Vector2(c3.X, c3.Y);


                     float MinX = Helper.min(Helper.min(a.X, b.X), c.X);
                     float MinY = Helper.min(Helper.min(a.Y, b.Y), c.Y);
                     float MaxX = Helper.max(Helper.max(a.X, b.X), c.X);
                     float MaxY = Helper.max(Helper.max(a.Y, b.Y), c.Y);

                     int blockstartx = Math.Clamp((int)MinX, 0, rendertarget.Width);
                     int blockstarty = Math.Clamp((int)MinY, 0, rendertarget.Height);

                     int blockendx = Math.Clamp((int)Math.Ceiling(MaxX), 0, rendertarget.Width - 1);
                     int blockendy = Math.Clamp((int)Math.Ceiling(MaxY), 0, rendertarget.Height - 1);

                     for (int y = blockstarty; y <= blockendy; y++)
                     {
                         for (int x = blockstartx; x <= blockendx; x++)
                         {
                             int pixelindex = (rendertarget.Width * y) + x;

                             Vector3 weights;
                             if (pointintriangle(a, b, c, new Vector2(x, y), out weights))
                             {
                                 Vector3 deapths = new Vector3(1 / a3.Z, 1 / b3.Z, 1 / c3.Z);
                                 float deapth = 1 / Helper.dot(deapths, weights);
                                 if (deapth >= rendertarget.DeapthBuffer[pixelindex]) continue;

                                 Vector3 Normal = Vector3.Zero;
                                 Normal = objecttorender.mesh.normals[Math.Clamp(i / 3, 0, objecttorender.mesh.normals.Length - 1)];
                                 Normal = objecttorender.transform.transformednormal(Normal);


                                 rendertarget.ColorBuffer[pixelindex] = objecttorender.mesh.shader.pixelcolor(deapth, Normal, Vector2.One);
                                 rendertarget.DeapthBuffer[pixelindex] = deapth;
                             }
                         }
                     }
                 }*/
                #endregion

                Parallel.For(0, indecis.Length, TriIndex =>
                { 
                     Vector3 a3 = TransformToscreenSpace(Verts[indecis[TriIndex / 3]], rendertarget.size, objecttorender.transform, scene.camera);
                     Vector3 b3 = TransformToscreenSpace(Verts[indecis[TriIndex / 3 + 1]], rendertarget.size, objecttorender.transform, scene.camera);
                     Vector3 c3 = TransformToscreenSpace(Verts[indecis[TriIndex / 3 + 2]], rendertarget.size, objecttorender.transform, scene.camera);

                     if (a3.Z <= 0 || b3.Z <= 0 || c3.Z <= 0) TriIndex++;

                     Vector2 a = new Vector2(a3.X, a3.Y);
                     Vector2 b = new Vector2(b3.X, b3.Y);
                     Vector2 c = new Vector2(c3.X, c3.Y);


                     float MinX = Helper.min(Helper.min(a.X, b.X), c.X);
                     float MinY = Helper.min(Helper.min(a.Y, b.Y), c.Y);
                     float MaxX = Helper.max(Helper.max(a.X, b.X), c.X);
                     float MaxY = Helper.max(Helper.max(a.Y, b.Y), c.Y);

                     int blockstartx = Math.Clamp((int)MinX, 0, rendertarget.Width);
                     int blockstarty = Math.Clamp((int)MinY, 0, rendertarget.Height);

                     int blockendx = Math.Clamp((int)Math.Ceiling(MaxX), 0, rendertarget.Width - 1);
                     int blockendy = Math.Clamp((int)Math.Ceiling(MaxY), 0, rendertarget.Height - 1);

                     for (int y = blockstarty; y <= blockendy; y++)
                     {
                         for (int x = blockstartx; x <= blockendx; x++)
                         {
                             int pixelindex = (rendertarget.Width * y) + x;

                             Vector3 weights;
                             if (pointintriangle(a, b, c, new Vector2(x, y), out weights))
                             {
                                 Vector3 deapths = new Vector3(1 / a3.Z, 1 / b3.Z, 1 / c3.Z);
                                 float deapth = 1 / Helper.dot(deapths, weights);
                                 if (deapth >= rendertarget.DeapthBuffer[pixelindex]) continue;

                                 Vector3 Normal = Vector3.Zero;
                                 Normal = objecttorender.mesh.normals[Math.Clamp(TriIndex, 0, objecttorender.mesh.normals.Length - 1)];
                                 Normal = objecttorender.transform.transformednormal(Normal);

                                lock (rendertarget.locks)
                                {
                                    rendertarget.ColorBuffer[pixelindex] = objecttorender.mesh.shader.pixelcolor(deapth, Normal, Vector2.One);
                                    rendertarget.DeapthBuffer[pixelindex] = deapth;
                                }
                             }
                         }
                     }
                }
                );
                 
                     
                 
            }

            if (RenderSettings.rendertofile)
            {
                writedatatofile(rendertarget, "RenderedImage");
            }
            
        }

        public static void writedatatofile(Rendertarget image, string name)
        {
            //setup file
            using BinaryWriter writer = new(File.Open((Directory.GetCurrentDirectory() + name + RenderSettings.frame.ToString() + ".bmp"), FileMode.Create));
            uint[] Bytecount = { 14, 40, (uint)image.ColorBuffer.Length * 4 }; // setup file size 14 is for the first header BM, and 40 is for the DIB Header

            //write bmp headers
            writer.Write("BM"u8.ToArray());
            writer.Write(Bytecount[0] + Bytecount[1] + Bytecount[2]); //addtotalfilesize
            writer.Write((uint)0);
            writer.Write(Bytecount[0] + Bytecount[1]); //dataoffset
            writer.Write(Bytecount[1]); // DIP header;
            writer.Write((uint)image.size.X); //addpixel amount
            writer.Write((uint)image.size.Y);
            writer.Write((ushort)1); //colorpallet
            writer.Write((ushort)(8 * 4));//bits per pixel
            writer.Write((uint)0);
            writer.Write(Bytecount[2]);
            writer.Write(new byte[16]); //print everything


            for (int x = 0; x < image.ColorBuffer.Length; x++)
            {
                float3 col = image.ColorBuffer[x];

                col.x *= 255;
                col.y *= 255;
                col.z *= 255;

                writer.Write((byte)col.z); // r
                writer.Write((byte)col.y); // g
                writer.Write((byte)col.x); // b
                writer.Write((byte)0); //a
            }

        }
    }
}