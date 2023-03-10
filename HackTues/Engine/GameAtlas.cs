using HackTues.OpenGL;

namespace HackTues.Engine;

public class GameAtlas: IAtlas {
    public IAtlas Atlas { get; }

    public int Size => Atlas.Size;

    private int levelI;
    public TextureLocation this[string name] {
        get {
            if (name.StartsWith("entry-") && int.TryParse(name[6..], out var levelI)) {
                return GetLevel(levelI) switch {
                    LevelState.Disabled => Atlas["monitor-off"],
                    LevelState.First => Atlas["monitor-glitched"],
                    _ => Atlas["monitor-on"],
                };
            }
            if (name.StartsWith("entry-spawn")) return Atlas["monitor-on"];
            return Atlas[name];
        }
    }

    public LevelState GetLevel(int i) {
        if (i == levelI) return LevelState.First;
        else if (i < levelI) return LevelState.Enabled;
        else return LevelState.Disabled;
    }

    public void Save(FileStream s) {
        var w = new BinaryWriter(s);
        w.Write(levelI);
    }
    public void Load(FileStream s) {
        var r = new BinaryReader(s);
        levelI = r.ReadInt32();
    }
    public void Use(int i) {
        Atlas.Use(i);
    }

    public GameAtlas(Atlas atlas, FileStream save) {
        Atlas = atlas;
        Load(save);
    }
    public GameAtlas(IAtlas atlas, int levelI) {
        Atlas = atlas;
        this.levelI = levelI;
    }
}
