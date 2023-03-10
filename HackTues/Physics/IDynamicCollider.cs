using HackTues.Engine;
using OpenTK.Mathematics;

namespace HackTues.Physics;

public interface IDynamicCollider: ICollider {
    Vector2 Position { get; set; }
    Vector2 Velocity { get; set; }
    void OnCollision(ICollider other);
}
