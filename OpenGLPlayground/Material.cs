using Silk.NET.OpenGL;
using StbImageSharp;

namespace OpenGLPlayground;

internal class Material : IDisposable
{
    private readonly GL _gl;
    private readonly uint _texture;

    public Material(GL gL, string filePath)
    {
        _gl = gL;

        var image = ImageResult.FromMemory(File.ReadAllBytes(filePath), ColorComponents.RedGreenBlueAlpha);

        //Make texture
        _texture = _gl.CreateTextures(GLEnum.Texture2D, 1);
        _gl.BindTexture(GLEnum.Texture2D, _texture);

        //Upload data
        _gl.TexImage2D<byte>(GLEnum.Texture2D, 0, InternalFormat.Rgba,
            (uint)image.Width, (uint)image.Height, 0,
            GLEnum.Rgba, GLEnum.UnsignedByte, image.Data.AsSpan());

        //Configure sampler
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMinFilter.Linear);
    }

    public void Use(int unit)
    {
        _gl.ActiveTexture(GLEnum.Texture0 + unit);
        _gl.BindTexture(TextureTarget.Texture2D, _texture);
    }

    #region Dispose
    // To detect redundant calls
    private bool _disposedValue;

    ~Material() => Dispose(false);

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
            _gl.DeleteTextures(1, _texture);

            //set large fields to null
            _disposedValue = true;
        }
    }
    #endregion
}
