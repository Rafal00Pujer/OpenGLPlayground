using Silk.NET.Maths;

namespace OpenGLPlayground.Components;

internal struct TransformComponent
{
    public Vector3D<float> Position { readonly get; set; }

    public Vector3D<float> Eulers { readonly get; set; }
}
