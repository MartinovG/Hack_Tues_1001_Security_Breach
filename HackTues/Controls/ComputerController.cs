using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HackTues.Controls;

public class ComputerController: IController {
    private readonly bool[] values = new bool[Enum.GetValues<Button>().Length];
    private readonly bool[] polls = new bool[Enum.GetValues<Button>().Length];

    public bool Poll(Button btn) {
        if (polls[(int)btn]) {
            polls[(int)btn] = false;
            return true;
        }
        return false;
    }
    public bool Get(Button btn) {
        return values[(int)btn];
    }

    public void Update(Keys key, bool down) {
        var btn = key switch {
            Keys.W => Button.Up,
            Keys.Space => Button.Up,
            Keys.Up => Button.Up,
            Keys.S => Button.Down,
            Keys.Down => Button.Down,
            Keys.A => Button.Left,
            Keys.Left => Button.Left,
            Keys.D => Button.Right,
            Keys.Right => Button.Right,
            _ => (Button)(-1),
        };

        if (btn < 0)
            return;

        this.values[(int)btn] = down;
        this.polls[(int)btn] = down;
    }
    public Vector2 Position { get; private set; }
    public Vector2 Velocity { get; private set; }

    public void Update(Vector2 pos, float delta) {
        Velocity = (pos - Position) * delta;
        Position = pos;
    }
}
