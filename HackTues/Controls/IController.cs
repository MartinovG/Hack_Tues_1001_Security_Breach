using OpenTK.Mathematics;

namespace HackTues.Controls;

public interface IController {
    bool Poll(Button btn);
    bool Get(Button btn);
    Vector2 Position { get; }
    Vector2 Velocity { get; }
}
