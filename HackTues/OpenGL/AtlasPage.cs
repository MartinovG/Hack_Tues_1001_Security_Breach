using HackTues.Engine;
using SkiaSharp;

namespace HackTues.OpenGL;

class AtlasPage
{
    public SKBitmap Bitmap { get; }
    public Dictionary<string, Hitbox> Bitmaps { get; } = new();

    private SKCanvas canvas;

    private bool TryFit(Hitbox hb, string name, SKBitmap bmp)
    {
        if (hb.Pos1.X < 0 || hb.Pos1.Y < 0 || hb.Pos2.X > 1 || hb.Pos2.Y > 1)
        {
            return false;
        }

        foreach (var el in Bitmaps.Values)
        {
            if (hb.CollidesWith(el))
            {
                return false;
            }
        }

        canvas.DrawImage(SKImage.FromBitmap(bmp), hb.Pos1.X * Bitmap.Width, hb.Pos1.Y * Bitmap.Height, null);
        Bitmaps[name] = hb;
        return true;
    }
    public bool TryFit(string name, SKBitmap bmp)
    {
        float w = bmp.Width / (float)Bitmap.Width;
        float h = bmp.Height / (float)Bitmap.Height;

        foreach (var img in Bitmaps)
        {
            var hb1 = new Hitbox(new(img.Value.Pos1.X, img.Value.Pos2.Y), new(w, h));
            var hb2 = new Hitbox(new(img.Value.Pos2.X, img.Value.Pos1.Y), new(w, h));

            if (TryFit(hb1, name, bmp))
                return true;
            if (TryFit(hb2, name, bmp))
                return true;
        }

        var hb = new Hitbox(new(0), new(w, h));
        return TryFit(hb, name, bmp);
    }
    public void PutEmpty(string name)
    {
        Bitmaps[name] = new(new(0, 0), new(0, 0));
    }

    public AtlasPage(int size)
    {
        Bitmap = new SKBitmap(size, size);
        canvas = new SKCanvas(Bitmap);
    }
}
