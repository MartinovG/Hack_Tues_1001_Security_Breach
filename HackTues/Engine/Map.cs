using HackTues.Engine;
using OpenTK.Mathematics;

namespace HackTues.Engine;

public class Map: ICollider, ILayerOwner {
    public SortedSet<Layer> Layers { get; } = new();
    public IEnumerable<Layer> EntryLayers => Layers.Where(v => v.Texture.StartsWith("entry-"));
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
        w.Write((int)MathF.Floor(layer.Position.X));
        w.Write((int)MathF.Floor(layer.Position.Y));
        w.Write((int)MathF.Floor(layer.Size.X));
        w.Write((int)MathF.Floor(layer.Size.Y));
        w.Write((int)MathF.Floor(layer.Origin.X));
        w.Write((int)MathF.Floor(layer.Origin.Y));
        w.Write(layer.Texture);
    }
    private static Layer LoadLayer(BinaryReader r) {
        var pos = new Vector2(
            r.ReadInt32(),
            r.ReadInt32()
        );
        var size = new Vector2(
            r.ReadInt32(),
            r.ReadInt32()
        );
        var origin = new Vector2(
            r.ReadInt32(),
            r.ReadInt32()
        );
        var texture = r.ReadString();

        return new(texture, pos, size, origin);
    }

    public static void Save(Map map, Stream stream) {
        var w = new BinaryWriter(stream);

        w.Write((int)MathF.Floor(map.Spawn.X));
        w.Write((int)MathF.Floor(map.Spawn.Y));
        w.Write(map.Layers.Count);
        foreach (var bg in map.Layers) {
            SaveLayer(w, bg);
        }

        w.Write(map.Colliders.Count);
        foreach (var hb in map.Colliders) {
            w.Write((int)MathF.Floor(hb.Pos.X));
            w.Write((int)MathF.Floor(hb.Pos.Y));
            w.Write((int)MathF.Floor(hb.Size.X));
            w.Write((int)MathF.Floor(hb.Size.Y));
        }
    }
    public static Map Load(Stream stream) {
        var r = new BinaryReader(stream);
        var res = new Map();

        res.Spawn = new Vector2(
            r.ReadInt32(),
            r.ReadInt32()
        );
        var n = r.ReadInt32();
        for (int i = 0; i < n; i++) {
            res.Layers.Add(LoadLayer(r));
        }

        n = r.ReadInt32();
        for (int i = 0; i < n; i++) {
            var x = r.ReadInt32();
            var y = r.ReadInt32();
            var w = r.ReadInt32();
            var h = r.ReadInt32();
            res.Colliders.Add(new(new(x, y), new(w, h)));
        }

        return res;
    }
}
