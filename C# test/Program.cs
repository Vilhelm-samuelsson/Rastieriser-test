using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using MainEngine.Rendering;

namespace MainEngine
{
    public class Camera
    {
        public Transform transform = new Transform();
        public float fov;
    }

    public class Rendertarget(int w, int h)
    {
        public readonly float3[] ColorBuffer = new float3[w * h];
        public readonly float[] DeapthBuffer = new float[w * h];

        public readonly int Height = h;
        public readonly int Width = w;
        public readonly Vector2 size = new(w, h);
    }
    public struct float3(float X, float Y, float Z)
    {
        public float x = X;
        public float y = Y;
        public float z = Z;

        public float r { get => x; set => x = value; }
        public float g { get => y; set => y = value; }
        public float b { get => z; set => z = value; }
    }

    public struct float2(float X, float Y)
    {
        public float x = X;
        public float y = Y;
    }

    public static class Engine
    {
        public static Scene scene = new Scene();

        public static void Start()
        {
            Camera camera = new Camera();
            camera.fov = 70;

            scene.camera = camera;
            Gameobject a = scene.AddObjectByMeshName("m");
            a.transform.postion.Z = 5;
            a.transform.rotation.Y = 0.25f;

            TextureShader ashader = new TextureShader();
            ashader.color = new float3(1, 1, 1);

            a.mesh.shader = ashader;
        }

        static void Main()
        {
            Start();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            float lasttime = stopwatch.ElapsedMilliseconds;
            for (int i = 0; i < RenderSettings.frames; i++)
            {
                ImageRenderer.Render(scene);
                Debug.Print((stopwatch.ElapsedMilliseconds - lasttime).ToString());
                lasttime = stopwatch.ElapsedMilliseconds;
                RenderSettings.frame++;
            }
        }


    }
}