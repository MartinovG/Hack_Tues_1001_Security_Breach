using HackTues.Controls;
using HackTues.Engine;
using OpenTK.Mathematics;

namespace HackTues.Engine;

public interface IGame: ILayerOwner {
    public Vector2 PlayerPosition { get; set; }
    public Vector2 CameraPos { get; }
    public Map Map { get; }
    public bool Switch { get; }

    public void Update(float delta, IController? controller);
}
