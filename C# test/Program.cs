using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics;
using MainEngine.Rendering;
using Raylib_cs;
using Microsoft.AspNetCore.Components.RenderTree;

namespace MainEngine
{
    public class Camera
    {
        public Transform transform = new Transform();
        public float fov;
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

    public static class Program
    {
        public static Scene scene = new Scene();
        public static Camera camera = new Camera();

        public static void Main()
        {
            Rendertarget rendertarget = new Rendertarget(RenderSettings.Imagewidth, RenderSettings.Imageheight);

            Start();
            Run(scene, rendertarget);
        }

        public static void Start()
        {
            camera.fov = 70;
            scene.camera = camera;

            Gameobject ground = scene.AddObjectByMeshName("plane");
            ground.transform.postion.Z = 5;
            ground.transform.postion.Y = 1;
            ground.transform.pitch = 0;
            ground.transform.jaw = 0;

            TextureShader GroundShader = scene.ImportTextureToShader("Ground");
            ground.mesh.shader = GroundShader;


            Gameobject cube = scene.AddObjectByMeshName("cube");
            cube.transform.postion.Z = 3;

            TextureShader Wood = scene.ImportTextureToShader("Basic Old wood_bright_Albedo");
            Wood.smoothnormals = false;

            cube.mesh.shader = Wood;

        }


        public static void Update()
        {

            if (!RenderSettings.rendertofile)
            {

              //  #if false 
                float camspeed = 5f;

                float camerasens = 105f;

                Vector2 mousedelta = Raylib.GetMouseDelta() * camerasens / RenderSettings.Imagewidth;
                camera.transform.pitch += mousedelta.Y * Time.DeltaTime;
                camera.transform.jaw -= mousedelta.X * Time.DeltaTime;


                (Vector3 camright, Vector3 camup, Vector3 camforward) = camera.transform.GetBasisVector();


                Vector3 camdelta = Vector3.Zero;
                if (Raylib.IsKeyDown(KeyboardKey.W)) camdelta += camforward * camspeed * Time.DeltaTime;
                if (Raylib.IsKeyDown(KeyboardKey.A)) camdelta -= camright * camspeed * Time.DeltaTime;
                if (Raylib.IsKeyDown(KeyboardKey.S)) camdelta -= camforward * camspeed * Time.DeltaTime;
                if (Raylib.IsKeyDown(KeyboardKey.D)) camdelta += camright * camspeed * Time.DeltaTime;

                camera.transform.postion += camdelta;

                Raylib.SetMousePosition(RenderSettings.Imagewidth / 2, RenderSettings.Imageheight / 2);
                Raylib.HideCursor();

             //   #endif

            }
            else
            {
                camera.transform.pitch = 0;
                camera.transform.jaw = 0;

                camera.transform.postion = Vector3.Zero;
            }

        }

        public static byte[] BuildTexture(Rendertarget target,byte[] bytes)
        {

            float3[] colorbuffer = target.UpscaledBuffer();

            for (int i = 0; i < bytes.Length; i += 4)
            {
                bytes[i] = (byte)(colorbuffer[i / 4].r * 255);
                bytes[i + 1] = (byte)(colorbuffer[i / 4].g * 255);
                bytes[i + 2] = (byte)(colorbuffer[i / 4].b * 255);
                bytes[i + 3] = (byte)255;
            }
            return bytes;
        }

        public static void Run(Scene scene, Rendertarget target)
        {
            if (RenderSettings.rendertofile)
            {
                for (int i = 0; i < RenderSettings.frames; i++)
                {
                    Update();

                    ImageRenderer.Render(scene, target);
                }
            }
            else
            {
             //   #if false
                Raylib.InitWindow(RenderSettings.Imagewidth * RenderSettings.uppscale, RenderSettings.Imageheight * RenderSettings.uppscale, "Engine");
                Raylib.MaximizeWindow();

                Texture2D texture = Raylib.LoadTextureFromImage(Raylib.GenImageColor(target.Width * RenderSettings.uppscale, target.Height * RenderSettings.uppscale, Color.Red));
                byte[] texturebytes = new byte[RenderSettings.Imagewidth * RenderSettings.uppscale * RenderSettings.Imageheight  * RenderSettings.uppscale * 4];

                while (!Raylib.WindowShouldClose())
                {
                    Update();

                    //render
                    ImageRenderer.Render(scene, target);
                    texturebytes = BuildTexture(target, texturebytes);

                    //apply bytes to texture
                    Raylib.UpdateTexture(texture, texturebytes);

                    //draw to screeen
                    Raylib.BeginDrawing();

                    Raylib.DrawTexture(texture, 0, 0, Color.White);

                    RenderSettings.frame++;

                    Raylib.EndDrawing();
                }

                Raylib.CloseWindow();
           //     #endif

            }
          

        }


    }
}