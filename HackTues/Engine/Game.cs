using HackTues.Controls;
using OpenTK.Mathematics;
using System;

namespace HackTues.Engine;

public interface IProjectileShooter {
    void Shoot(Projectile projectile);
}

public abstract class Projectile: ILayerOwner {
    public abstract Layer Layer { get; }
    public float Direction { get; }
    public float Speed { get; }
    public Vector3 Position { get; }

    protected abstract void OnCollide(SideViewPlayer player);

    public void Update(float delta, ICollider environment, SideViewPlayer player) {

    }
    public void AddLayers(SortedSet<Layer> layers) {
        layers.Add(Layer);
    }
}

public abstract class AbstractGame: IGame {
    public Map Map { get; private set; } = new();
    public List<Projectile> Projectiles { get; } = new();
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
        layers.Add(PlayerLayer + PlayerPosition);
    }

    public void LoadMap(string name) {
        Projectiles.Clear();
        var f = new FileStream(Path.Join(Game.AssetsPath(Environment.CurrentDirectory), "maps/" + name), FileMode.Open);
        Map = Map.Load(f);
        Player!.Position = Map.Spawn;
        f.Close();
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

public class LobbyGame: AbstractGame {
    public override Vector2 CameraPos => PlayerPosition;
    public override Layer PlayerLayer { get; } = new("player", new(0, 0), new(32, 64), new(16, 64));
    public override Hitbox PlayerHitbox { get; } = new(new(-16, -8), new(32, 8));
    protected override Vector2 Friction { get; } = new(0.000001f, 0.000001f);

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
                    LoadMap(entry.Texture[6..]);
                }
            }
        }
    }
}
public class LevelGame: AbstractGame {
    public override Vector2 CameraPos => PlayerPosition;
    public override Layer PlayerLayer { get; } = new("player", new(0, 0), new(32, 64), new(16, 64));
    public override Hitbox PlayerHitbox { get; } = new(new(-16, -64), new(32, 64));
    protected override Vector2 Friction { get; } = new(0.000001f, 0.1f);

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
}

public class Game: IProjectileShooter {
    public static string AssetsPath(string root) {
        if (File.Exists(Path.Join(root, ".assets"))) {
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

    public IGame Player { get; set; }
    public SideViewPlayer SidePlayer { get; set; }
    public TopViewPlayer TopPlayer { get; set; }
    public Map Map { get; set; } = new();
    public List<Projectile> Projectiles { get; } = new();

    public void Shoot(Projectile projectile) {
        this.Projectiles.Add(projectile);
    }

    public void Render() {
        var set = new SortedSet<Layer>();
        Map.AddLayers(set);
        Player?.AddLayers(set);

        foreach (var fg in set) {
            fg.Render();
        }
    }

    public void LoadMap(string name) {
        Projectiles.Clear();
        var f = new FileStream(Path.Join(AssetsPath(Environment.CurrentDirectory), "maps/" + name), FileMode.Open);
        Map = Map.Load(f);
        if (name == "spawn") {
            Player = TopPlayer;
        }
        else {
            Player = SidePlayer;
        }
        Player!.Position = Map.Spawn;
        f.Close();
    }

    public void Update(IController? controller, float delta) {
        Player.Update(delta, Map, controller, this);

        if (controller?.Poll(Button.Shoot) == true) {
            foreach (var entry in Map.EntryLayers) {
                if ((Player.Position - entry.Position).Length < 16) {
                    LoadMap(entry.Texture[6..]);
                }
            }
        }
    }

    public Game(SideViewPlayer sidePlayer, TopViewPlayer topPlayer) {
        SidePlayer = sidePlayer;
        TopPlayer = topPlayer;
        LoadMap("spawn");
    }
}
