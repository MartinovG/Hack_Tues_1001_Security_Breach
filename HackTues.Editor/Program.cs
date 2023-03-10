using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using HackTues.Engine;
using HackTues.OpenGL;
using HackTues.OpenGL.Textures;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HackTues.Editor;

public class Program: GameWindow {
    public static Vector2 Round(Vector2 vec) {
        return new(
            MathF.Floor(vec.X * 16) / 16,
            MathF.Floor(vec.Y * 16) / 16
        );
    }

    private string assetsPath = Path.GetFullPath("./assets");
    private List<ModifiableHitbox> colliders = new();
    private List<Layer> layers = new();
    private Vector2 spawn;
    private Atlas atlas = new(2048, "D:/test/assets/textures");
    private GLRenderer gl;
    private GLMesh<SolidVertex> hitbox;
    private float x, y, scale = 1, speed = 4;

    private Keys activatorKey = 0;
    private IAction? action = null;
    private object? selection = null;

    private void ChangeSelecection() {
        var candidates = new List<object>();

        foreach (var layer in layers) {
            if (new Hitbox(layer.Position, layer.Size).CollidesWith(new Vector2(x, y))) {
                candidates.Add(layer);
            }
        }

        foreach (var collider in colliders) {
            if (collider.Value.CollidesWith(new Vector2(x, y))) {
                candidates.Add(collider);
            }
        }

        if (candidates.Count == 0) {
            selection = null;
            return;
        }

        int i = selection != null ? candidates.LastIndexOf(selection) : -1;
        i--;
        if (i < 0) i = candidates.Count - 1;

        selection = candidates[i];
    }
    private void ChangeAssetsDir() {
        var path = Console.ReadLine();
        if (path == null) return;
        assetsPath = Path.GetFullPath(path);
        Directory.CreateDirectory(assetsPath);
        atlas = new Atlas(2048, Path.Join(assetsPath, "textures"));
        gl.SolidTexShader.Atlas = atlas;
    }
    private void SaveMap() {
        Console.Write("Map name: ");
        var path = Console.ReadLine();
        if (path == null) return;
        path = Path.Join(assetsPath, "maps", path);

        var map = new Map();

        map.Spawn = spawn;

        foreach (var el in layers) {
            map.Layers.Add(el);
        }
        foreach (var el in colliders) {
            map.Colliders.Add(el.Value);
        }

        try {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var f = new FileStream(path, FileMode.OpenOrCreate);
            Map.Save(map, f);
        }
        catch (Exception) {
            Console.WriteLine("Unable to open file.");
        }
    }
    private void LoadMap() {
        Console.Write("Map name: ");
        var path = Console.ReadLine();
        if (path == null) return;
        path = Path.Join(assetsPath, "maps", path);

        try {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var f = new FileStream(path, FileMode.Open);
            var map = Map.Load(f);

            spawn = map.Spawn;
            layers = map.Layers.ToList();
            colliders = map.Colliders.Select(v => new ModifiableHitbox(v)).ToList();
        }
        catch (Exception) {
            Console.WriteLine("Unable to open file.");
        }
    }
    private void AddLayer() {
        Console.Write("Image name: ");
        string path = Console.ReadLine() ?? "";
        if (path == null) return;

        var tex = atlas[path];

        if (tex.Hitbox.Size.Length < 0.00001f) {
            Console.WriteLine("Texture doesn't exist.");
        }
        else {
            layers.Add(new(path, Round(new(x, y)), tex.Hitbox.Size * atlas.Size / 16f));
        }
    }
    private void AddHitbox() {
        colliders.Add(new(new(Round(new(x, y)), new(1, 1))));
    }

    private void RenderHorizontalLine(Vector2 pos, float width, Vector4? color = null, float thickness = 1) {
        RenderHB(new(pos, new(width, thickness)), color);
    }
    private void RenderOutline(Hitbox hb, Vector4? color = null, float thickness = 1) {
        RenderHB(new(new(hb.Pos1.X - thickness, hb.Pos1.Y - thickness), new(thickness, hb.Size.Y + thickness)), color);
        RenderHB(new(new(hb.Pos2.X, hb.Pos1.Y), new(thickness, hb.Size.Y + thickness)), color);
        RenderHB(new(new(hb.Pos1.X, hb.Pos1.Y - thickness), new(hb.Size.X + thickness, thickness)), color);
        RenderHB(new(new(hb.Pos1.X - thickness, hb.Pos2.Y), new(hb.Size.X + thickness, thickness)), color);
        //RenderHB(new(new(hb.Pos1.X, hb.Pos2.Y - thickness), new(hb.Size.X, thickness)), color);
    }
    private void RenderHB(Hitbox hb, Vector4? color = null) {
        color ??= new(1, 0, 0, 0.5f);
        gl.SolidShader.TransformMatrix = Matrix4.CreateScale(hb.Size.X, hb.Size.Y, 1) * Matrix4.CreateTranslation(new(hb.Pos));

        hitbox.Data(new SolidVertex[] {
            new (new(0, 0), color.Value),
            new (new(1, 0), color.Value),
            new (new(0, 1), color.Value),
            new (new(1, 1), color.Value),
        }, new[] {
            0, 1, 2,
            1, 2, 3,
        });
        hitbox.Draw(Primitive.Triangles);
    }

