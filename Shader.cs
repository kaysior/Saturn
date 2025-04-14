using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class Shader
{
    private readonly int handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        string vertexCode;
        string fragmentCode;

        try
        {
            vertexCode = System.IO.File.ReadAllText(vertexPath);
            fragmentCode = System.IO.File.ReadAllText(fragmentPath);
        }
        catch (Exception e)
        {
            throw new Exception($"Błąd podczas wczytywania shaderów: {e.Message}");
        }

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexCode);
        GL.CompileShader(vertexShader);

        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(vertexShader);
            throw new Exception($"Błąd kompilacji vertex shadera: {infoLog}");
        }

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentCode);
        GL.CompileShader(fragmentShader);

        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(fragmentShader);
            throw new Exception($"Błąd kompilacji fragment shadera: {infoLog}");
        }

        handle = GL.CreateProgram();
        GL.AttachShader(handle, vertexShader);
        GL.AttachShader(handle, fragmentShader);
        GL.LinkProgram(handle);

        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(handle);
            throw new Exception($"Błąd linkowania programu shadera: {infoLog}");
        }

        GL.DetachShader(handle, vertexShader);
        GL.DetachShader(handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Use()
    {
        GL.UseProgram(handle);
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(handle, name);
        GL.UniformMatrix4(location, false, ref matrix);
    }

    public void SetVector3(string name, Vector3 vector)
    {
        int location = GL.GetUniformLocation(handle, name);
        GL.Uniform3(location, vector);
    }

    public void SetFloat(string name, float value)
    {
        int location = GL.GetUniformLocation(handle, name);
        GL.Uniform1(location, value);
    }

    public void Dispose()
    {
        GL.DeleteProgram(handle);
    }
}