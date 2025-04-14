using OpenTK.Mathematics;
using System.Diagnostics;

public class Moon
{
    private float distance;
    private float size;
    private float speed;
    private float angle;
    private float rotationSpeed;
    private float rotationAngle;
    private Sphere mesh;
    private Vector3 color;

    public Moon(float distance, float size, float speed, float rotationSpeed, Vector3 color)
    {
        this.distance = distance;
        this.size = size;
        this.speed = speed;
        this.rotationSpeed = rotationSpeed;
        this.angle = 0f;
        this.rotationAngle = 0f;
        this.color = color;

        mesh = new Sphere(size, 32, 32);
    }

    public void Update(float deltaTime)
    {
        angle += speed * deltaTime;
        rotationAngle += rotationSpeed * deltaTime;
        if (angle >= 360f)
            angle -= 360f;
        if (rotationAngle >= 360f)
            rotationAngle -= 360f;
    }

    public void Draw(Shader shader, Matrix4 userRotation)
    {
        float rad = MathHelper.DegreesToRadians(angle);
        float x = distance * MathF.Cos(rad);
        float z = distance * MathF.Sin(rad);
        Vector3 position = new Vector3(x, 0f, z);

        // Obracamy pozycję orbity za pomocą macierzy userRotation
        Vector4 homogeneousPosition = new Vector4(position, 1.0f);
        Vector4 rotatedHomogeneous = homogeneousPosition * userRotation; // Poprawione mnożenie
        Vector3 rotatedPosition = new Vector3(rotatedHomogeneous.X, rotatedHomogeneous.Y, rotatedHomogeneous.Z);

        // Tworzymy macierz translacji na obróconą pozycję
        Matrix4 orbit = Matrix4.CreateTranslation(rotatedPosition);

        // Obrót własny księżyca
        Matrix4 rotation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotationAngle));

        // Końcowa macierz modelu
        Matrix4 model = orbit * rotation;

        shader.SetMatrix4("model", model);
        shader.SetVector3("objectColor", color);

        mesh.Draw();

        Debug.WriteLine($"Moon Draw: angle={angle}, position=({x}, 0, {z}), rotatedPosition=({rotatedPosition.X}, {rotatedPosition.Y}, {rotatedPosition.Z})");
    }
}