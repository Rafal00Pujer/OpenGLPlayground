using OpenGLPlayground.Components;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace OpenGLPlayground.Factories;

internal class Factory(GL gl,
    Dictionary<uint, PhysicsComponent> physicsComponents,
    Dictionary<uint, RenderComponent> renderComponents,
    Dictionary<uint, TransformComponent> transformComponents)
    : IDisposable
{
    private readonly GL _gl = gl;

    private readonly Dictionary<uint, PhysicsComponent> _physicsComponents = physicsComponents;
    private readonly Dictionary<uint, RenderComponent> _renderComponents = renderComponents;
    private readonly Dictionary<uint, TransformComponent> _transformComponents = transformComponents;

    private readonly List<uint> _vaos = new();
    private readonly List<uint> _vbos = new();
    private readonly List<uint> _textures = new();

    private uint _entitiesMade = 0;

    public uint MakeCamera(Vector3D<float> position, Vector3D<float> eulers)
    {
        var transform = new TransformComponent
        {
            Position = position,
            Eulers = eulers
        };

        _transformComponents.Add(_entitiesMade, transform);

        _entitiesMade++;

        return _entitiesMade - 1;
    }

    public void MakeCube(Vector3D<float> position, Vector3D<float> eulers, Vector3D<float> eulersVelocity)
    {
        var transform = new TransformComponent
        {
            Position = position,
            Eulers = eulers
        };

        _transformComponents.Add(_entitiesMade, transform);

        var physics = new PhysicsComponent
        {
            Velocity = Vector3D<float>.Zero,
            EulerVelocity = eulersVelocity
        };

        _physicsComponents.Add(_entitiesMade, physics);

        var render = MakeCubeMesh(new Vector3D<float>(0.25f, 0.25f, 0.25f));
        render.Material = MakeTexture(Path.Combine("img", "paper.jpg"));

        _renderComponents.Add(_entitiesMade, render);

        _entitiesMade++;
    }

    public void MakeGirl(Vector3D<float> position, Vector3D<float> eulers)
    {
        var transform = new TransformComponent
        {
            Position = position,
            Eulers = eulers
        };

        _transformComponents.Add(_entitiesMade, transform);

        var preTransform = Matrix4X4.CreateFromYawPitchRoll(
            Scalar.DegreesToRadians(90.0f),
            Scalar.DegreesToRadians(0.0f),
            Scalar.DegreesToRadians(90.0f));

        var render = MakeObjMesh(Path.Combine("models", "girl.obj"), preTransform);
        render.Material = MakeTexture(Path.Combine("img", "stargirl.png"));

        _renderComponents.Add(_entitiesMade, render);

        _entitiesMade++;
    }

    private unsafe RenderComponent MakeCubeMesh(Vector3D<float> size)
    {
        var l = size.X;
        var w = size.Y;
        var h = size.Z;

        var vertices = new float[]
        {
            l,  w, -h, 1.0f, 1.0f,  0.0f,  0.0f, -1.0f,
             l, -w, -h, 1.0f, 0.0f,  0.0f,  0.0f, -1.0f,
            -l, -w, -h, 0.0f, 0.0f,  0.0f,  0.0f, -1.0f,
            -l, -w, -h, 0.0f, 0.0f,  0.0f,  0.0f, -1.0f,
            -l,  w, -h, 0.0f, 1.0f,  0.0f,  0.0f, -1.0f,
             l,  w, -h, 1.0f, 1.0f,  0.0f,  0.0f, -1.0f,

             -l, -w,  h, 0.0f, 0.0f,  0.0f,  0.0f,  1.0f,
             l, -w,  h, 1.0f, 0.0f,  0.0f,  0.0f,  1.0f,
             l,  w,  h, 1.0f, 1.0f,  0.0f,  0.0f,  1.0f,
             l,  w,  h, 1.0f, 1.0f,  0.0f,  0.0f,  1.0f,
            -l,  w,  h, 0.0f, 1.0f,  0.0f,  0.0f,  1.0f,
            -l, -w,  h, 0.0f, 0.0f,  0.0f,  0.0f,  1.0f,

            -l,  w,  h, 1.0f, 1.0f, -1.0f,  0.0f,  0.0f,
            -l,  w, -h, 1.0f, 0.0f, -1.0f,  0.0f,  0.0f,
            -l, -w, -h, 0.0f, 0.0f, -1.0f,  0.0f,  0.0f,
            -l, -w, -h, 0.0f, 0.0f, -1.0f,  0.0f,  0.0f,
            -l, -w,  h, 0.0f, 1.0f, -1.0f,  0.0f,  0.0f,
            -l,  w,  h, 1.0f, 1.0f, -1.0f,  0.0f,  0.0f,

            l, -w, -h, 0.0f, 0.0f,  1.0f,  0.0f,  0.0f,
             l,  w, -h, 1.0f, 0.0f,  1.0f,  0.0f,  0.0f,
             l,  w,  h, 1.0f, 1.0f,  1.0f,  0.0f,  0.0f,
             l,  w,  h, 1.0f, 1.0f,  1.0f,  0.0f,  0.0f,
             l, -w,  h, 0.0f, 1.0f,  1.0f,  0.0f,  0.0f,
             l, -w, -h, 0.0f, 0.0f,  1.0f,  0.0f,  0.0f,

             -l, -w, -h, 0.0f, 0.0f,  0.0f, -1.0f,  0.0f,
             l, -w, -h, 1.0f, 0.0f,  0.0f, -1.0f,  0.0f,
             l, -w,  h, 1.0f, 1.0f,  0.0f, -1.0f,  0.0f,
             l, -w,  h, 1.0f, 1.0f,  0.0f, -1.0f,  0.0f,
            -l, -w,  h, 0.0f, 1.0f,  0.0f, -1.0f,  0.0f,
            -l, -w, -h, 0.0f, 0.0f,  0.0f, -1.0f,  0.0f,

            l,  w,  h, 1.0f, 1.0f,  0.0f,  1.0f,  0.0f,
             l,  w, -h, 1.0f, 0.0f,  0.0f,  1.0f,  0.0f,
            -l,  w, -h, 0.0f, 0.0f,  0.0f,  1.0f,  0.0f,
            -l,  w, -h, 0.0f, 0.0f,  0.0f,  1.0f,  0.0f,
            -l,  w,  h, 0.0f, 1.0f,  0.0f,  1.0f,  0.0f,
             l,  w,  h, 1.0f, 1.0f,  0.0f,  1.0f,  0.0f
        };

        var vao = _gl.GenVertexArrays(1);
        _vaos.Add(vao);
        _gl.BindVertexArray(vao);

        var vbo = _gl.GenBuffers(1);
        _vbos.Add(vbo);
        _gl.BindBuffer(GLEnum.ArrayBuffer, vbo);
        _gl.BufferData<float>(GLEnum.ArrayBuffer, vertices, GLEnum.StaticDraw);

        //Position
        _gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 32, (void*)0);
        _gl.EnableVertexAttribArray(0);

        //Texture coordinates
        _gl.VertexAttribPointer(1, 2, GLEnum.Float, false, 32, (void*)12);
        _gl.EnableVertexAttribArray(1);

        //Normal
        _gl.VertexAttribPointer(2, 3, GLEnum.Float, false, 32, (void*)20);
        _gl.EnableVertexAttribArray(2);

        var render = new RenderComponent
        {
            Vao = vao,
            VertexCount = 36
        };

        return render;
    }

    private unsafe RenderComponent MakeObjMesh(string filePath, Matrix4X4<float> preTransform)
    {
        var objectMeshFactory = new ObjectMeshFactory(filePath);
        var vertices = objectMeshFactory.Get(preTransform);

        var vao = _gl.GenVertexArrays(1);
        _vaos.Add(vao);
        _gl.BindVertexArray(vao);

        var vbo = _gl.GenBuffers(1);
        _vbos.Add(vbo);
        _gl.BindBuffer(GLEnum.ArrayBuffer, vbo);
        _gl.BufferData<float>(GLEnum.ArrayBuffer, vertices, GLEnum.StaticDraw);

        //Position
        _gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 32, (void*)0);
        _gl.EnableVertexAttribArray(0);

        //Texture coordinates
        _gl.VertexAttribPointer(1, 2, GLEnum.Float, false, 32, (void*)12);
        _gl.EnableVertexAttribArray(1);

        //Normal
        _gl.VertexAttribPointer(2, 3, GLEnum.Float, false, 32, (void*)20);
        _gl.EnableVertexAttribArray(2);

        var render = new RenderComponent
        {
            Vao = vao,
            VertexCount = (uint)vertices.Length / 8
        };

        return render;
    }

    private uint MakeTexture(string filePath)
    {
        StbImage.stbi_set_flip_vertically_on_load(1);
        var image = ImageResult.FromMemory(File.ReadAllBytes(filePath), ColorComponents.RedGreenBlueAlpha);

        //Make the texture
        var texture = _gl.GenTextures(1);
        _textures.Add(texture);
        _gl.BindTexture(GLEnum.Texture2D, texture);

        //Load data
        _gl.TexImage2D<byte>(GLEnum.Texture2D, 0, InternalFormat.Rgba,
            (uint)image.Width, (uint)image.Height, 0,
            GLEnum.Rgba, GLEnum.UnsignedByte, image.Data);

        //Configure sampler
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.NearestMipmapLinear);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMinFilter.Linear);
        _gl.GenerateMipmap(GLEnum.Texture2D);

        return texture;
    }

    #region Dispose
    // To detect redundant calls
    private bool _disposedValue;

    ~Factory() => Dispose(false);

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
            _gl.DeleteBuffers(_vbos.ToArray());
            _gl.DeleteVertexArrays(_vaos.ToArray());
            _gl.DeleteTextures(_textures.ToArray());

            //set large fields to null
            _disposedValue = true;
        }
    }
    #endregion
}
