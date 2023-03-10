using OpenTK.Mathematics;

namespace HackTues.OpenGL.Shaders;

public class GLMatrixShader<T> : GLShader<T>
{
    private Matrix4 transformMatrix;
    private Matrix4 viewMatrix;

    public GLMatrixShader(string vert, string frag) : base(vert, frag)
    {
        TransformMatrix = Matrix4.Identity;
        ViewMatrix = Matrix4.Identity;
    }

    public Matrix4 TransformMatrix
    {
        get => transformMatrix;
        set
        {
            Use();
            transformMatrix = value;
            SetUniform("transform", value);
        }
    }
    public Matrix4 ViewMatrix
    {
        get => viewMatrix;
        set
        {
            Use();
            viewMatrix = value;
            SetUniform("view", value);
        }
    }
}
