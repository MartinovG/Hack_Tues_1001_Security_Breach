using HackTues.OpenGL.Shaders;
using HackTues.OpenGL.Textures;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SkiaSharp;

namespace HackTues.OpenGL;

public class GLRenderer
{
    private bool depth, depthWrite, blending, backface;
    private Vector4 clearColor;

    public bool DepthWrite
    {
        get => depthWrite;
        set => GL.DepthMask(depthWrite = value);
    }
    public bool DepthTest
    {
        get => depth;
        set
        {
            if (depth = value)
                GL.Enable(EnableCap.DepthTest);
            else
                GL.Disable(EnableCap.DepthTest);
        }
    }
    public bool Blending
    {
        get => blending;
        set
        {
            if (blending = value)
                GL.Enable(EnableCap.Blend);
            else
                GL.Disable(EnableCap.Blend);
        }
    }
    public bool BackfaceCulling
    {
        get => backface;
        set
        {
            if (backface = value)
                GL.Enable(EnableCap.CullFace);
            else
                GL.Disable(EnableCap.CullFace);
        }
    }
    public Vector4 BackgroundColor
    {
        get => clearColor;
        set
        {
            clearColor = value;
            GL.ClearColor(value.X, value.Y, value.Z, value.W);
        }
    }

    public GLSolidShader SolidShader { get; } = new();
    public GLSolidTexShader SolidTexShader { get; } = new();

    public GLMesh<T> Mesh<T>(GLShader<T> shader)
    {
        if (shader is GLShader<T> s)
        {
            return new(s);
        }
        else throw new ArgumentException("Shader must be GLShader", nameof(shader));
    }

    public GLTexture Texture() => new();
    public GLTexture Texture(SKBitmap bmp) {
        var tex = Texture();
        var pixels = bmp.Pixels;
        var data = new Vector4[pixels.Length];

        for (int i = 0; i < pixels.Length; i++) {
            data[i] = new(
                pixels[i].Red / 255f,
                pixels[i].Green / 255f,
                pixels[i].Blue / 255f,
                pixels[i].Alpha / 255f
           );
        }

        tex.Data(bmp.Width, bmp.Height, data);
        return tex;
    }
    public GLTexture Texture(FileStream file) {
        using var bmp = SKBitmap.Decode(file);
        return Texture(bmp);
    }

    public GLTextureArray TextureArray() => new();
    public GLTextureArray TextureArray(SKBitmap[] bmps) {
        var tex = TextureArray();

        if (bmps.Length == 0) {
            tex.Data(0, 0, 0, new Vector4[0]);
            return tex;
        }

        int w = bmps[0].Width;
        int h = bmps[0].Height;

        var data = new Vector4[w * h * bmps.Length];

        for (int i = 0; i < bmps.Length; i++) {
            var bmp = bmps[i];
            var pixels = bmp.Pixels;
            if (bmp.Width != w || bmp.Height != h) {
                throw new ArgumentException("Expected all bitmaps to be in the same size.", nameof(bmps));
            }
            for (int j = 0; j < pixels.Length; j++) {
                data[i * w * h + j] = new(
                    pixels[j].Red / 255f,
                    pixels[j].Green / 255f,
                    pixels[j].Blue / 255f,
                    pixels[j].Alpha / 255f
               );
            }
        }

        tex.Data(w, h, bmps.Length, data);
        return tex;
    }

    public void NewFrame(Vector2i windowSize)
    {
        GL.Viewport(0, 0, windowSize.X, windowSize.Y);
        GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
    }
    public void ClearDepth() => GL.Clear(ClearBufferMask.DepthBufferBit);

    public GLRenderer()
    {
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        DepthWrite = true;
        DepthTest = false;
        Blending = false;
        BackfaceCulling = true;
    }
}
