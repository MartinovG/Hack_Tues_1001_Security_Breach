using HackTues.Controls;
using HackTues.Engine;
using OpenTK.Mathematics;

namespace HackTues.Physics;

public class PhysicsEngine: IPhysicsEngine {
    public List<DynamicCollider> DynamicColliders => new();
    public List<ICollider> StaticColliders => new();
    public void Update(float delta, IController controller) {
        DynamicColliders.ForEach(c => {
            c.Update(delta, controller);
            var friction = c.Friction;
            c.Velocity *= new Vector2(MathF.Pow(friction.X, delta), MathF.Pow(friction.Y, delta));
            var x = delta * c.Velocity.X;
            var y = delta * c.Velocity.Y;
            CollidedX(c, x);
            CollidedY(c, y);
        });
    }
    private void CollidedX(DynamicCollider collider, float deltaX) {
        collider.Position = new(collider.Position.X + deltaX, collider.Position.Y);
        var collided = false;
        foreach (var s in StaticColliders) {
            if (s.CollidesWith(collider.Hitbox)) {
                collider.OnCollisionX(s);
                collided = true;
                break;
            }
        }
        foreach (var c in DynamicColliders) {
            if (c == collider) continue;
            if (collider.Hitbox.CollidesWith(c.Hitbox)) {
                collider.OnCollisionX(c);
                c.OnCollisionX(collider);
                collided = true;
                break;
            }
        }
        if (collided) {
            collider.Position = new(collider.Position.X - deltaX, collider.Position.Y);
            collider.Velocity = new(0, collider.Velocity.Y);
        }
    }
    private void CollidedY(DynamicCollider collider, float deltaY) {
        collider.Position = new(collider.Position.X, collider.Position.Y + deltaY);
        var collided = false;
        foreach (var s in StaticColliders) {
            if (s.CollidesWith(collider.Hitbox)) {
                collider.OnCollisionY(s);
                collided = true;
                break;
            }
        }
        foreach (var c in DynamicColliders) {
            if (c == collider)
                continue;
            if (collider.Hitbox.CollidesWith(c.Hitbox)) {
                collider.OnCollisionY(c);
                c.OnCollisionY(collider);
                collided = true;
                break;
            }
        }
        if (collided) {
            collider.Position = new(collider.Position.X, collider.Position.Y - deltaY);
            collider.Velocity = new(collider.Velocity.X, 0);
        }
    }
}