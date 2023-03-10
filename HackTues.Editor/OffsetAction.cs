using OpenTK.Mathematics;
using HackTues.Engine;

namespace HackTues.Editor;

public class OffsetAction: IAction {
    private Vector2 offset1;
    private Vector2 offset2;
    private Layer obj;

    public void Update(float x, float y) {
        var pos1 = Program.Round((new Vector2(x, y) + offset1));
        var pos2 = Program.Round((new Vector2(x, y) + offset2));
        obj.Position = pos2;
        obj.Origin = pos1;
    }

    public OffsetAction(float x, float y, Layer obj) {
        this.offset1 = (obj.Origin - new Vector2(x, y));
        this.offset2 = (obj.Position - new Vector2(x, y));
        this.obj = obj;
    }
}
