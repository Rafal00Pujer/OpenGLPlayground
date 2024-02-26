using OpenGLPlayground.Components;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace OpenGLPlayground.Systems;

internal unsafe class CameraSystem
{
    private readonly GL _gl;
    private readonly Glfw _glfw;
    private readonly int _viewLocation;
    private readonly Vector3D<float> _globalUp = new(0.0f, 0.0f, 1.0f);
    private readonly WindowHandle* _windowHandle;

    public CameraSystem(GL gl, uint shader, Glfw glfw, WindowHandle* windowHandle)
    {
        _gl = gl;
        _glfw = glfw;
        _windowHandle = windowHandle;

        _gl.UseProgram(shader);
        _viewLocation = _gl.GetUniformLocation(shader, "view");
    }

    public bool Update(Dictionary<uint, TransformComponent> transformComponents, uint cameraId, ref CameraComponent cameraComponent, float deltaTime)
    {
        var pos = transformComponents[cameraId].Position;
        var eulers = transformComponents[cameraId].Eulers;

        var theta = Scalar.DegreesToRadians(eulers.Z);
        var phi = Scalar.DegreesToRadians(eulers.Y);

        var forwards = cameraComponent.Forward;

        forwards.X = Scalar.Cos(theta) * Scalar.Cos(phi);
        forwards.Y = Scalar.Sin(theta) * Scalar.Cos(phi);
        forwards.Z = Scalar.Sin(phi);

        cameraComponent.Forward = forwards;

        var right = Vector3D.Normalize(Vector3D.Cross(forwards, _globalUp));
        cameraComponent.Right = right;

        var up = Vector3D.Normalize(Vector3D.Cross(right, forwards));
        cameraComponent.Up = up;

        var view = Matrix4X4.CreateLookAt(pos, pos + forwards, up);

        _gl.UniformMatrix4(_viewLocation, 1, true, view.AsSpan());

        //Keys
        var dPos = Vector3D<float>.Zero;

        if (_glfw.GetKey(_windowHandle, Keys.W) == (int)InputAction.Press)
        {
            dPos.X += 1.0f;
        }

        if (_glfw.GetKey(_windowHandle, Keys.A) == (int)InputAction.Press)
        {
            dPos.Y -= 1.0f;
        }

        if (_glfw.GetKey(_windowHandle, Keys.S) == (int)InputAction.Press)
        {
            dPos.X -= 1.0f;
        }

        if (_glfw.GetKey(_windowHandle, Keys.D) == (int)InputAction.Press)
        {
            dPos.Y += 1.0f;
        }

        if (dPos.Length > 0.1f)
        {
            dPos = Vector3D.Normalize(dPos);
            pos += 1.0f * dPos.X * forwards * deltaTime;
            pos += 1.0f * dPos.Y * right * deltaTime;
        }

        if (_glfw.GetKey(_windowHandle, Keys.Escape) == (int)InputAction.Press)
        {
            return true;
        }

        //Mouse
        var dEulers = Vector3D<float>.Zero;
        _glfw.GetCursorPos(_windowHandle, out var mouseX, out var mouseY);
        _glfw.SetCursorPos(_windowHandle, 320.0, 240.0);
        _glfw.PollEvents();

        dEulers.Z = -0.1f * (float)(mouseX - 320.0);
        dEulers.Y = -0.1f * (float)(mouseY - 240.0);

        eulers.Y = Scalar.Min(89.0f, Scalar.Max(-89.0f, eulers.Y + dEulers.Y));
        eulers.Z += dEulers.Z;

        if (eulers.Z > 360.0f)
        {
            eulers.Z -= 360.0f;
        }
        else if (eulers.Z < 0)
        {
            eulers.Z += 360.0f;
        }

        var cameraTransform = transformComponents[cameraId];
        cameraTransform.Position = pos;
        cameraTransform.Eulers = eulers;
        transformComponents[cameraId] = cameraTransform;

        return false;
    }
}
