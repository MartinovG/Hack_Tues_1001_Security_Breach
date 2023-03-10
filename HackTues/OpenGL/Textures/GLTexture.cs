using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace HackTues.OpenGL.Textures;

public class GLTexture {
    internal int id;
    private Interpolation interpolation;
    private bool disposedValue;


    protected virtual void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                GL.DeleteTexture(id);
            }

            disposedValue = true;
        }
    }

    public Interpolation Interpolation {
        get => interpolation;
        set {
            int val = (interpolation = value) switch {
                Interpolation.Nearest => (int)TextureMagFilter.Nearest,
                _ => (int)TextureMagFilter.Linear
            };
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, val);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, val);
        }
    }

    public void Bind(int slot) {
        GL.BindTexture(TextureTarget.Texture2D, id);
        GL.ActiveTexture(TextureUnit.Texture0 + slot);
    }
    public void Data(int width, int height, Vector4[] data) {
        GL.BindTexture(TextureTarget.Texture2D, id);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.Float, data);
    }
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public GLTexture() {
        Interpolation = Interpolation.Linear;
        GL.CreateTextures(TextureTarget.Texture2D, 1, out id);
    }
}
