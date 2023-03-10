using OpenTK.Mathematics;
using HackTues.Engine;

namespace HackTues.Editor;

public class OffsetAction: IAction {
    private float offset;
    private Layer obj;

    public void Update(float x, float y) {
        var pos = Program.Round(new Vector2(x, y + offset));
        obj.YOffset = pos.Y;
    }

    public OffsetAction(float y, Layer obj) {
        this.offset = obj.YOffset - y;
        this.obj = obj;
    }
}
