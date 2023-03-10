using OpenTK.Mathematics;

namespace HackTues.Engine;

public interface ICollider {
    public bool CollidesWith(Hitbox hitbox);
    public bool CollidesWith(Vector2 pt);
}
