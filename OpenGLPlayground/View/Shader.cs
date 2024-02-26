using Silk.NET.OpenGL;
using System.Diagnostics;

namespace OpenGLPlayground.View;

internal static class Shader
{
    public static uint MakeModule(GL gl, string filePath, ShaderType type)
    {
        var shaderSource = File.ReadAllText(filePath);

        LogShaderSource(shaderSource);

        var shaderModule = gl.CreateShader(type);
        gl.ShaderSource(shaderModule, shaderSource);
        gl.CompileShader(shaderModule);

        gl.GetShader(shaderModule, ShaderParameterName.CompileStatus, out int status);
        if (status != (int)GLEnum.True)
        {
            throw new Exception($"Shader module failed to compile: {gl.GetShaderInfoLog(shaderModule)}file path: {filePath}{Environment.NewLine}{Environment.NewLine}code:{Environment.NewLine}{shaderSource}");
        }

        return shaderModule;
    }

    public static uint MakeShader(GL gl, string vertexFilePath, string fragmentFilePath)
    {
        //To store all the shader modules
        var modules = new List<uint>
        {
            //Add a vertex shader module
            MakeModule(gl, vertexFilePath, ShaderType.VertexShader),

            //Add a fragment shader module
            MakeModule(gl, fragmentFilePath, ShaderType.FragmentShader),
        };

        //Attach all the modules then link the program
        var shader = gl.CreateProgram();

        foreach (var module in modules)
        {
            gl.AttachShader(shader, module);
        }

        gl.LinkProgram(shader);

        //Check the linking worked
        gl.GetProgram(shader, ProgramPropertyARB.LinkStatus, out int status);
        if (status != (int)GLEnum.True)
        {
            throw new Exception($"Shader failed to link: {gl.GetProgramInfoLog(shader)}");
        }

        //Modules are now unneeded and can be freed
        foreach (var module in modules)
        {
            gl.DeleteShader(module);
        }

        return shader;
    }

    [Conditional("DEBUG")]
    private static void LogShaderSource(string shaderSource) => Console.WriteLine(shaderSource);
}
