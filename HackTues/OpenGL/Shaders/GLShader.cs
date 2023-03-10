using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace HackTues.OpenGL.Shaders;

static class ShaderIDKeeper
{
    internal static int currId = 0;
}

public class GLShader<T>
{
    internal int program;

    public virtual void Use()
    {
        if (ShaderIDKeeper.currId == program) return;
        GL.UseProgram(program);
        ShaderIDKeeper.currId = program;
    }

    public void SetUniform(string name, Vector4 value) => GL.Uniform4(GL.GetUniformLocation(program, name), value);
    public void SetUniform(string name, Vector3 value) => GL.Uniform3(GL.GetUniformLocation(program, name), value);
    public void SetUniform(string name, Vector2 value) => GL.Uniform2(GL.GetUniformLocation(program, name), value);
    public void SetUniform(string name, Matrix4 value) => GL.UniformMatrix4(GL.GetUniformLocation(program, name), false, ref value);
    public void SetUniform(string name, float value) => GL.Uniform1(GL.GetUniformLocation(program, name), value);
    public void SetUniform(string name, int value) => GL.Uniform1(GL.GetUniformLocation(program, name), value);

    protected void SetAttrib(int program, string name, int size, VertexAttribPointerType type, int stride, int offset)
    {
        int index = GL.GetAttribLocation(program, name);
        GL.EnableVertexAttribArray(index);
        GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, stride, offset);
    }

    internal virtual void SetupVAO(int vao, int vbo, int ebo) { }
    public virtual float[] Serialize(T[] data) => throw new NotImplementedException();

    public GLShader(string vert, string frag)
    {
        int vertI = GL.CreateShader(ShaderType.VertexShader);
        int fragI = GL.CreateShader(ShaderType.FragmentShader);

        GL.ShaderSource(vertI, vert);
        GL.CompileShader(vertI);
        GL.GetShader(vertI, ShaderParameter.CompileStatus, out int success);

        if (success == 0)
        {
            throw new Exception(GL.GetShaderInfoLog(vertI));
        }

        GL.ShaderSource(fragI, frag);
        GL.CompileShader(fragI);
        GL.GetShader(fragI, ShaderParameter.CompileStatus, out success);

        if (success == 0)
        {
            throw new Exception(GL.GetShaderInfoLog(fragI));
        }

        program = GL.CreateProgram();

        GL.AttachShader(program, vertI);
        GL.AttachShader(program, fragI);

        GL.LinkProgram(program);

        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out success);

        if (success == 0)
        {
            throw new Exception(GL.GetProgramInfoLog(program));
        }

        GL.DetachShader(program, vertI);
        GL.DetachShader(program, fragI);

        GL.DeleteShader(vertI);
        GL.DeleteShader(fragI);

        GL.UseProgram(program);

        ShaderIDKeeper.currId = program;
    }
}
