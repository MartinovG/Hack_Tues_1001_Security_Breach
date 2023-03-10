using OpenTK.Mathematics;

namespace HackTues.OpenGL;

public interface IMatricesOwner {
    Matrix4 TransformMatrix { get; set; }
    Matrix4 ViewMatrix { get; set; }
}
