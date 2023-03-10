using HackTues.Controls;
using OpenTK.Mathematics;

namespace HackTues.Engine;

public class TopViewPlayer: Player {
    protected override Vector2 Friction { get; } = new(0.000001f, 0.000001f);

    protected override Vector2 GetAcceleration(float delta, IController? controller) {
        var acc = Vector2.Zero;
        if (controller == null) return acc;

        if (controller.Get(Button.Left)) {
            acc -= Vector2.UnitX;
        }
        if (controller.Get(Button.Right)) {
            acc += Vector2.UnitX;
        }
        if (controller.Get(Button.Up)) {
            acc -= Vector2.UnitY;
        }
        if (controller.Get(Button.Down)) {
            acc += Vector2.UnitY;
        }

        if (acc.Length > 1) {
            acc.Normalize();
        }
        acc *= 50;

        return acc;
    }

    public TopViewPlayer(Layer layer) : base(layer) {
        Hitbox = new(new(0, .75f), new(1, .25f));
    }
}
