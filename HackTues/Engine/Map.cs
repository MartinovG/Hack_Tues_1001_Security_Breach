using HackTues.Engine;
using OpenTK.Mathematics;

namespace HackTues.Engine;

public class Map: ICollider, ILayerOwner {
    public SortedSet<Layer> Layers { get; } = new();
    public List<Hitbox> Colliders { get; } = new();
    public Vector2 Spawn { get; set; }
    
    public bool CollidesWith(Hitbox hitbox) {
        foreach (var collider in Colliders) {
            if (collider.CollidesWith(hitbox)) return true;
        }
        return false;
    }
    public bool CollidesWith(Vector2 pt) {
        foreach (var collider in Colliders) {
            if (collider.CollidesWith(pt))
                return true;
        }
        return false;
    }

    public void AddLayers(SortedSet<Layer> layers) {
        foreach (var el in Layers) layers.Add(el);
    }

    public Map() { }

    private static void SaveLayer(BinaryWriter w, Layer layer) {
        w.Write(MathF.Floor(layer.Position.X * 16));
        w.Write(MathF.Floor(layer.Position.Y * 16));
        w.Write(MathF.Floor(layer.Size.X * 16));
        w.Write(MathF.Floor(layer.Size.Y * 16));
        w.Write(MathF.Floor(layer.YOffset * 16));
        w.Write(layer.Texture);
    }
    private static Layer LoadLayer(BinaryReader r) {
        var pos = new Vector2(
            r.ReadInt32() / 16f,
            r.ReadInt32() / 16f
        );
        var size = new Vector2(
            r.ReadInt32() / 16f,
            r.ReadInt32() / 16f
        );
        var yOffst = r.ReadInt32() / 16f;
        var texture = r.ReadString();

        return new(texture, pos, size, yOffst);
    }

    public static void Save(Map map, Stream stream) {
        var w = new BinaryWriter(stream);

        w.Write(MathF.Floor(map.Spawn.X * 16));
        w.Write(MathF.Floor(map.Spawn.Y * 16));
        w.Write(map.Layers.Count);
        foreach (var bg in map.Layers) {
            SaveLayer(w, bg);
        }

        w.Write(map.Colliders.Count);
        foreach (var hb in map.Colliders) {
            w.Write(MathF.Floor(hb.Pos.X * 16));
            w.Write(MathF.Floor(hb.Pos.Y * 16));
            w.Write(MathF.Floor(hb.Size.X * 16));
            w.Write(MathF.Floor(hb.Size.Y * 16));
        }
    }
    public static Map Load(Stream stream) {
        var r = new BinaryReader(stream);
        var res = new Map();

        res.Spawn = new Vector2(
            r.ReadInt32() / 16f,
            r.ReadInt32() / 16f
        );
        var n = r.ReadInt32();
        for (int i = 0; i < n; i++) {
            res.Layers.Add(LoadLayer(r));
        }

        n = r.ReadInt32();
        for (int i = 0; i < n; i++) {
            var x = r.ReadInt32() / 16f;
            var y = r.ReadInt32() / 16f;
            var w = r.ReadInt32() / 16f;
            var h = r.ReadInt32() / 16f;
            res.Colliders.Add(new(new(x, y), new(w, h)));
        }

        return res;
    }
}
