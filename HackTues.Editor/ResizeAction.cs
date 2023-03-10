using OpenTK.Mathematics;
using HackTues.Engine;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HackTues.Editor;

public class ResizeAction: IAction {
    private Vector2 offset;
    private Vector2 pos1;
    private ModifiableHitbox hb;

    public bool Ended(Keys key) {
        return key == Keys.Enter;
    }
    public void Update(float x, float y) {
        var pos = Program.Round(new Vector2(x, y) + offset);

        hb.Value = Hitbox.TwoPoints(pos1, pos);
    }

    public ResizeAction(Vector2 offset, ModifiableHitbox obj) {
        this.offset = obj.Value.Pos2 - offset;
        this.hb = obj;
        this.pos1 = obj.Value.Pos1;
    }
}
