using HackTues.OpenGL;
using HackTues.OpenGL.Shaders;
using OpenTK.Mathematics;
using static OpenTK.Graphics.OpenGL.GL;

namespace HackTues.Engine;

public class Layer: IComparable<Layer> {
    public static GLMesh<SolidTexVertex>? mesh { get; private set; }
    public static GLSolidTexShader? shader { get; private set; }

    public Vector2 Position { get; set; }
    public float YOffset { get; set; }
    public Vector2 Size { get; }
    public string Texture { get; }

    public void Render() {
        var pos = new Vector2((int)(Position.X * 16) / 16f, (int)(Position.Y * 16) / 16f);
        if (shader == null || mesh == null) {
            throw new InvalidOperationException("A layer may not be rendered, before Layer.Init has been called.");
        }

        shader.TransformMatrix =
            Matrix4.CreateScale(Size.X, Size.Y, 1) *
            Matrix4.CreateTranslation(new(pos));
        shader.Texture = Texture;
        mesh.Draw(Primitive.Triangles);
    }

    public int CompareTo(Layer? other) {
        if (other is null) throw new ArgumentNullException(nameof(other));
            return (this.Position.Y + this.YOffset).CompareTo(other.Position.Y + other.YOffset);
    }

    public Layer(string texture, Vector2 pos, Vector2 size, float yOffset = 0) {
        this.Texture = texture;
        this.Size = size;
        this.Position = pos;
        this.YOffset = yOffset;
    }

    public static bool operator ==(Layer left, Layer right) => left.Equals(right);
    public static bool operator !=(Layer left, Layer right) => !(left == right);

    public static bool operator <(Layer left, Layer right) => left.CompareTo(right) < 0;
    public static bool operator <=(Layer left, Layer right) => left.CompareTo(right) <= 0;
    public static bool operator >(Layer left, Layer right) => left.CompareTo(right) > 0;
    public static bool operator >=(Layer left, Layer right) => left.CompareTo(right) >= 0;

    public static Layer operator +(Layer left, Vector2 right) => new(left.Texture, left.Position + right, left.Size, left.YOffset);
    public static Layer operator -(Layer left, Vector2 right) => new(left.Texture, left.Position - right, left.Size, left.YOffset);

    public static void Init(GLRenderer r) {
        if (mesh != null)
            return;

        mesh = r.Mesh(r.SolidTexShader);
        mesh.Data(new SolidTexVertex[] {
            new(new(0, 0), new(0, 0)),
            new(new(0, 1), new(0, 1)),
            new(new(1, 0), new(1, 0)),
            new(new(1, 1), new(1, 1)),
        }, new int[] {
            0, 1, 2,
            1, 2, 3,
        });
        shader = r.SolidTexShader;
    }
}
