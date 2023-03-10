using HackTues.Controls;
using OpenTK.Mathematics;
using System;
using System.IO;

namespace HackTues.Engine;

public interface ILevelLoader {
    void LoadMap(string name);
}
public interface ILobbyLoader {
    void LoadLobby();
}

public abstract class Game: IGame {
    public static string AssetsPath {
        get {
            if (File.Exists(".assets")) {
                var r = new StreamReader(".assets");
                var path = r.ReadToEnd().Trim();
                r.Close();
                if (path != "") return Path.GetFullPath(path);
            }

            var w = new StreamWriter(".assets");
            w.WriteLine("assets");
            w.Close();

            return Path.GetFullPath("assets");
        }
    }
    public Map Map { get; protected set; } = new();
    protected abstract Vector2 Friction { get; }
    public abstract Vector2 CameraPos { get; }
    public abstract Layer PlayerLayer { get; }
    public abstract Hitbox PlayerHitbox { get; }
    public Vector2 PlayerPosition {
        get => pos;
        set => pos = value;
    }
    public bool Switch { get; protected set; }

    protected Vector2 pos, vel;

    protected abstract Vector2 GetAcceleration(float detla, IController? controller);
    protected virtual void HitX() { }
    protected virtual void HitY() {  }

    public void AddLayers(SortedSet<Layer> layers) {
        Map.AddLayers(layers);
        layers.Add(PlayerLayer + PlayerPosition);
    }

    public void Render() {
        var set = new SortedSet<Layer>();
        Map.AddLayers(set);
        set.Add(PlayerLayer);

        foreach (var fg in set) {
            fg.Render();
        }
    }
    public virtual void Update(float delta, IController? controller) {
        var acc = GetAcceleration(delta, controller);

        vel += acc * delta;
        vel *= new Vector2(MathF.Pow(Friction.X, delta), MathF.Pow(Friction.Y, delta));

        pos.X += vel.X * delta;
        if (Map.CollidesWith(PlayerHitbox + pos)) {
            pos.X -= vel.X * delta;
            vel.X = 0;
            HitX();
        }

        pos.Y += vel.Y * delta;
        if (Map.CollidesWith(PlayerHitbox + pos)) {
            pos.Y -= vel.Y * delta;
            vel.Y = 0;
            HitY();
        }
    }
}

public class LobbyGame: Game {
    protected override Vector2 Friction { get; } = new(0.000001f, 0.000001f);
    public override Layer PlayerLayer { get; } = new("player", new(0, 0), new(32, 64), new(16, 64));
    public override Hitbox PlayerHitbox { get; } = new(new(-16, -8), new(32, 8));
    public override Vector2 CameraPos => PlayerPosition;
    public ILevelLoader LevelLoader { get; }

    protected override Vector2 GetAcceleration(float detla, IController? controller) {
        var acc = Vector2.Zero;
        if (controller == null)
            return acc;

        if (controller.Get(Button.Left)) {
            acc -= Vector2.UnitX;
        }
        if (controller.Get(Button.Right)) {
            acc += Vector2.UnitX;
        }
        if (controller.Get(Button.Up)) {
            acc -= Vector2.UnitY;
        }
        if (controller.Get(Button.Down)) {
            acc += Vector2.UnitY;
        }

        if (acc.Length > 1)
            acc.Normalize();
        acc *= 1000;

        return acc;
    }

    public override void Update(float delta, IController? controller) {
        base.Update(delta, controller);

        if (controller?.Poll(Button.Shoot) == true) {
            foreach (var entry in Map.EntryLayers) {
                if ((pos - entry.Position).Length < 16) {
                    LevelLoader.LoadMap(entry.Texture[6..]);
                }
            }
        }
    }

    public LobbyGame(ILevelLoader lobbyLoader) {
        LevelLoader = lobbyLoader;
        Map = Map.Load(new FileStream(Path.Combine(AssetsPath, "maps/spawn"), FileMode.Open));
        PlayerPosition = Map.Spawn;
    }
}
public class LevelGame: Game {
    protected override Vector2 Friction { get; } = new(0.000001f, 0.1f);
    public override Vector2 CameraPos => new(Math.Max(1440 / 4, pos.X), 900 / 4);
    public override Layer PlayerLayer { get; } = new("player", new(0, 0), new(32, 64), new(16, 64));
    public override Hitbox PlayerHitbox { get; } = new(new(-16, -64), new(32, 64));
    public ILevelLoader LevelLoader { get; }

    public void LoadMap(string name) {
        Map = Map.Load(new FileStream(Path.Combine(AssetsPath, "maps/" + name), FileMode.Open));
    }

    private float kyoteeTime = 0;
    private float jumpCooldown = 0;

    protected override void HitY() {
        kyoteeTime = .1f;
    }
    protected override Vector2 GetAcceleration(float detla, IController? controller) {
        var acc = Vector2.Zero;
        if (controller == null)
            return acc;
        if (controller.Get(Button.Left)) {
            acc -= Vector2.UnitX * 1250;
        }
        if (controller.Get(Button.Right)) {
            acc += Vector2.UnitX * 1250;
        }
        if (controller.Get(Button.Up) && kyoteeTime > 0 && jumpCooldown < 0 && controller.Poll(Button.Up)) {
            vel.Y = -400;
            kyoteeTime = 0;
            jumpCooldown = .5f;
        }

        acc += Vector2.UnitY * 700;

        return acc;
    }

    public override void Update(float delta, IController? controller) {
        base.Update(delta, controller);

        jumpCooldown -= delta;
        kyoteeTime -= delta;
    }

    public LevelGame(ILevelLoader levelLoader) {
        LevelLoader = levelLoader;
    }
}
