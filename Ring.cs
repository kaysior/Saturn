using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

public class Ring
{
    private int vao, vbo;
    private int vertexCount;
    private float rotationSpeed;
    private float rotationAngle;
    private Shader shader;
    private Vector3 color;

    public Ring(float innerRadius, float outerRadius, int segments, float rotationSpeed, Shader shader, Vector3 color)
    {
        this.rotationSpeed = rotationSpeed;
        this.rotationAngle = 0f;
        this.shader = shader;
        this.color = color;

        List<float> vertices = new List<float>();

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * MathHelper.TwoPi / segments;
            float angle2 = (i + 1) * MathHelper.TwoPi / segments;

            float x1Inner = innerRadius * MathF.Cos(angle1);
            float y1Inner = innerRadius * MathF.Sin(angle1);
            float x1Outer = outerRadius * MathF.Cos(angle1);
            float y1Outer = outerRadius * MathF.Sin(angle1);

            float x2Inner = innerRadius * MathF.Cos(angle2);
            float y2Inner = innerRadius * MathF.Sin(angle2);
            float x2Outer = outerRadius * MathF.Cos(angle2);
            float y2Outer = outerRadius * MathF.Sin(angle2);

            vertices.AddRange(new float[]
            {
                x1Inner, y1Inner, 0,
                x1Outer, y1Outer, 0,
                x2Outer, y2Outer, 0,

                x1Inner, y1Inner, 0,
                x2Outer, y2Outer, 0,
                x2Inner, y2Inner, 0
            });
        }

        vertexCount = vertices.Count / 3;

        vao = GL.GenVertexArray();
        vbo = GL.GenBuffer();

        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);
    }

    public void Update(float deltaTime)
    {
        rotationAngle += rotationSpeed * deltaTime;
        if (rotationAngle >= 360f)
            rotationAngle -= 360f;
    }

    public void Draw(Matrix4 userRotation)
    {
        Matrix4 rotation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotationAngle));
        Matrix4 model = userRotation * rotation;
        shader.SetMatrix4("model", model);
        shader.SetVector3("objectColor", color);

        GL.BindVertexArray(vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
        GL.BindVertexArray(0);
    }
}