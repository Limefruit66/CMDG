﻿using System.Runtime.InteropServices;
using CMDG.Worst3DEngine;

namespace CMDG;

public class Scene6
{
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);

    private static Rasterer? m_Raster;

    //Placeholder
    private struct Input
    {
        public bool Forward;
        public bool Backward;
        public bool Up;
        public bool Down;
        public bool Left;
        public bool Right;
        public bool Left2;
        public bool Right2;
        public bool Up2;
        public bool Down2;
    };

    private static Input m_Input;

    public static void Run()
    {
        DebugConsole.SetMessageLimit(10);
        m_Input = new Input();
        m_Raster = new Rasterer(Config.ScreenWidth, Config.ScreenHeight);

        var camera = Rasterer.GetCamera();
        camera!.SetPosition(new Vec3(0, 1, -3));

        Random random = new();

        m_Raster.UseLight(true);
        m_Raster.SetAmbientColor(new Vec3(0.0f, 0.0f, 0.2f));
        m_Raster.SetLightColor(new Vec3(1.0f, 1.0f, 1.0f));

        for (int i = 0; i < 1000; i++)
        {
            var pos = new Vec3(0, 0, 0);
            pos.X = (float)(random.NextDouble() * 2.0f - 1) * 10;
            pos.Y = (float)(random.NextDouble() * 2.0f - 1) * 10;
            pos.Z = (float)(random.NextDouble() * 2.0f - 1) * 10;

            //init snow particles
            var particle = GameObjects.Add(new GameObject(pos, new Color32(255, 255, 255), ObjectType.Particle));
            particle.SetMaxRenderingDistance(10);
        }
        
        var cube = GameObjects.Add(new GameObject());
        cube.CreateCube(new Vec3(2, 2, 2), new Color32(255, 0, 0));
        

        while (true)
        {
            SceneControl.StartFrame(); // Clears frame buffer and starts frame timer.
            float deltaTime = (float)(SceneControl.DeltaTime);

            GetInputs();
            HandleCamera(camera, deltaTime);


            for (int i = 0; i < GameObjects.GameObjectsList.Count; i++)
            {
                var gob = GameObjects.GameObjectsList[i];
                //fancy stuff with gameobjects here
                gob.Update();
            }

            m_Raster.Process3D();

            // Calculates spent time, limits to max framerate,
            // and allows quitting by pressing ESC.
            SceneControl.EndFrame();
        }
    }

    private static void HandleCamera(Camera camera, float deltaTime)
    {
        //case 2: Move based on camera direction (more natural for 3d movement)
        var forward = camera.GetForward();
        var right = camera.GetRight();
        var up = camera.GetUp();


        float cameraMovementSpeed = 1.0f * deltaTime;
        var vc = camera.GetPosition();
        if (m_Input.Forward) vc = Vec3.Add(vc, Vec3.Mul(forward, cameraMovementSpeed));
        if (m_Input.Backward) vc = Vec3.Sub(vc, Vec3.Mul(forward, cameraMovementSpeed));
        if (m_Input.Left) vc = Vec3.Add(vc, Vec3.Mul(right, cameraMovementSpeed));
        if (m_Input.Right) vc = Vec3.Sub(vc, Vec3.Mul(right, cameraMovementSpeed));
        if (m_Input.Up) vc = Vec3.Add(vc, Vec3.Mul(up, cameraMovementSpeed));
        if (m_Input.Down) vc = Vec3.Sub(vc, Vec3.Mul(up, cameraMovementSpeed));
        //--------------------------------------------
        camera.SetPosition(vc);
        // get the current rotation values of the camera.
        float cameraRotY = camera.GetRotation().Y;
        float cameraRotX = camera.GetRotation().X;

        //rotate the camera based in input
        if (m_Input.Left2) cameraRotY -= 1.0f * deltaTime;
        if (m_Input.Right2) cameraRotY += 1.0f * deltaTime;
        if (m_Input.Up2) cameraRotX -= 1.0f * deltaTime;
        if (m_Input.Down2) cameraRotX += 1.0f * deltaTime;

        camera.SetRotation(new Vec3(cameraRotX, cameraRotY, 0));
    }


    //Just placeholder to get inputs
    private static void GetInputs()
    {
        m_Input.Forward = (GetAsyncKeyState((int)ConsoleKey.W) & 0x8000) != 0;
        m_Input.Backward = (GetAsyncKeyState((int)ConsoleKey.S) & 0x8000) != 0;
        m_Input.Up = (GetAsyncKeyState((int)ConsoleKey.R) & 0x8000) != 0;
        m_Input.Down = (GetAsyncKeyState((int)ConsoleKey.F) & 0x8000) != 0;
        m_Input.Left = (GetAsyncKeyState((int)ConsoleKey.A) & 0x8000) != 0;
        m_Input.Right = (GetAsyncKeyState((int)ConsoleKey.D) & 0x8000) != 0;
        m_Input.Left2 = (GetAsyncKeyState((int)ConsoleKey.LeftArrow) & 0x8000) != 0;
        m_Input.Right2 = (GetAsyncKeyState((int)ConsoleKey.RightArrow) & 0x8000) != 0;
        m_Input.Up2 = (GetAsyncKeyState((int)ConsoleKey.UpArrow) & 0x8000) != 0;
        m_Input.Down2 = (GetAsyncKeyState((int)ConsoleKey.DownArrow) & 0x8000) != 0;
    }
}