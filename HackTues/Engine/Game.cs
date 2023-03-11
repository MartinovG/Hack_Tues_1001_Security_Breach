using HackTues.Controls;
using HackTues.Physics;
using OpenTK.Mathematics;

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
    public PhysicsEngine Physics { get; }
    public Map Map { get; protected set; }
    public abstract Vector2 CameraPos { get; }
    public abstract Layer PlayerLayer { get; }
    public abstract Hitbox PlayerHitbox { get; }
    public abstract Vector2 PlayerPosition { get; set; }
    public bool Switch { get; protected set; }


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
    public virtual void Update(float delta, IController controller) {
        Physics.Update(delta, controller);
    }

    public Game() {
        Physics = new();
    }
}

public enum Direction {
    North,
    South,
    West,
    East,
}
public enum TurnDirection {
    Left,
    Right,
}
public enum WeaponDirection {
    Up,
    Middle,
    Down,
}

public class LobbyPlayer: DynamicCollider {
    private float walkCycle = 0;
    private Direction direction = Direction.North;
    private LobbyGame game;

    private Layer[] east = {
        new("player-office-east-0", new(0, 0), new(34, 84), new(17, 75)),
        new("player-office-east-1", new(0, 0), new(34, 81), new(17, 75)),
        new("player-office-east-2", new(0, 0), new(34, 78), new(17, 75)),
    };
    private Layer[] west = {
        new("player-office-west-0", new(0, 0), new(34, 84), new(17, 75)),
        new("player-office-west-1", new(0, 0), new(34, 81), new(17, 75)),
        new("player-office-west-2", new(0, 0), new(34, 78), new(17, 75)),
    };
    private Layer[] north = {
        new("player-office-north-0", new(0, 0), new(45, 83), new(23, 77)),
        new("player-office-north-1", new(0, 0), new(39, 83), new(20, 77)),
        new("player-office-north-2", new(0, 0), new(40, 83), new(20, 77)),
    };
    private Layer[] south = {
        new("player-office-south-0", new(0, 0), new(45, 83), new(23, 77)),
        new("player-office-south-1", new(0, 0), new(39, 83), new(20, 77)),
        new("player-office-south-2", new(0, 0), new(40, 83), new(20, 77)),
    };

    public override bool HasGravity => false;
    public override Vector2 Friction { get; } = new(.000001f, .000001f);
    public override Hitbox Hitbox { get; } = new(new(-12, -5), new(24, 10));
    public Layer Layer {
        get {
            int i = (int)walkCycle % 4;
            if (i == 2)
                i = 0;
            if (i == 3)
                i = 2;

            return direction switch {
                Direction.North => north[i],
                Direction.South => south[i],
                Direction.West => west[i],
                Direction.East => east[i],
                _ => null!,
            };
        }
    }

    public override void Update(float delta, IController controller) {
        var acc = Vector2.Zero;
        if (controller == null) return;

        if (controller.Get(Button.Left)) {
            acc -= Vector2.UnitX;
            direction = Direction.West;
        }
        if (controller.Get(Button.Right)) {
            acc += Vector2.UnitX;
            direction = Direction.East;
        }
        if (controller.Get(Button.Up)) {
            acc -= Vector2.UnitY;
            direction = Direction.North;
        }
        if (controller.Get(Button.Down)) {
            acc += Vector2.UnitY;
            direction = Direction.South;
        }

        if (acc.Length > 1)
            acc.Normalize();
        acc *= 1000;
        Velocity += acc * delta;

        if (controller?.Poll(Button.Shoot) == true) {
            foreach (var entry in game.Map.EntryLayers) {
                if ((Position - entry.Position).Length < 16) {
                    game.LevelLoader.LoadMap(entry.Texture[6..]);
                }
            }
        }
    }

    public LobbyPlayer(LobbyGame game) {
        this.game = game;
    }
}
public class LevelPlayer: DynamicCollider {
    private float walkCycle = 0;
    private float kyoteeTime = 0;
    private float jumpCooldown = 0;
    private TurnDirection turn = TurnDirection.Right;
    private WeaponDirection weapon = WeaponDirection.Middle;
    private LevelGame game;

