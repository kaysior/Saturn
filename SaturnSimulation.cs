using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class SaturnSimulation : GameWindow
{
    private Shader shader;
    private int vao, vbo, ebo;
    private Sphere sphere;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private float zoom = -30f; // Zmniejszony domyślny zoom dla lepszej widoczności

    private float animationSpeed = 1f; // mnożnik animacji
    private float dayDuration = 10f;   // 10 sekund = 1 doba Saturna

    private float planetRotation = 0f;

    private List<Moon> moons = new();
    private Ring[] rings;

    public SaturnSimulation()
        : base(GameWindowSettings.Default,
               new NativeWindowSettings()
               {
                   Size = new Vector2i(1280, 720),
                   Title = "Saturn 3D",
                   Flags = ContextFlags.ForwardCompatible,
                   APIVersion = new Version(4, 6)
               })
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0f, 0f, 0f, 1f);
        GL.Enable(EnableCap.DepthTest);

        shader = new Shader("Shaders/vertex.glsl", "Shaders/fragment.glsl");

        sphere = new Sphere(2f, 64, 64); // Promień planety = 2f

        // Pierścienie A, B, C, D, F
        rings = new[]
        {
            new Ring(2.30f, 2.56f, 100, 40.0f, shader, new Vector3(0.6f, 0.6f, 0.55f)), // D
            new Ring(2.56f, 3.16f, 100, 30.0f, shader, new Vector3(0.7f, 0.65f, 0.5f)),  // C
            new Ring(3.16f, 4.03f, 100, 25.0f, shader, new Vector3(0.9f, 0.8f, 0.6f)),   // B
            new Ring(4.19f, 4.70f, 100, 20.0f, shader, new Vector3(0.8f, 0.7f, 0.5f)),   // A
            new Ring(4.81f, 4.82f, 100, 18.0f, shader, new Vector3(0.85f, 0.85f, 0.9f)), // F
        };

        // 10 księżyców z większymi rozmiarami i mniejszymi odległościami
        moons = new List<Moon>
        {
            // Tytan: większy rozmiar, bliższa orbita
            new Moon(15.0f, 0.5f, 360f / 159.5f, 30f, new Vector3(0.8f, 0.8f, 1.0f)),
            // Enceladus: mały, ale widoczny
            new Moon(6.0f, 0.15f, 360f / 13.7f, 20f, new Vector3(1.0f, 0.7f, 0.7f)),
            // Dione: średni rozmiar
            new Moon(8.0f, 0.3f, 360f / 27.4f, 15f, new Vector3(0.9f, 0.9f, 0.6f)),
            // Rhea: większy od średniej
            new Moon(10.0f, 0.4f, 360f / 45.2f, 12f, new Vector3(0.6f, 1.0f, 0.6f)),
            // Mimas: mały
            new Moon(7.0f, 0.1f, 360f / 9.4f, 25f, new Vector3(1.0f, 1.0f, 1.0f)),
            // Hyperion: mały, dalsza orbita
            new Moon(12.0f, 0.12f, 360f / 212.8f, 10f, new Vector3(1.0f, 0.5f, 0.5f)),
            // Iapetus: średni rozmiar
            new Moon(20.0f, 0.35f, 360f / 793.3f, 8f, new Vector3(0.4f, 0.7f, 1.0f)),
            // Janus: mały
            new Moon(6.5f, 0.13f, 360f / 6.9f, 18f, new Vector3(0.7f, 1.0f, 0.7f)),
            // Phoebe: średni rozmiar, najdalej
            new Moon(30.0f, 0.2f, 360f / 5505.6f, 5f, new Vector3(1.0f, 0.9f, 0.6f)),
            // Pandora: najmniejszy
            new Moon(6.2f, 0.1f, 360f / 6.3f, 22f, new Vector3(0.8f, 0.6f, 1.0f)),
        };

        // VAO/VBO/EBO dla planety
        vao = GL.GenVertexArray();
        vbo = GL.GenBuffer();
        ebo = GL.GenBuffer();

        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer,
            sphere.Vertices.Count * Vector3.SizeInBytes,
            sphere.Vertices.ToArray(),
            BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer,
            sphere.Indices.Count * sizeof(uint),
            sphere.Indices.ToArray(),
            BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        var input = KeyboardState;

        // Obracanie modelu za pomocą strzałek
        if (input.IsKeyDown(Keys.Up))
        {
            rotationX -= 100f * (float)args.Time;
            Debug.WriteLine($"Up pressed, rotationX: {rotationX}");
        }
        if (input.IsKeyDown(Keys.Down))
        {
            rotationX += 100f * (float)args.Time;
            Debug.WriteLine($"Down pressed, rotationX: {rotationX}");
        }
        if (input.IsKeyDown(Keys.Left))
        {
            rotationY -= 100f * (float)args.Time;
            Debug.WriteLine($"Left pressed, rotationY: {rotationY}");
        }
        if (input.IsKeyDown(Keys.Right))
        {
            rotationY += 100f * (float)args.Time;
            Debug.WriteLine($"Right pressed, rotationY: {rotationY}");
        }

        if (input.IsKeyDown(Keys.PageUp))
        {
            zoom += 100f * (float)args.Time;
            Debug.WriteLine($"PageUp pressed, zoom: {zoom}");
        }
        if (input.IsKeyDown(Keys.PageDown))
        {
            zoom -= 100f * (float)args.Time;
            Debug.WriteLine($"PageDown pressed, zoom: {zoom}");
        }

        // Obsługa zmiany tempa animacji
        for (int i = 0; i <= 9; i++)
        {
            if (input.IsKeyPressed((Keys)((int)Keys.D0 + i)))
            {
                if (i == 0)
                    animationSpeed = 0f;      // Brak animacji
                else if (i == 1)
                    animationSpeed = 0.25f;
                else if (i == 2)
                    animationSpeed = 0.5f;
                else if (i == 3)
                    animationSpeed = 0.75f;
                else if (i == 4)
                    animationSpeed = 1f;      // 10s/doba (domyślne)
                else if (i == 5)
                    animationSpeed = 1.25f;
                else if (i == 6)
                    animationSpeed = 1.5f;
                else if (i == 7)
                    animationSpeed = 1.65f;
                else if (i == 8)
                    animationSpeed = 1.85f;
                else if (i == 9)
                    animationSpeed = 2f;       // 5s/doba
                Debug.WriteLine($"Animation speed set to: {animationSpeed} (doba: {10f / animationSpeed}s)");
            }
        }

        // Aktualizacja obrotu planety
        float delta = (float)args.Time * animationSpeed;

        planetRotation += 360f / dayDuration * delta;
        planetRotation %= 360f;

        foreach (var moon in moons)
            moon.Update(delta);

        foreach (var ring in rings)
            ring.Update(delta);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        shader.Use();

        // Globalna macierz obrotu użytkownika
        Matrix4 userRotation = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotationX)) *
                              Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotationY));

        // Rysowanie planety
        Matrix4 planetModel = userRotation * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(planetRotation));
        Matrix4 view = Matrix4.CreateTranslation(0f, 0f, zoom);
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 1000f);

        shader.SetMatrix4("view", view);
        shader.SetMatrix4("projection", projection);
        shader.SetMatrix4("model", planetModel);
        shader.SetVector3("objectColor", new Vector3(1.0f, 0.9f, 0.6f));
        sphere.Draw();

        // Rysowanie pierścieni
        foreach (var ring in rings)
        {
            ring.Draw(userRotation);
        }

        // Rysowanie księżyców
        foreach (var moon in moons)
        {
            moon.Draw(shader, userRotation);
        }

        SwapBuffers();

        Debug.WriteLine($"Render: rotationX={rotationX}, rotationY={rotationY}, planetRotation={planetRotation}");
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);
        GL.DeleteVertexArray(vao);
        shader.Dispose();
    }
}