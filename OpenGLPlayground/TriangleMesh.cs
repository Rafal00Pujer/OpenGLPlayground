using Silk.NET.OpenGL;

namespace OpenGLPlayground;

internal class TriangleMesh : IDisposable
{
    private readonly GL _gl;
    private readonly uint _ebo;
    private readonly uint _vao;
    private readonly uint _elementCount;
    private readonly uint[] _vbos;

    public unsafe TriangleMesh(GL gL)
    {
        _gl = gL;

        var positions = new float[]
        {
            -1.0f, -1.0f, 0.0f, //bottom left
             1.0f, -1.0f, 0.0f, //bottom right
            -1.0f,  1.0f, 0.0f, //top left
             1.0f,  1.0f, 0.0f  //top right
        };

        var colorIndices = new int[] { 0, 1, 2, 3 };
        var elements = new int[] { 0, 1, 2, 2, 1, 3 };

        _elementCount = 6;

        _vao = _gl.GenVertexArrays(1);
        _gl.BindVertexArray(_vao);

        _vbos = new uint[2];

        //position
        _vbos[0] = _gl.GenBuffers(1);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbos[0]);
        _gl.BufferData<float>(BufferTargetARB.ArrayBuffer, positions.AsSpan(), GLEnum.StaticDraw);
        _gl.VertexAttribPointer(0, 3, GLEnum.Float, false, 12, (void*)0);
        _gl.EnableVertexAttribArray(0);

        //color
        _vbos[1] = _gl.GenBuffers(1);
        _gl.BindBuffer(GLEnum.ArrayBuffer, _vbos[1]);
        _gl.BufferData<int>(GLEnum.ArrayBuffer, colorIndices.AsSpan(), GLEnum.StaticDraw);
        _gl.VertexAttribIPointer(1, 1, GLEnum.Int, 4, (void*)0);
        _gl.EnableVertexAttribArray(1);

        //element buffer
        _ebo = _gl.GenBuffers(1);
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
        _gl.BufferData<int>(BufferTargetARB.ElementArrayBuffer, elements.AsSpan(), BufferUsageARB.StaticDraw);
    }

    public unsafe void Draw()
    {
        _gl.BindVertexArray(_vao);
        _gl.DrawElements(GLEnum.Triangles, _elementCount, DrawElementsType.UnsignedInt, (void*)0);
    }

    #region Dispose
    // To detect redundant calls
    private bool _disposedValue;

    ~TriangleMesh() => Dispose(false);

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
            _gl.DeleteVertexArrays(1, _vao);
            _gl.DeleteBuffers(_vbos.AsSpan());
            _gl.DeleteBuffers(1, _ebo);

            //set large fields to null
            _disposedValue = true;
        }
    }
    #endregion
}