    private Layer[] left = {
        new("player-map-left-0-0", new(0, 0), new(36, 63), new(19, 61)),
        new("player-map-left-0-1", new(0, 0), new(42, 63), new(22, 61)),
        new("player-map-left-0-2", new(0, 0), new(42, 63), new(21, 61)),
        new("player-map-left-1-0", new(0, 0), new(36, 67), new(19, 61)),
        new("player-map-left-1-1", new(0, 0), new(42, 67), new(22, 61)),
        new("player-map-left-1-2", new(0, 0), new(42, 67), new(21, 61)),
    };
    private Layer[] right = {
        new("player-map-right-0-0", new(0, 0), new(36, 63), new(19, 61)),
        new("player-map-right-0-1", new(0, 0), new(42, 63), new(22, 61)),
        new("player-map-right-0-2", new(0, 0), new(42, 63), new(21, 61)),
        new("player-map-right-1-0", new(0, 0), new(36, 67), new(19, 61)),
        new("player-map-right-1-1", new(0, 0), new(42, 67), new(22, 61)),
        new("player-map-right-1-2", new(0, 0), new(42, 67), new(21, 61)),
    };

    public override bool HasGravity => true;
    public override Vector2 Friction { get; } = new(0.000001f, 0.1f);
    public override Hitbox Hitbox { get; } = new(new(-16, -64), new(32, 64));
    public Layer Layer {
        get {
            int i = (int)walkCycle % 2;

            return turn switch {
                TurnDirection.Right => right[(int)weapon + i * 3],
                TurnDirection.Left => left[(int)weapon + i * 3],
                _ => null!,
            };
        }
    }

    public override void OnCollisionY(ICollider other) {
        kyoteeTime = .4f;
    }
    public override void Update(float delta, IController controller) {
        var acc = Vector2.Zero;

        if (controller.Get(Button.Left)) {
            acc -= Vector2.UnitX * 1250;
            turn = TurnDirection.Left;
        }
        if (controller.Get(Button.Right)) {
            acc += Vector2.UnitX * 1250;
            turn = TurnDirection.Right;
        }
        if (controller.Get(Button.Up) && kyoteeTime > 0 && jumpCooldown < 0 && controller.Poll(Button.Up)) {
            Velocity = new(Velocity.X, -500);
            kyoteeTime = 0;
            jumpCooldown = .5f;
        }

        if (controller.Get(Button.WeaponLeft)) {
            turn = TurnDirection.Left;
            weapon = WeaponDirection.Middle;
        }
        if (controller.Get(Button.WeaponRight)) {
            turn = TurnDirection.Right;
            weapon = WeaponDirection.Middle;
        }
        if (controller.Get(Button.WeaponUp)) {
            weapon = WeaponDirection.Up;
        }
        if (controller.Get(Button.WeaponDown)) {
            weapon = WeaponDirection.Down;
        }

        Velocity += acc * delta;

        if (Math.Abs(Velocity.X) > 1) {
            walkCycle += delta * 4;
        }
        else {
            walkCycle = 0;
        }

        jumpCooldown -= delta;
        kyoteeTime -= delta;
    }

    public LevelPlayer(LevelGame game) {
        this.game = game;
    }
}

public class LobbyGame: Game {
    public LobbyPlayer Player { get; }
    public override Vector2 PlayerPosition {
        get => Player.Position;
        set => Player.Position = value;
    }
    public override Layer PlayerLayer => Player.Layer;
    public override Hitbox PlayerHitbox => Player.Hitbox;
    public override Vector2 CameraPos => Player.Position;
    public ILevelLoader LevelLoader { get; }

    public override void Update(float delta, IController controller) {
        base.Update(delta, controller);
    }

    public LobbyGame(ILevelLoader lobbyLoader) {
        LevelLoader = lobbyLoader;
        Map = Map.Load(new FileStream(Path.Combine(AssetsPath, "maps/spawn"), FileMode.Open));
        Player = new(this);
        PlayerPosition = Map.Spawn;
        Physics.DynamicColliders.Add(Player);
        Physics.StaticColliders.Add(Map);
    }
}
public class LevelGame: Game {
    public LevelPlayer Player { get; }
    public override Vector2 CameraPos => new(Math.Max(1440 / 4, PlayerPosition.X), 900 / 4);
    public override Vector2 PlayerPosition {
        get => Player.Position;
        set => Player.Position = value;
    }
    public override Layer PlayerLayer => Player.Layer;
    public override Hitbox PlayerHitbox => Player.Hitbox;
    public ILevelLoader LevelLoader { get; }

    public void LoadMap(string name) {
        Map = Map.Load(new FileStream(Path.Combine(AssetsPath, "maps/" + name), FileMode.Open));
        Physics.DynamicColliders.Clear();
        Physics.StaticColliders.Clear();
        Physics.DynamicColliders.Add(Player);
        Physics.StaticColliders.Add(Map);
    }

    public override void Update(float delta, IController controller) {
        base.Update(delta, controller);
    }

    public LevelGame(ILevelLoader levelLoader) {
        LevelLoader = levelLoader;
        Player = new(this);
    }
}
