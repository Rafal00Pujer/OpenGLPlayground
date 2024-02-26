using OpenGLPlayground.Components;
using OpenGLPlayground.Systems;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Diagnostics;

namespace OpenGLPlayground.Controller;

internal unsafe class App : IDisposable
{
    private uint _shader;

    //Systems
    private MotionSystem _motionSystem;
    private CameraSystem _cameraSystem;
    private RenderSystem _renderSystem;

    public GL Gl { get; set; }

    public Glfw Glfw { get; set; }

    public WindowHandle* WindowHandle { get; set; }

    public Dictionary<uint, TransformComponent> TransformComponents { get; set; } = [];

    public Dictionary<uint, PhysicsComponent> PhysicsComponents { get; set; } = [];

    public Dictionary<uint, RenderComponent> RenderComponents { get; set; } = [];

    public CameraComponent CameraComponent { get; set; }

    public uint CameraId { get; set; }


    public App()
    {
        SetUpGlfw();
    }

    public void Run()
    {
        var stopwatch = Stopwatch.StartNew();

        while (!Glfw.WindowShouldClose(WindowHandle))
        {
            var deltaTime = (float)stopwatch.Elapsed.TotalSeconds;
            stopwatch.Restart();

            //Console.WriteLine(deltaTime);

            _motionSystem.Update(TransformComponents, PhysicsComponents, deltaTime/*16.67f / 1000.0f*/);

            var cameraComp = CameraComponent;
            var shouldClose = _cameraSystem.Update(TransformComponents, CameraId, ref cameraComp, deltaTime/*16.67f / 1000.0f*/);
            CameraComponent = cameraComp;

            if (shouldClose)
            {
                break;
            }

            _renderSystem.Update(TransformComponents, RenderComponents);
        }
    }

    public void SetUpOpenGl()
    {
        Gl.ClearColor(0.25f, 0.5f, 0.75f, 1.0f);

        //Set the rendering region to the actual screen size
        Glfw.GetFramebufferSize(WindowHandle, out var width, out var height);
        Gl.Viewport(0, 0, (uint)width, (uint)height);

        Gl.Enable(EnableCap.DepthTest);
        Gl.DepthFunc(DepthFunction.Less);

        Gl.Enable(EnableCap.CullFace);
        Gl.CullFace(TriangleFace.Back);

        _shader = View.Shader.MakeShader(Gl,
            Path.Combine("shaders", "vertex.txt"),
            Path.Combine("shaders", "fragment.txt"));

        Gl.UseProgram(_shader);

        var projectionLocation = Gl.GetUniformLocation(_shader, "projection");
        var projection = Matrix4X4.CreatePerspectiveFieldOfView(Scalar.DegreesToRadians(45.0f), 640.0f / 480.0f, 0.1f, 10.0f);
        Gl.UniformMatrix4(projectionLocation, 1, true, projection.AsSpan());
    }

    public void MakeSystems()
    {
        _motionSystem = new MotionSystem();
        _cameraSystem = new CameraSystem(Gl, _shader, Glfw, WindowHandle);
        _renderSystem = new RenderSystem(Gl, _shader, Glfw, WindowHandle);
    }

    private void SetUpGlfw()
    {
        Glfw = Glfw.GetApi();
        Glfw.Init();

        Glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
        Glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
        Glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
        Glfw.WindowHint(WindowHintBool.OpenGLForwardCompat, true);

        WindowHandle = Glfw.CreateWindow(640, 480, "Hello Window!", null, null);
        Glfw.MakeContextCurrent(WindowHandle);
        Glfw.SetInputMode(WindowHandle, CursorStateAttribute.Cursor, CursorModeValue.CursorHidden);

        Gl = GL.GetApi(Glfw.GetProcAddress);
    }

    #region Dispose
    // To detect redundant calls
    private bool _disposedValue;

    ~App() => Dispose(false);

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                //dispose managed state (managed objects)
            }

            //free unmanaged resources (unmanaged objects) and override finalizer
            Gl.DeleteProgram(_shader);
            Glfw.Terminate();

            //set large fields to null
            _disposedValue = true;
        }
    }
    #endregion
}
