using HackTues.Engine;

namespace HackTues.Physics;
public interface IPhysicsEngine {
    public List<IDynamicCollider> DynamicColliders { get; }
    public List<ICollider> StaticColliders { get; }
    // Uses the methods from Game.cs
    public void Update(float delta);
}