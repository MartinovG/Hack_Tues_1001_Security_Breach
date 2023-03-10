using HackTues.Controls;
using HackTues.Engine;
using HackTues.OpenGL;
using HackTues.OpenGL.Textures;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SkiaSharp;

namespace HackTues.Editor;

public class Program: GameWindow {
    private ComputerController controller = new();
    private Game game = new();
    private TopViewPlayer topPlayer = new(new("player", new(0), new(1), 1f));
    private SideViewPlayer sidePlayer = new(new("player", new(0), new(1), 1f));
    private Atlas atlas = new(2048, "D:/test/assets/textures");
    private GLRenderer gl;
    private GLMesh<SolidVertex> hitbox;

    protected override void OnKeyDown(KeyboardKeyEventArgs e) {
        if (e.IsRepeat)
            return;
        controller.Update(e.Key, true);
        base.OnKeyDown(e);
    }
    protected override void OnKeyUp(KeyboardKeyEventArgs e) {
        controller.Update(e.Key, false);
        base.OnKeyUp(e);
    }
    protected override void OnUpdateFrame(FrameEventArgs args) {
        var delta = (float)args.Time;
        controller.Update(MousePosition, delta);
        game.Update(controller, delta);
        base.OnUpdateFrame(args);
    }
    private void RenderHB(Hitbox hb) {
        gl.SolidShader.TransformMatrix = Matrix4.CreateScale(hb.Size.X, hb.Size.Y, 1) * Matrix4.CreateTranslation(new(hb.Pos));
        hitbox.Draw(Primitive.Triangles);
    }
    protected override void OnRenderFrame(FrameEventArgs args) {
        gl.NewFrame(Size);
        gl.SolidTexShader.ViewMatrix = Matrix4.CreateOrthographicOffCenter(0, 1440 / 64, 900 / 64, 0, -1, 1);
        gl.SolidShader.ViewMatrix = Matrix4.CreateOrthographicOffCenter(0, 1440 / 64, 900 / 64, 0, -1, 1);

        game.Render((float)args.Time);


        foreach (var hb in game.Map.Colliders) {
            RenderHB(hb);
        }

        base.OnRenderFrame(args);
        SwapBuffers();
    }

    public Program() : base(new(), new()) {
        Size = new(1440, 900);

        gl = new();
        hitbox = gl.Mesh(gl.SolidShader);

        atlas.LoadToGL(gl);
        atlas.Texture!.Interpolation = Interpolation.Nearest;
        gl.SolidTexShader.Atlas = atlas;

        gl.BackfaceCulling = false;
        gl.DepthTest = false;
        gl.Blending = true;

        game.Map = new Map();
        game.Map.Layers.Add(new("test-bg", new(0, 0), new(32, 32), -10));
        game.Map.Layers.Add(new("fikus", new(2.5f, 2.5f), new(.5f, .5f), .5f));
        game.Map.Colliders.Add(new(new(2.70f, 2.8f), new(.2f, .2f)));
        game.Player = topPlayer;

        hitbox.Data(new SolidVertex[] {
        new (new(0, 0), new(1, 0, 0, .5f)),
        new (new(1, 0), new(1, 0, 0, .5f)),
        new (new(0, 1), new(1, 0, 0, .5f)),
        new (new(1, 1), new(1, 0, 0, .5f)),
    }, new[] {
        0, 1, 2,
        1, 2, 3,
    });

        Layer.Init(gl);
        base.OnLoad();
    }

    public static void Main() {
        new Program().Run();
    }
}