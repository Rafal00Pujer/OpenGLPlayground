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

        var window = glfw.CreateWindow(640, 480, "My Window", null, null);
        glfw.MakeContextCurrent(window);

        _gl = GL.GetApi(glfw.GetProcAddress);

        _gl.ClearColor(0.25f, 0.5f, 0.75f, 1.0f);

        var shader = MakeShader(Path.Combine("shaders", "vertex.txt"), Path.Combine("shaders", "fragment.txt"));

        while (!glfw.WindowShouldClose(window))
        {
            glfw.PollEvents();

            _gl.Clear(ClearBufferMask.ColorBufferBit);

            _gl.UseProgram(shader);

            glfw.SwapBuffers(window);
        }

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
            throw new Exception($"Shader module failed to compile: {_gl.GetShaderInfoLog(shaderModule)},{Environment.NewLine}code:{Environment.NewLine}{shaderSource}");
        }

        return shaderModule;
    }
}