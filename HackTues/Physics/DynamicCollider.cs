using HackTues.Controls;
using HackTues.Engine;
using OpenTK.Mathematics;

namespace HackTues.Physics;

public abstract class DynamicCollider: ICollider {
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public abstract Vector2 Friction { get; }
    public abstract Hitbox Hitbox { get; }
    public abstract bool HasGravity { get; }

    public virtual void Update(float delta, IController controller) { }
    public virtual void OnCollisionX(ICollider other) { }
    public virtual void OnCollisionY(ICollider other) { }

    public bool CollidesWith(Hitbox hitbox) => Hitbox.CollidesWith(hitbox);
    public bool CollidesWith(Vector2 pt) => Hitbox.CollidesWith(pt);
}
