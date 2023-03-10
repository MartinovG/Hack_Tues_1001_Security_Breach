using OpenTK.Mathematics;
using HackTues.Engine;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HackTues.Editor;

public class MoveAction: IAction {
    private Vector2 offset;
    private object obj;

    public bool Ended(Keys key) {
        return key == Keys.Space;
    }
    public void Update(float x, float y) {
        var pos = Program.Round(new Vector2(x, y) + offset);

        if (obj is Layer layer) layer.Position = pos;
        if (obj is ModifiableHitbox hb) hb.Value = new(pos, hb.Value.Size);
    }

    public MoveAction(Vector2 offset, Layer obj) {
        this.offset = obj.Position - offset;
        this.obj = obj;
    }
    public MoveAction(Vector2 offset, ModifiableHitbox obj) {
        this.offset = obj.Value.Pos1 - offset;
        this.obj = obj;
    }
}