    private void RenderCrosshair() {
        gl.SolidTexShader.ViewMatrix = Matrix4.CreateOrthographicOffCenter(0, 1440, 900, 0, -1, 1);

        gl.SolidTexShader.Texture = "crosshair";
        gl.SolidTexShader.TransformMatrix =
            Matrix4.CreateTranslation(-.5f, -.5f, 0) *
            Matrix4.CreateScale(10, 10, 1) *
            Matrix4.CreateTranslation(1440 / 2, 900 / 2, 0);
        Layer.mesh!.Draw();
    }
    private void RenderLayers() {
        foreach (var layer in layers) {
            layer.Render();
        }
    }
    private void RenderLayerOutlines() {
        foreach (var el in layers) {
            if (selection == el) {
                RenderOutline(new(el.Position, el.Size), new(1, 0, 1, 1), 1 / 32f);
                RenderHorizontalLine(el.Position + new Vector2(0, el.YOffset), el.Size.X, new(1, 0, 1, 1), 1 / 32f);
            }
            else {
                RenderOutline(new(el.Position, el.Size), new(1, 0, 0, 1), 1 / 32f);
                RenderHorizontalLine(el.Position + new Vector2(0, el.YOffset), el.Size.X, new(1, 0, 0, 1), 1 / 32f);
            }
        }
    }
    private void RenderColliders() {
        foreach (var el in colliders) {
            if (selection == el) {
                RenderOutline(el.Value, new(0, 1, 1, 1), 1 / 32f);
            }
            else {
                RenderOutline(el.Value, new(0, 1, 0, 1), 1 / 32f);
            }
        }
    }
    private void RenderSpawn() {
        RenderHorizontalLine(spawn, 1 / 16f, new Vector4(1, .5f, 0, 1), 1 / 16f);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e) {
        if (e.IsRepeat)
            return;

        if (e.Key == Keys.Equal) {
            scale *= 2;
        }
        if (e.Key == Keys.Minus) {
            scale /= 2;
        }
        if (e.Key == Keys.D1) {
            AddLayer();
        }
        if (e.Key == Keys.D2) {
            AddHitbox();
        }
        if (e.Key == Keys.B) {
            spawn = Round(new(x, y));
        }
        if (e.Key == Keys.Space) {
            ChangeSelecection();
            if (selection != null) {
            }
        }
        if (e.Key == Keys.D9) {
            SaveMap();
        }
        if (e.Key == Keys.D0) {
            LoadMap();
        }
        if (e.Key == Keys.F1) {
            ChangeAssetsDir();
        }
        if (action == null) {
            if (selection is Layer layer) {
                if (e.Key == Keys.M) {
                    action = new MoveAction(new Vector2(x, y), layer);
                    activatorKey = e.Key;
                }
                else if (e.Key == Keys.O) {
                    action = new OffsetAction(y, layer);
                    activatorKey = e.Key;
                }
                else if (e.Key == Keys.Delete) {
                    layers.Remove(layer);
                }
            }
            else if (selection is ModifiableHitbox hb) {
                if (e.Key == Keys.M) {
                    action = new MoveAction(new Vector2(x, y), hb);
                    activatorKey = e.Key;
                }
                else if (e.Key == Keys.N) {
                    action = new ResizeAction(new Vector2(x, y), hb);
                    activatorKey = e.Key;
                }
                else if (e.Key == Keys.Delete) {
                    colliders.Remove(hb);
                }

            }
        }

        base.OnKeyDown(e);
    }
    protected override void OnKeyUp(KeyboardKeyEventArgs e) {
        if (e.Key == activatorKey) action = null;
        base.OnKeyUp(e);
    }
    protected override void OnUpdateFrame(FrameEventArgs args) {
        var delta = (float)args.Time;
        float speed = MathF.Pow(2, this.speed);
        
        if (IsKeyDown(Keys.W)) y -= speed * delta;
        if (IsKeyDown(Keys.S)) y += speed * delta;
        if (IsKeyDown(Keys.A)) x -= speed * delta;
        if (IsKeyDown(Keys.D)) x += speed * delta;

        action?.Update(x, y);

        base.OnUpdateFrame(args);
    }
    protected override void OnRenderFrame(FrameEventArgs args) {
        gl.NewFrame(Size);
        var view = Matrix4.CreateTranslation(1440 / 128f / scale - x, 900 / 128f / scale -y, 0) * Matrix4.CreateOrthographicOffCenter(0, 1440 / 64f / scale, 900 / 64f / scale, 0, -1, 1);
        gl.SolidTexShader.ViewMatrix = view;
        gl.SolidShader.ViewMatrix = view;

        layers.Sort();

        RenderLayers();
        RenderLayerOutlines();
        RenderColliders();
        RenderSpawn();
        RenderCrosshair();

        base.OnRenderFrame(args);
        SwapBuffers();
    }
    protected override void OnMouseWheel(MouseWheelEventArgs e) {
        speed += e.OffsetY / 5f;
        base.OnMouseWheel(e);
    }

    public Program() : base(new(), new()) {
        Size = new(1440, 900);

        gl = new() {
            BackgroundColor = new(1),
            BackfaceCulling = false,
            DepthTest = false,
            Blending = true,
        };

        atlas.LoadToGL(gl);
        atlas.Texture!.Interpolation = Interpolation.Nearest;
        gl.SolidTexShader.Atlas = atlas;

        hitbox = gl.Mesh(gl.SolidShader);

        Layer.Init(gl);
        base.OnLoad();
    }

    public static void Main() {
        new Program().Run();
    }
}