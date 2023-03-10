using HackTues.OpenGL.Shaders;
using OpenTK.Graphics.OpenGL;

namespace HackTues.OpenGL;
public class GLMesh<T>
{
    internal int count;
    internal int vbo, ebo, vao;
    private bool disposedValue;

    public GLShader<T> Shader { get; private set; }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                GL.DeleteBuffer(vbo);
                GL.DeleteBuffer(ebo);
                GL.DeleteVertexArray(vao);
            }

            disposedValue = true;
        }
    }

    public void Data(T[] vertices, int[] indices)
    {
        count = indices.Length;
        var data = Shader.Serialize(vertices);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * 4, data, BufferUsageHint.StreamDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * 4, indices, BufferUsageHint.StreamDraw);
    }
    public void Data(T[] vertices) {
        int[] indicies = new int[vertices.Length];

        for (int i = 0; i < indicies.Length; i++) {
            indicies[i] = i;
        }

        Data(vertices, indicies);
    }

    public void Draw(Primitive primitive = Primitive.Triangles)
    {
        Shader.Use();
        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.DrawElements(primitive switch
        {
            Primitive.Quads => PrimitiveType.Quads,
            Primitive.Points => PrimitiveType.Points,
            _ => PrimitiveType.Triangles,
        }, count, DrawElementsType.UnsignedInt, 0);
    }

    public GLMesh(GLShader<T> shader)
    {
        Shader = shader;
        GL.CreateVertexArrays(1, out vao);
        GL.CreateBuffers(1, out vbo);
        GL.CreateBuffers(1, out ebo);

        shader.SetupVAO(vao, vbo, ebo);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
