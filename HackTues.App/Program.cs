using HackTues.Controls;
using HackTues.Engine;
using HackTues.OpenGL;
using HackTues.OpenGL.Textures;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SkiaSharp;

namespace HackTues.App;

public class Program: GameWindow {
    private static string GetAssetsPath() {
        if (File.Exists(".assets")) {
            var r = new StreamReader(".assets");
            var path = r.ReadToEnd();
            path = Path.GetFullPath(path);
            r.Close();
            if (path != null)
                return path;
        }

        var w = new StreamWriter(".assets");
        w.Write("assets");
        w.Close();
        return Path.GetFullPath("assets");
    }
    public static Vector2 Round(Vector2 vec) {
        return new(
            MathF.Floor(vec.X * 16) / 16,
            MathF.Floor(vec.Y * 16) / 16
        );
    }

    private ComputerController controller = new();
    private Game game = new();
    private TopViewPlayer topPlayer = new(new("player", new(0), new(1, 2), 2f));
    private SideViewPlayer sidePlayer = new(new("player", new(0), new(1), 1f));
    private Atlas atlas = new(2048, Path.Join(GetAssetsPath(), "textures"));
    private GLRenderer gl;
    private GLMesh<SolidVertex> hitbox;

    private void RenderHB(Hitbox hb) {
        gl.SolidShader.TransformMatrix = Matrix4.CreateScale(hb.Size.X, hb.Size.Y, 1) * Matrix4.CreateTranslation(new(hb.Pos));
        hitbox.Draw(Primitive.Triangles);
    }
    private void LoadMap(string name) {
        game.Map = Map.Load(new FileStream(Path.Join(GetAssetsPath(), "maps/" + name), FileMode.Open));
        game.Player = sidePlayer;
        game.Player!.Position = game.Map.Spawn;
    }
    private void LoadSpawn() {
        game.Map = Map.Load(new FileStream(Path.Join(GetAssetsPath(), "maps/spawn"), FileMode.Open));
        game.Player = topPlayer;
        game.Player!.Position = game.Map.Spawn;
    }

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
    protected override void OnRenderFrame(FrameEventArgs args) {
        gl.NewFrame(Size);
        var view = Matrix4.CreateTranslation(new(Round(new Vector2(1440, 900) / 256) - Round(game.Player!.CameraPos))) * Matrix4.CreateOrthographicOffCenter(0, 1440 / 128, 900 / 128, 0, -1, 1);
        gl.SolidTexShader.ViewMatrix = view;
        gl.SolidShader.ViewMatrix = view;

        game.Render((float)args.Time);

        //foreach (var hb in game.Map.Colliders) {
        //    RenderHB(hb);
        //}

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

        game.Player = topPlayer;

        LoadSpawn();

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