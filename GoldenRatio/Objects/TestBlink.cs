using Hawk.Framework;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace GoldenRatio.Objects
{
    public class TestBlink
    {
        private readonly Program _program;
        private readonly GLObjects _buffers;
        private float _time;

        private readonly Vector2[] _vertices =
        {
            new Vector2(-1, -1),
            new Vector2(-1, 1),
            new Vector2(1, 1),
            new Vector2(1, -1)
        };

        public TestBlink()
        {
            _program = new ProgramBuilder()
                .WithFileShader(ShaderType.VertexShader, "./Shaders/blink.vert")
                .WithFileShader(ShaderType.FragmentShader, "./Shaders/blink.frag")
                .Build();
            
            _program.Bind();

            _buffers = new GLObjects(GLObject.VertexArray | GLObject.VertexBuffer);
            
            GL.BindVertexArray(_buffers.VertexArray);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffers.VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * Vector2.SizeInBytes, _vertices, BufferUsageHint.StaticDraw);
            
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);
            
        }

        public void Render(FrameEventArgs e)
        {
            GL.BindVertexArray(_buffers.VertexArray);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
        }

        public void Update(FrameEventArgs e)
        {
            const float timeStep = 3.5f;
            _time = (_time + timeStep * (float) e.Time) % MathHelper.TwoPi;
            _program.SetUniformValue("time", _time);
        }

        public void OnResize(ResizeEventArgs e)
        {
            _program.Bind();
            
            var matrix = Matrix4.CreateScale(e.Height / (float) e.Width, 1, 1);
            _program.SetUniformValue("view", ref matrix);
        }
    }
}