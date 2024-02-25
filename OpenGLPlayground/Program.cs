using OpenGLPlayground;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;

internal static class Program
{
    private static GL _gl = null!;

    private unsafe static void Main(string[] args)
    {
        var glfw = Glfw.GetApi();

        if (!glfw.Init())
        {
            Console.WriteLine("GLFW couldn't start");
            return;
        }

        glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
        glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
        glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
        glfw.WindowHint(WindowHintBool.OpenGLForwardCompat, true);

        var window = glfw.CreateWindow(640, 480, "My Window", null, null);
        glfw.MakeContextCurrent(window);

        // Set up OpenGL
        _gl = GL.GetApi(glfw.GetProcAddress);
        _gl.ClearColor(0.25f, 0.5f, 0.75f, 1.0f);

        // Set the rendering region to the actual screen size
        glfw.GetFramebufferSize(window, out var width, out var height);
        _gl.Viewport(0, 0, (uint)width, (uint)height);

        var triangle = new TriangleMesh(_gl);
        var material = new Material(_gl, Path.Combine("img", "marika_matsumoto.jpg"));
        var mask = new Material(_gl, Path.Combine("img", "mask.jpg"));
        var shader = MakeShader(Path.Combine("shaders", "vertex.txt"), Path.Combine("shaders", "fragment.txt"));

        // Set texture units
        _gl.UseProgram(shader);
        _gl.Uniform1(_gl.GetUniformLocation(shader, "material"), 0);
        _gl.Uniform1(_gl.GetUniformLocation(shader, "mask"), 1);

        // Enable alfa blending
        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        while (!glfw.WindowShouldClose(window))
        {
            glfw.PollEvents();

            _gl.Clear(ClearBufferMask.ColorBufferBit);

            _gl.UseProgram(shader);
            material.Use(0);
            mask.Use(1);
            triangle.Draw();

            glfw.SwapBuffers(window);
        }

        triangle.Dispose();
        material.Dispose();
        mask.Dispose();

        _gl.DeleteProgram(shader);
        glfw.Terminate();
    }

    private static uint MakeShader(string vertexFilePath, string fragmentFilePath)
    {
        var modules = new List<uint>
        {
            MakeModule(vertexFilePath, ShaderType.VertexShader),
            MakeModule(fragmentFilePath, ShaderType.FragmentShader)
        };

        var shader = _gl.CreateProgram();

        foreach (var module in modules)
        {
            _gl.AttachShader(shader, module);
        }

        _gl.LinkProgram(shader);

        _gl.GetProgram(shader, ProgramPropertyARB.LinkStatus, out int status);
        if (status != (int)GLEnum.True)
        {
            throw new Exception($"Shader failed to link: {_gl.GetProgramInfoLog(shader)}");
        }

        foreach (var module in modules)
        {
            _gl.DeleteShader(module);
        }

        return shader;
    }

    private static uint MakeModule(string filePath, ShaderType type)
    {
        var shaderSource = File.ReadAllText(filePath);

        var shaderModule = _gl.CreateShader(type);
        _gl.ShaderSource(shaderModule, shaderSource);
        _gl.CompileShader(shaderModule);

        _gl.GetShader(shaderModule, ShaderParameterName.CompileStatus, out int status);
        if (status != (int)GLEnum.True)
        {
            throw new Exception($"Shader module failed to compile: {_gl.GetShaderInfoLog(shaderModule)}file path: {filePath}{Environment.NewLine}{Environment.NewLine}code:{Environment.NewLine}{shaderSource}");
        }

        return shaderModule;
    }
}