﻿using System.Globalization;

namespace CMDG.Worst3DEngine
{
    public struct Material
    {
        public string Name;
        public Vec3 Color; //just diffuse
    }

    public class Mesh
    {
        public Triangle[] Triangles;
        public string MeshFileName;
        private int triangleIndex;

        private List<Material> m_Materials;

        private readonly List<Triangle> TempTriangles;

        public Mesh()
        {
            MeshFileName = "";
            Triangles = [];
            m_Materials = [];
            TempTriangles = [];
            triangleIndex = 0;
        }

        public void AddTriangle(float ax, float ay, float az, float bx, float by, float bz, float cx, float cy,
            float cz)
        {
            Triangles[triangleIndex++] = new Triangle
            {
                P1 = new Vec3(ax, ay, az),
                P2 = new Vec3(bx, by, bz),
                P3 = new Vec3(cx, cy, cz),
            };
        }

        private void AddTriangle(Vec3 a, Vec3 b, Vec3 c)
        {
            Triangles[triangleIndex++] = new Triangle
            {
                P1 = a,
                P2 = b,
                P3 = c,
            };
        }


        private void AddTempTriangle(Vec3 a, Vec3 b, Vec3 c, Color32 color)
        {
            TempTriangles.Add(new Triangle
            {
                P1 = a,
                P2 = b,
                P3 = c,
                Color = color,
            });
        }

        public void CreateCube(Vec3 size, bool flipFace, Color32 color)
        {
            size.X = MathF.Abs(size.X);
            size.Y = MathF.Abs(size.Y);
            size.Z = MathF.Abs(size.Z);

            if (flipFace)
            {
                size.X = -size.X;
                size.Y = -size.Y;
                size.Z = -size.Z;
            }

            Triangles = new Triangle[8];
            triangleIndex = 0;

            var p0 = new Vec3(-size.X / 2, -size.Y / 2, -size.Z / 2);
            var p1 = new Vec3(size.X / 2, -size.Y / 2, -size.Z / 2);
            var p2 = new Vec3(size.X / 2, size.Y / 2, -size.Z / 2);
            var p3 = new Vec3(-size.X / 2, size.Y / 2, -size.Z / 2);

            var p4 = new Vec3(-size.X / 2, -size.Y / 2, size.Z / 2);
            var p5 = new Vec3(size.X / 2, -size.Y / 2, size.Z / 2);
            var p6 = new Vec3(size.X / 2, size.Y / 2, size.Z / 2);
            var p7 = new Vec3(-size.X / 2, size.Y / 2, size.Z / 2);

            // Front
            AddTempTriangle(p0, p2, p1, color);
            AddTempTriangle(p0, p3, p2, color);

            // Back
            AddTempTriangle(p4, p5, p6, color);
            AddTempTriangle(p4, p6, p7, color);

            // Left
            AddTempTriangle(p0, p7, p3, color);
            AddTempTriangle(p0, p4, p7, color);

            // Right
            AddTempTriangle(p1, p2, p6, color);
            AddTempTriangle(p1, p6, p5, color);

            // Top
            AddTempTriangle(p2, p3, p7, color);
            AddTempTriangle(p2, p7, p6, color);

            // Bottom
            AddTempTriangle(p0, p1, p5, color);
            AddTempTriangle(p0, p5, p4, color);
            CreateMesh();
        }


        private void LoadMaterials(string filename)
        {
            m_Materials.Clear();

            var material = new Material();

            foreach (var line in File.ReadAllLines(filename.Trim()))
            {
                if (line.StartsWith("newmtl"))
                {
                    material = new Material();
                    material.Name = line.Substring(6).Trim();
                }
                else if (line.StartsWith("Kd")) //diffuse
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var r = float.Parse(parts[1], CultureInfo.InvariantCulture);
                    var g = float.Parse(parts[2], CultureInfo.InvariantCulture);
                    var b = float.Parse(parts[3], CultureInfo.InvariantCulture);
                    material.Color = new Vec3(r, g, b);
                }
                else if (line.StartsWith("illum"))
                {
                    m_Materials.Add(material);
                }
            }
        }

        public void LoadMesh(string filename)
        {
            MeshFileName = filename;
            TempTriangles.Clear();

            var vertices = new List<Vec3>();

            var CurrentColor = new Vec3();

            foreach (var line in File.ReadAllLines(filename))
            {
                if (line.StartsWith("v "))
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    switch (parts.Length)
                    {
                        case 4:
                        {
                            var x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                            var y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                            var z = float.Parse(parts[3], CultureInfo.InvariantCulture);

                            vertices.Add(new Vec3(x, y, z));
                            break;
                        }
                        case 7:
                        {
                            var x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                            var y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                            var z = float.Parse(parts[3], CultureInfo.InvariantCulture);

                            var r = float.Parse(parts[4], CultureInfo.InvariantCulture);
                            var g = float.Parse(parts[5], CultureInfo.InvariantCulture);
                            var b = float.Parse(parts[6], CultureInfo.InvariantCulture);

                            vertices.Add(new Vec3(x, y, z));
                            //colors.Add(new Vec3(r, g, b)); //found color!
                            break;
                        }
                    }
                }
                else if (line.StartsWith($"f"))
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var f = new int[3];
                    var f2 = new int[3];

                    for (int i = 0; i < 3; i++)
                    {
                        var indices = parts[i + 1].Split('/');

                        if (indices.Length > 1) //is uv coordinates included?
                        {
                            int.TryParse(indices[0], out f[i]);
                            int.TryParse(indices[1], out f2[i]);
                        }
                        else
                        {
                            int.TryParse(indices[0], out f[i]);
                        }
                    }

                    var c = new Color32((byte)(CurrentColor.X * 255), (byte)(CurrentColor.Y * 255),
                        (byte)(CurrentColor.Z * 255));
                    AddTempTriangle(vertices[f[0] - 1], vertices[f[1] - 1], vertices[f[2] - 1], c);
                }
                else if (line.StartsWith($"usemtl"))
                {
                    string[] mm = line.Split("usemtl");
                    for (int m = 0; m < m_Materials.Count; m++)
                    {
                        if (m_Materials[m].Name == mm[1].Trim())
                        {
                            CurrentColor = m_Materials[m].Color;
                        }
                    }
                }
                else if (line.StartsWith($"mtllib"))
                {
                    string[] mm = line.Split("mtllib");
                    string materialFile = Path.Combine(Path.GetDirectoryName(filename), mm[1].Trim());
                    LoadMaterials(materialFile);
                }
            }

            CreateMesh();
        }

        private void CreateMesh()
        {
            Triangles = new Triangle[TempTriangles.Count];

            for (var i = 0; i < TempTriangles.Count; i++)
            {
                Triangles[i] = new Triangle();
                Triangles[i] = TempTriangles[i];
                
            }

            TempTriangles.Clear();
        }
    }
}