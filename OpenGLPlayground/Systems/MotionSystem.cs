using OpenGLPlayground.Components;

namespace OpenGLPlayground.Systems;

internal class MotionSystem
{
    public void Update(Dictionary<uint, TransformComponent> transformComponents, Dictionary<uint, PhysicsComponent> physicsComponents, float deltaTime)
    {
        foreach (var key in physicsComponents.Keys)
        {
            var physics = physicsComponents[key];

            var transform = transformComponents[key];
            transform.Position += physics.Velocity * deltaTime;
            transform.Eulers += physics.EulerVelocity * deltaTime;

            if (transform.Eulers.Z > 360.0f)
            {
                var eulers = transform.Eulers;
                eulers.Z -= 360.0f;
                transform.Eulers = eulers;
            }

            transformComponents[key] = transform;
        }
    }
}
