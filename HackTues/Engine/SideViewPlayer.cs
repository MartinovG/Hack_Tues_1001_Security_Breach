using HackTues.Controls;
using HackTues.Engine;
using OpenTK.Mathematics;

namespace HackTues.Engine;

public class SideViewPlayer: Player {
    private float kyoteeTime = 0;
    private float jumpCooldown = 0;

    public override Vector2 CameraPos => throw new NotImplementedException();
    protected override Vector2 Friction { get; } = new(0.000001f, 0.1f);

    public override void Update(float delta, ICollider environment, IController? controller) {
        base.Update(delta, environment, controller);

        jumpCooldown -= delta;
        kyoteeTime -= delta;
    }
    protected override void HitX() {
        base.HitX();
        kyoteeTime = .1f;
    }
    protected override void HitY() {
        base.HitY();
        kyoteeTime = .1f;
    }
    protected override Vector2 GetAcceleration(float detla, IController? controller) {
        var acc = Vector2.Zero;
        if (controller == null) return acc;
        if (controller.Get(Button.Left)) {
            acc -= Vector2.UnitX * 50;
        }
        if (controller.Get(Button.Right)) {
            acc += Vector2.UnitX * 50;
        }
        if (controller.Get(Button.Up) && kyoteeTime > 0 && jumpCooldown < 0 && controller.Poll(Button.Up)) {
            vel.Y = -20;
            kyoteeTime = 0;
            jumpCooldown = .25f;
        }

        acc += Vector2.UnitY * 50;

        return acc;
    }

    public SideViewPlayer(Layer layer): base(layer) {
    }
}
