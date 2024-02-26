using Silk.NET.Maths;

namespace OpenGLPlayground.Components;

internal struct CameraComponent
{
    public Vector3D<float> Right { readonly get; set; }

    public Vector3D<float> Up { readonly get; set; }

    public Vector3D<float> Forward { readonly get; set; }
}
