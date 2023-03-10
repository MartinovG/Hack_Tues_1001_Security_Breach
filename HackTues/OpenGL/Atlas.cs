using HackTues.Engine;
using HackTues.OpenGL.Textures;
using SkiaSharp;

namespace HackTues.OpenGL;

public record struct TextureLocation(int PageIndex, Hitbox Hitbox);

public class Atlas
{
    private List<AtlasPage> pages = new();
    public int Size { get; }
    public GLTextureArray? Texture { get; private set; }

    public TextureLocation this[string name]
    {
        get
        {
            for (var i = 0; i < pages.Count; i++)
            {
                if (pages[i].Bitmaps.TryGetValue(name, out var res))
                    return new(i, res);
            }

            return default;
        }
    }

    public void LoadToGL(GLRenderer renderer)
    {
        Texture = renderer.TextureArray(pages.Select(v => v.Bitmap).ToArray());
    }

    public Atlas(int size, IEnumerable<KeyValuePair<string, SKBitmap>> resources)
    {
        Size = size = 1 << (int)Math.Ceiling(Math.Log2(size));
        pages.Add(new(size));

        var entries = resources.ToList();
        entries.Sort((a, b) => -(a.Value.Width * a.Value.Height).CompareTo(b.Value.Width * b.Value.Height));

        foreach (var res in entries)
        {
            if (res.Value.Width > size || res.Value.Height > size)
            {
                pages[0].PutEmpty(res.Key);
                Console.WriteLine("WARNING: Texture {0} exceeds the atlas size of {1}x{1}.", res.Key, size);
            }
            else
            {
                bool fitted = false;
                foreach (var page in pages)
                {
                    if (page.TryFit(res.Key, res.Value))
                    {
                        fitted = true;
                        break;
                    }
                }

                if (!fitted)
                {
                    var page = new AtlasPage(size);
                    page.TryFit(res.Key, res.Value);
                    pages.Add(page);
                }
            }
        }
    }

    public Atlas(int size, string path) : this(size, Directory
        .GetFiles(path, "*", SearchOption.AllDirectories)
        .Where(v => v.EndsWith(".png"))
        .Select(v => {
            var name = v[(path.Length + 1)..^4];
            var bmp = SKBitmap.Decode(new FileStream(v, FileMode.Open));
            return new KeyValuePair<string, SKBitmap>(name, bmp);
        })
    ) { }
}
