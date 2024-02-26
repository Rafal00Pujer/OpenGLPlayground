using OpenGLPlayground.Components;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace OpenGLPlayground.Systems;

internal unsafe class RenderSystem
{
    private readonly GL _gl;
    private readonly Glfw _glfw;
    private readonly int _modelLocation;
    private readonly WindowHandle* _windowHandle;

    public RenderSystem(GL gl, uint shader, Glfw glfw, WindowHandle* windowHandle)
    {
        _gl = gl;
        _glfw = glfw;
        _windowHandle = windowHandle;

        _modelLocation = _gl.GetUniformLocation(shader, "model");
    }

    public void Update(Dictionary<uint, TransformComponent> transformComponents, Dictionary<uint, RenderComponent> renderComponents)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        foreach (var (key, renderComponent) in renderComponents)
        {
            var transform = transformComponents[key];

            var translation = Matrix4X4.CreateTranslation(transform.Position);
            var rotation = Matrix4X4.CreateRotationZ(Scalar.DegreesToRadians(transform.Eulers.Z));
            var model = rotation * translation;

            _gl.UniformMatrix4(_modelLocation, true, model.AsSpan());

            _gl.BindTexture(GLEnum.Texture2D, renderComponent.Material);
            _gl.BindVertexArray(renderComponent.Vao);
            _gl.DrawArrays(GLEnum.Triangles, 0, renderComponent.VertexCount);
        }

        _glfw.SwapBuffers(_windowHandle);
    }
}
