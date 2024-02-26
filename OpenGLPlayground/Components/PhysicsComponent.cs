using Silk.NET.Maths;

namespace OpenGLPlayground.Components;

internal struct PhysicsComponent
{
    public Vector3D<float> Velocity { readonly get; set; }

    public Vector3D<float> EulerVelocity { readonly get; set; }
}
