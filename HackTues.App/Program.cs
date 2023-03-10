using HackTues.Controls;
using HackTues.Engine;
using HackTues.OpenGL;
using HackTues.OpenGL.Textures;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SkiaSharp;
using System;

namespace HackTues.App;

public class Program: GameWindow, ILevelLoader, ILobbyLoader {
    public static Vector2 Round(Vector2 vec) {
        return new(
            MathF.Floor(vec.X),
            MathF.Floor(vec.Y)
        );
    }

    private ComputerController controller = new();
    private LevelGame levelGame;
    private LobbyGame lobbyGame;
    private IGame currGame;
    private IAtlas atlas;
    private GLRenderer gl;
    private GLMesh<SolidVertex> hitbox;

    private void RenderHB(Hitbox hb) {
        gl.SolidShader.TransformMatrix = Matrix4.CreateScale(hb.Size.X, hb.Size.Y, 1) * Matrix4.CreateTranslation(new(hb.Pos));
        hitbox.Draw(Primitive.Triangles);
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
        currGame.Update(delta, controller);
        base.OnUpdateFrame(args);
    }
    protected override void OnRenderFrame(FrameEventArgs args) {
        gl.NewFrame(Size);
        var view =
            Matrix4.CreateTranslation(new(Round(new Vector2(1440, 900) / 4) - Round(currGame.CameraPos))) *
            Matrix4.CreateOrthographicOffCenter(0, 1440 / 2, 900 / 2, 0, -1, 1);
        gl.SolidTexShader.ViewMatrix = view;
        gl.SolidShader.ViewMatrix = view;

        var layers = new SortedSet<Layer>();
        currGame.AddLayers(layers);

        foreach (var layer in layers) {
            layer.Render();
        }

        base.OnRenderFrame(args);
        SwapBuffers();
    }

    public void LoadMap(string name) {
        currGame = levelGame;
        levelGame.LoadMap(name);
    }
    public void LoadLobby() {
        currGame = lobbyGame;
    }

    public Program() : base(new(), new()) {
        Size = new(1440, 900);

        lobbyGame = new(this);
        levelGame = new(this);

        gl = new();
        hitbox = gl.Mesh(gl.SolidShader);
        currGame = lobbyGame;


        atlas = new GameAtlas(new Atlas(2048, Path.Join(Game.AssetsPath, "textures"), gl), 0);
        gl.SolidTexShader.Atlas = atlas;

        gl.BackfaceCulling = false;
        gl.DepthTest = false;
        gl.Blending = true;

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