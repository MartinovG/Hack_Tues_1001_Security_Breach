using OpenTK.Mathematics;

namespace HackTues.Engine;
public readonly struct Hitbox: ICollider {
    public Vector2 Pos { get; }
    public Vector2 Size { get; }

    public Vector2 Pos1 => Pos;
    public Vector2 Pos2 => Pos + Size;

    public static Hitbox operator +(Hitbox a, Vector2 b) => new(a.Pos + b, a.Size);
    public static Hitbox operator -(Hitbox a, Vector2 b) => new(a.Pos - b, a.Size);

    public bool CollidesWith(Hitbox other) {
        return
            Pos1.X < other.Pos2.X &&
            Pos2.X > other.Pos1.X &&
            Pos1.Y < other.Pos2.Y &&
            Pos2.Y > other.Pos1.Y;
    }
    public bool CollidesWith(Vector2 other) =>
        other.X >= Pos1.X && other.X <= Pos2.X &&
        other.Y >= Pos1.Y && other.Y <= Pos2.Y;

    public Hitbox(Vector2 pos, Vector2 size) {
        Pos = pos;
        Size = size;
    }

    public static Hitbox TwoPoints(Vector2 pos1, Vector2 pos2) {
        float x1 = Math.Min(pos1.X, pos2.X);
        float y1 = Math.Min(pos1.Y, pos2.Y);

        float x2 = Math.Max(pos1.X, pos2.X);
        float y2 = Math.Max(pos1.Y, pos2.Y);

        return new(new(x1, y1), new(x2 - x1, y2 - y1));
    }
}
