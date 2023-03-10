using HackTues.Controls;

namespace HackTues.Engine;

public class Game {
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

    public IPlayer Player { get; set; }
    public SideViewPlayer SidePlayer { get; set; }
    public TopViewPlayer TopPlayer { get; set; }
    public Map Map { get; set; } = new();

    public void Render(float delta) {
        var set = new SortedSet<Layer>();
        Map.AddLayers(set);
        Player?.AddLayers(set);

        foreach (var fg in set) {
            fg.Render();
        }
    }

    public void LoadMap(string name) {
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
        Player.Update(delta, Map, controller);

        if (controller?.Poll(Button.Shoot) == true) {
            foreach (var entry in Map.EntryLayers) {
                if ((Player.Position - entry.Position).Length < 1) {
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
