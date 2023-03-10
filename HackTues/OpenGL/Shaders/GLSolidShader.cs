using OpenTK.Graphics.OpenGL;

namespace HackTues.OpenGL.Shaders;

public class GLSolidShader : GLMatrixShader<SolidVertex>
{
    public GLSolidShader() : base(
        @"
#version 330 core

in vec2 in_pos;
in vec4 in_col;

out vec4 out_col;

uniform mat4 transform;
uniform mat4 view;

void main() {
    gl_Position = view * transform * vec4(in_pos, 0, 1);
    out_col = in_col;
}",
        @"
#version 330 core

in vec4 out_col;
out vec4 color;

void main() {
    color = out_col;
}")
    { }

    internal override void SetupVAO(int vao, int vbo, int ebo)
    {
        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

        SetAttrib(program, "in_pos", 2, VertexAttribPointerType.Float, 24, 0);
        SetAttrib(program, "in_col", 4, VertexAttribPointerType.Float, 24, 8);
    }
    public override float[] Serialize(SolidVertex[] data)
    {
        float[] res = new float[data.Length * 7];

        for (int i = 0; i < data.Length; i++)
        {
            res[i * 6 + 0] = data[i].in_pos.X;
            res[i * 6 + 1] = data[i].in_pos.Y;

            res[i * 6 + 2] = data[i].in_col.X;
            res[i * 6 + 3] = data[i].in_col.Y;
            res[i * 6 + 4] = data[i].in_col.Z;
            res[i * 6 + 5] = data[i].in_col.W;
        }

        return res;
    }
}
