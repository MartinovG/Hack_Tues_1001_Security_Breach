using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HackTues.Editor;

public interface IAction {
    void Update(float x, float y);
}
