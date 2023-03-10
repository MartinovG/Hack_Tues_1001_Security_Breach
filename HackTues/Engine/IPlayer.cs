using HackTues.Controls;
using HackTues.Engine;
using OpenTK.Mathematics;

namespace HackTues.Engine;

public interface IPlayer: ILayerOwner {
    public Hitbox Hitbox { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 CameraPos { get; }

    public void Update(float delta, ICollider environment, IController? controller);
}
