﻿using System.Numerics;
using System.Runtime.InteropServices;
using CMDG.Worst3DEngine;

namespace CMDG;

// Requires this vehicles folder in bin. https://u.pcloud.link/publink/show?code=XZeoWL5Z0MaG3wEryCfl6Tn7DDsihfyC1x7X 


public class AssemblyWinter2025
{
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);

    private static Rasterer? m_Raster;
    private static Vec3 vc;

    private static string vehicleFolderPath;
    private static Random random = new();

    public static void Run()
    {
        float mainZ = 0;  // demo main Z-position. It's about the same as camera z-position, but has this helper variable because of frequent access.

        vehicleFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenes", "AssemblyWinter2025", "vehicles");
        Directory.SetCurrentDirectory(vehicleFolderPath);

        m_Raster = new Rasterer(Config.ScreenWidth, Config.ScreenHeight);

        var camera = Rasterer.GetCamera();

        Random random = new();

        m_Raster.UseLight(false);
        m_Raster.SetAmbientColor(new Vec3(0f, 0f, 0f));
        m_Raster.SetLightColor(new Vec3(1f, 1f, 1f));


        // Snow flakes (just regular objects instead of particles for now)
        List<GameObject> snowflakes = [];
        // Not many colors to choose from in ANSI. More probability for brightest white.
        Color32[] snowflakeColors = new Color32[]
        {
            new Color32(255, 255, 255),
            new Color32(255, 255, 255),
            new Color32(204, 204, 204),
            new Color32(249, 241, 165),
            new Color32(118, 118, 188),
        };
        for (int i = 0; i < 500; i++)
        {
            var pos = new Vec3(0, 0, 0);
            pos.X = (float)(random.NextDouble() * 20f);
            pos.Y = (float)(random.NextDouble() * 5f);
            pos.Z = (float)(random.NextDouble() * 20f);
            //int colorIndex = random.Next(snowflakeColors.Length);
            int colorIndex = 1;
            GameObject gob = GameObjects.Add(new GameObject());
            //float flakeSize = 0.05f + i / 5000f;      // could vary size like this but small + near and large + far easily appears off
            float flakeSize = 0.07f;
            gob.CreateCube(new Vec3(flakeSize, flakeSize, flakeSize), snowflakeColors[colorIndex]);
            snowflakes.Add(gob);
        }

        float laneWidth = 2.5f;
        // Road edges (long continuous lines)
        float medianWidth = 4f;
        float roadEdgeWidth = 0.3f;
        float roadEdgeLength = 100f;
        List<float> roadEdgeXCoords = new();
        roadEdgeXCoords.Add(0);
        roadEdgeXCoords.Add(laneWidth * 2);
        roadEdgeXCoords.Add(laneWidth * 2 + medianWidth);
        roadEdgeXCoords.Add(laneWidth * 4 + medianWidth);
        List<GameObject> roadEdges = new();
        for (int i = 0; i < 4; i++)
        {
            var roadEdge = GameObjects.Add(new GameObject());
            roadEdge.CreateCube(new Vec3(roadEdgeWidth, 0.2f, roadEdgeLength), new Color32(249, 241, 165));
            roadEdge.SetPosition(new Vec3(roadEdgeXCoords[i], 0f, roadEdgeLength/2f));
            roadEdges.Add(roadEdge);
            roadEdge.Update();
        }

        // Road banks
        List<GameObject> roadbanks = new List<GameObject>();
        GameObject median = GameObjects.Add(new GameObject());
        //median.CreateCube(new Vec3(medianWidth, 0.1f, roadEdgeLength), new Color32(118, 118, 118));
        //median.SetPosition(new Vec3(roadEdgeXCoords[1] + medianWidth/2, -0.1f, roadEdgeLength / 2f));
        //roadEdges.Add(median);
        //median.Update();

        median.CreateCube(new Vec3(medianWidth, 0.1f, roadEdgeLength), new Color32(204, 204, 204));
        median.SetPosition(new Vec3(roadEdgeXCoords[1] + medianWidth / 2, -0.1f, roadEdgeLength / 2f));
        roadEdges.Add(median);
        median.Update();

        //// Road sides disabled for performance reasons for now
        //GameObject rightBank = GameObjects.Add(new GameObject());
        //rightBank.CreateCube(new Vec3(10f, 0.1f, roadEdgeLength), new Color32(118, 118, 118));
        //rightBank.SetPosition(new Vec3(roadEdgeXCoords[0] - 5f, -0.1f, roadEdgeLength / 2f));
        //roadEdges.Add(rightBank);
        //rightBank.Update();

        //GameObject leftBank = GameObjects.Add(new GameObject());
        //leftBank.CreateCube(new Vec3(40f, 0.1f, roadEdgeLength), new Color32(118, 118, 118));
        //leftBank.SetPosition(new Vec3(roadEdgeXCoords[3] + 20f, -0.1f, roadEdgeLength / 2f));
        //roadEdges.Add(leftBank);
        //leftBank.Update();

        ////Road base color
        //GameObject roadfwd = GameObjects.Add(new GameObject());
        //roadfwd.CreateCube(new Vec3(laneWidth * 2, 0.1f, roadEdgeLength), new Color32(118, 118, 118));
        //roadfwd.SetPosition(new Vec3(roadEdgeXCoords[0] + laneWidth, -0.1f, roadEdgeLength / 2f));
        //roadEdges.Add(roadfwd);
        //roadfwd.Update();

        //GameObject roadrev = GameObjects.Add(new GameObject());
        //roadrev.CreateCube(new Vec3(laneWidth * 2, 0.1f, roadEdgeLength), new Color32(118, 118, 118));
        //roadrev.SetPosition(new Vec3(roadEdgeXCoords[2] + laneWidth, -0.1f, roadEdgeLength / 2f));
        //roadEdges.Add(roadrev);
        //roadrev.Update();




        // Dashed lines between lanes
        float dashWidth = 0.15f;
        float dashLength = 0.75f;
        float dashSpacing = 4.5f;
        List<float> dashXCoords = new();
        dashXCoords.Add(laneWidth);
        dashXCoords.Add(laneWidth * 3 + medianWidth);
        List<GameObject> dashes = new();
        List<GameObject> oppositeDashes = new();
        int numberOfDashes = 30;
        for (int i = 0; i < numberOfDashes; i++)
        {
            var dash = GameObjects.Add(new GameObject());
            dash.CreateCube(new Vec3(dashWidth, 0.1f, dashLength), new Color32(255, 255, 255));
            dash.SetPosition(new Vec3(dashXCoords[0], 0, dashSpacing * i));
            dashes.Add(dash);
            dash.Update();

            var oppositeDash = GameObjects.Add(new GameObject());
            oppositeDash.CreateCube(new Vec3(dashWidth, 0.1f, dashLength), new Color32(255, 255, 255));
            oppositeDash.SetPosition(new Vec3(dashXCoords[1], 0, dashSpacing * i));
            oppositeDashes.Add(oppositeDash);
            oppositeDash.Update();
        }

        // Main car
        var mainCar = GameObjects.Add(new GameObject());
        mainCar.LoadMesh("car-coupe-red.obj");
        mainCar.SetPosition(new Vec3(dashXCoords[0] + laneWidth / 2f, 0, 0));
        mainCar.Update();
        Vec3 mainCarVelocity = new Vec3(0, 0, 15f);
        Vec3 mainCarCameraOffset = new Vec3(-4, 4f, -2f);
        camera.SetPosition(mainCar.GetPosition() - mainCarCameraOffset);
        camera.SetRotation(new Vec3(0.6f, -0.6f, 0));


        // Forward-going cars
        int number_of_forward_cars = 7;
        List<ForwardCar> forwardCars = new();
        float carPosZ = 0f;
        for (int i = 0; i < number_of_forward_cars; i++)
        {
            carPosZ += 3 + (float)(random.NextDouble() * 10f);
            GameObject car = GameObjects.Add(new GameObject());
            car.LoadMesh(getRandomCarFileName());
            car.SetPosition(new Vec3(roadEdgeXCoords[0] + laneWidth / 2f, 0, carPosZ));
            car.Update();
            Vec3 velocity = new Vec3(0, 0, 5 + (float)(random.NextDouble() * 7f));
            float tailgateDistance = 5 + (float)(random.NextDouble() * 5f);
            forwardCars.Add(new ForwardCar(car, velocity, tailgateDistance));
        }

        // Opposite cars
        List<OppositeCar> oppositeCars = new();
        carPosZ = 0f;
        for (int i = 0; i < 5; i++)
        {
            carPosZ += 3 + (float)(random.NextDouble() * 5.0);
            float carPosX = roadEdgeXCoords[1] + medianWidth + laneWidth / 2f;
            if (random.Next(2) < 1)
            {
                carPosX += laneWidth;
            }
            GameObject car = GameObjects.Add(new GameObject());
            car.LoadMesh(getRandomCarFileName());
            car.SetPosition(new Vec3(carPosX, 0, carPosZ));
            car.SetRotation(new Vec3(0, 3.14f, 0));
            car.Update();
            Vec3 velocity = new Vec3(0, 0, - 5 - (float)(random.NextDouble() * 7f));
            oppositeCars.Add(new OppositeCar(car, velocity));
        }

        int slowUpdateInterval = 40;  // Update less frequent stuff every nth frame
        int slowUpdateFrame = 0;


        while (true)
        {
            SceneControl.StartFrame(); // Clears frame buffer and starts frame timer.
            float deltaTime = (float)SceneControl.DeltaTime;

            // Update main car position
            mainCar.SetPosition(mainCar.GetPosition() + mainCarVelocity * deltaTime);
            mainCar.Update();
            mainZ = mainCar.GetPosition().Z;

            // Update foward car positions. Reverse iteration to enable removing cars while iterating.
            for (int i = forwardCars.Count - 1; i >= 0; i--)
            {
                ForwardCar car = forwardCars[i];               
                car.gameObject.SetPosition(car.gameObject.GetPosition() + car.velocity * deltaTime);
                if (car.gameObject.GetPosition().Z < mainZ - 10)
                {
                    forwardCars.RemoveAt(i);
                    GameObjects.Remove(car.gameObject);
                    SpawnNewForwardCar();
                    continue;
                }
                car.gameObject.Update();
            }

            // Avoid collisions among forwardCars
            forwardCars = forwardCars.OrderBy(car => car.gameObject.GetPosition().Z).ToList();  // sort cars by z-coordinate so need to check only against the next car
            for (int i = 0; i < forwardCars.Count - 1; i++)
            {
                var carA = forwardCars[i];
                var carB = forwardCars[i + 1];

                float xA = carA.gameObject.GetPosition().Z;
                float xB = carB.gameObject.GetPosition().Z;

                if (xB - xA < carA.tailgateDistance) // Collision detected
                {
                    // Move carA back to be exactly 3 units behind carB
                    carA.gameObject.SetPosition(carB.gameObject.GetPosition() - new Vec3(0, 0, carA.tailgateDistance));
                }
            }


            // Update opposite car positions.
            for (int i = oppositeCars.Count - 1; i >= 0; i--)
            {
                OppositeCar car = oppositeCars[i];
                car.gameObject.SetPosition(car.gameObject.GetPosition() + car.velocity * deltaTime);
                if (car.gameObject.GetPosition().Z < mainZ - 10)
                {
                    oppositeCars.RemoveAt(i);
                    GameObjects.Remove(car.gameObject);
                    SpawnNewOppositeCar();
                    continue;
                }
                car.gameObject.Update();
            }

            // Update snowflakes
            for (int i = 0; i < snowflakes.Count; i++)
            {
                var gob = snowflakes[i];
                var v = new Vec3(0, -5, 0) * deltaTime;
                var pos = gob.GetPosition() + v;

                if ((pos.Y < 0) || (pos.Z < mainZ - 3))
                {
                    pos.X = (float)(random.NextDouble() * 40f - 20);
                    pos.Y = 5 + (float)(random.NextDouble() * 10f);
                    pos.Z = mainZ + 10 + (float)(random.NextDouble() * 40f);
                }
                gob.SetPosition(pos);
                gob.Update();
            }

            // Things to do less frequently
            slowUpdateFrame++;
            if (slowUpdateFrame > slowUpdateInterval)
            {
                slowUpdateFrame = 0;

                // Move road edges forward to main car position
                foreach (GameObject roadEdge in roadEdges)
                {
                    roadEdge.SetPosition(new Vec3(roadEdge.GetPosition().X, roadEdge.GetPosition().Y, mainZ + roadEdgeLength/2.5f));
                    roadEdge.Update();
                }
                // Snap dashed lines to their spacing so they doesn't visually jump around
                float dashSnap = (float)(Math.Floor(mainZ / dashSpacing) * dashSpacing);
                float dashOffset = 0f;
                foreach (GameObject dash in dashes)
                {
                    dash.SetPosition(new Vec3(dash.GetPosition().X, dash.GetPosition().Y, dashSnap + dashOffset));
                    dash.Update();
                    dashOffset += dashSpacing;
                }
                dashOffset = 0f;
                foreach (GameObject dash in oppositeDashes)
                {
                    dash.SetPosition(new Vec3(dash.GetPosition().X, dash.GetPosition().Y, dashSnap + dashOffset));
                    dash.Update();
                    dashOffset += dashSpacing;
                }
            }

            // Adjust camera 
            // Sin multiplier is random-ish just to avoid them being in sync
            double time = SceneControl.ElapsedTime;
            float x = 0.6f + 0.1f * (float)Math.Sin(time * 0.19f);       // pan up/down
            float y = -0.6f + 0.2f * (float)Math.Sin(time * 0.24f);     // pan left/righ
            float heightVariance = -1 + (float)Math.Sin(time * 0.13f);  // move up/down
            camera.SetPosition(mainCar.GetPosition() + mainCarCameraOffset + new Vec3(0, heightVariance, 0));
            camera.SetRotation(new Vec3(x, y, 0));
            camera.Update();

            m_Raster.Process3D();
            SceneControl.EndFrame();             // Calculates spent time, limits to max framerate, and allows quitting by pressing ESC.
        }

        void SpawnNewForwardCar()
        {
            carPosZ = mainZ + 50 + (float)(random.NextDouble()) * 20;
            GameObject car = GameObjects.Add(new GameObject());
            car.LoadMesh(getRandomCarFileName());
            car.SetPosition(new Vec3(roadEdgeXCoords[0] + laneWidth / 2f, 0, carPosZ));
            car.Update();
            Vec3 velocity = new Vec3(0, 0, 5 + (float)(random.NextDouble() * 7f));
            float tailgateDistance = 5 + (float)(random.NextDouble() * 5f);
            forwardCars.Add(new ForwardCar(car, velocity, tailgateDistance));
        }

        void SpawnNewOppositeCar()
        {
            carPosZ = mainZ + 50;
            float carPosX = roadEdgeXCoords[1] + medianWidth + laneWidth / 2f;
            if (random.Next(2) < 1)
            {
                carPosX += laneWidth;
            }
            GameObject car = GameObjects.Add(new GameObject());
            car.LoadMesh(getRandomCarFileName());
            car.SetPosition(new Vec3(carPosX, 0, carPosZ));
            car.SetRotation(new Vec3(0, 3.14f, 0));
            car.Update();
            Vec3 velocity = new Vec3(0, 0, -5 - (float)(random.NextDouble() * 7f));
            oppositeCars.Add(new OppositeCar(car, velocity));
        }

    }

    private static string getRandomCarFileName()
    {
        string randomCarFileName = "";
        if (Directory.Exists(vehicleFolderPath))
        {
            string[] objFiles = Directory.GetFiles(vehicleFolderPath, "*.obj");

            if (objFiles.Length > 0)
            {
                randomCarFileName = objFiles[random.Next(objFiles.Length)];
            }
            else
            {
                Console.WriteLine("No .obj files found in the 'vehicles' folder.");
            }
        }
        else
        {
            Console.WriteLine("The 'vehicles' folder does not exist.");
        }
        return randomCarFileName;
    }
}

public class ForwardCar
{
    public GameObject gameObject;
    public Vec3 velocity;
    public float tailgateDistance;
    public ForwardCar(GameObject gameObject, Vec3 velocity, float tailgateDistance)
    {
        this.gameObject = gameObject;
        this.velocity = velocity;
        this.tailgateDistance = tailgateDistance;
    }
}
public class OppositeCar
{
    public GameObject gameObject;
    public Vec3 velocity;
    public OppositeCar(GameObject gameObject, Vec3 velocity)
    {
        this.gameObject = gameObject;
        this.velocity = velocity;
    }
}