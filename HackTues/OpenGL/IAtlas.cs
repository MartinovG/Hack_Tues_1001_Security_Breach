using OpenTK.Mathematics;

namespace HackTues.OpenGL;

public interface IAtlas {
    public int Size { get; }
    public TextureLocation this[string name] { get; }
    public void Use(int slot);
}
