using OpenTK.Mathematics;

namespace HackTues.OpenGL;

public record struct SolidTexVertex(Vector2 in_pos, Vector4 in_col, Vector2 in_tex) {
    public SolidTexVertex(Vector2 in_pos, Vector2 in_tex) : this(in_pos, new Vector4(1), in_tex) {
    }
}
