using OpenGLPlayground.Components;
using OpenGLPlayground.Controller;
using OpenGLPlayground.Factories;
using Silk.NET.Maths;

using var app = new App();
app.SetUpOpenGl();
app.MakeSystems();

using var factory = new Factory(app.Gl, app.PhysicsComponents, app.RenderComponents, app.TransformComponents);

factory.MakeCube(new Vector3D<float>(3.0f, 0.0f, 0.25f), Vector3D<float>.Zero, new Vector3D<float>(0.0f, 0.0f, 10.0f));

factory.MakeGirl(new Vector3D<float>(5.0f, 0.0f, 0.0f), new Vector3D<float>(0.0f, 0.0f, 180.0f));

var cameraEntity = factory.MakeCamera(Vector3D<float>.UnitZ, Vector3D<float>.Zero);
var camera = new CameraComponent();

app.CameraComponent = camera;
app.CameraId = cameraEntity;

app.Run();