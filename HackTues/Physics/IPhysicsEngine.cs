using HackTues.Controls;
using HackTues.Engine;

namespace HackTues.Physics;
public interface IPhysicsEngine {
    public List<DynamicCollider> DynamicColliders { get; }
    public List<ICollider> StaticColliders { get; }
    // Uses the methods from Game.cs
    public void Update(float delta, IController controller);
}
