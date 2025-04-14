using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

public class Sphere
{
    public List<Vector3> Vertices { get; } = new();
    public List<uint> Indices { get; } = new();

    public int Vao { get; private set; }

    public Sphere(float radius, int sectorCount, int stackCount)
    {
        // Tworzenie wierzchołków i indeksów
        for (int i = 0; i <= stackCount; ++i)
        {
            float stackAngle = MathHelper.PiOver2 - i * MathHelper.Pi / stackCount;
            float xy = radius * MathF.Cos(stackAngle);
            float z = radius * MathF.Sin(stackAngle);

            for (int j = 0; j <= sectorCount; ++j)
            {
                float sectorAngle = j * 2 * MathHelper.Pi / sectorCount;
                float x = xy * MathF.Cos(sectorAngle);
                float y = xy * MathF.Sin(sectorAngle);

                Vertices.Add(new Vector3(x, y, z));
            }
        }

        for (int i = 0; i < stackCount; ++i)
        {
            int k1 = i * (sectorCount + 1);
            int k2 = k1 + sectorCount + 1;

            for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
            {
                if (i != 0)
                {
                    Indices.Add((uint)k1);
                    Indices.Add((uint)(k1 + 1));
                    Indices.Add((uint)k2);
                }

                if (i != stackCount - 1)
                {
                    Indices.Add((uint)(k1 + 1));
                    Indices.Add((uint)(k2 + 1));
                    Indices.Add((uint)k2);
                }
            }
        }

        // Tworzenie VAO, VBO i EBO
        Vao = GL.GenVertexArray();
        int vbo = GL.GenBuffer();
        int ebo = GL.GenBuffer();

        GL.BindVertexArray(Vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Count * Vector3.SizeInBytes, Vertices.ToArray(), BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Count * sizeof(uint), Indices.ToArray(), BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);
    }

    public void Draw()
    {
        GL.BindVertexArray(Vao);
        GL.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);
    }
}
