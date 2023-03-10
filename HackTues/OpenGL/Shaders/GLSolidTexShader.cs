using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace HackTues.OpenGL.Shaders;

public class GLSolidTexShader : GLMatrixShader<SolidTexVertex>
{
    public GLSolidTexShader() : base(
            @"
#version 330 core

in vec3 in_pos;
in vec4 in_col;
in vec2 in_tex;

out vec4 out_col;
out vec3 out_tex;

uniform mat4 transform;
uniform mat4 view;
uniform mat4 tex_transform;

void main() {
    gl_Position = view * transform * vec4(in_pos, 1);
    out_col = in_col;
    out_tex = (tex_transform * vec4(in_tex, 0, 1)).xyz;
}",
            @"
#version 330 core
#extension GL_EXT_texture_array : enable

uniform sampler2DArray texture;

in vec4 out_col;
in vec3 out_tex;
out vec4 color;

void main() {
    color = texture2DArray(texture, out_tex) * out_col;
}")
    {
        SetUniform("texture", 0);
    }

    public IAtlas? Atlas { get; set; }
    public  string? Texture { get; set; }

    public override void Use()
    {
        base.Use();
    
        if (Texture != null && Atlas != null) {
            var loc = Atlas[Texture];

            SetUniform("tex_transform",
                Matrix4.CreateScale(loc.Hitbox.Size.X, loc.Hitbox.Size.Y, 1) *
                Matrix4.CreateTranslation(loc.Hitbox.Pos.X, loc.Hitbox.Pos.Y, loc.PageIndex)
            );

            Atlas.Use(0);
        }
    }
    internal override void SetupVAO(int vao, int vbo, int ebo)
    {
        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

        SetAttrib(program, "in_pos", 3, VertexAttribPointerType.Float, 32, 0);
        SetAttrib(program, "in_col", 4, VertexAttribPointerType.Float, 32, 8);
        SetAttrib(program, "in_tex", 2, VertexAttribPointerType.Float, 32, 24);
    }
    public override float[] Serialize(SolidTexVertex[] data)
    {
        float[] res = new float[data.Length * 9];

        for (int i = 0; i < data.Length; i++)
        {
            res[i * 8 + 0] = data[i].in_pos.X;
            res[i * 8 + 1] = data[i].in_pos.Y;

            res[i * 8 + 2] = data[i].in_col.X;
            res[i * 8 + 3] = data[i].in_col.Y;
            res[i * 8 + 4] = data[i].in_col.Z;
            res[i * 8 + 5] = data[i].in_col.Z;

            res[i * 8 + 6] = data[i].in_tex.X;
            res[i * 8 + 7] = data[i].in_tex.Y;
        }

        return res;
    }
}
