using Hawk.Framework;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace GoldenRatio.Objects
{
    public class Test
    {
        private readonly Program _program;
        private readonly GLObjects _buffers;

        private readonly Vector2[] _vertices =
        {
            new Vector2(-1, -1),
            new Vector2(-1, 1),
            new Vector2(1, 1),
            new Vector2(1, -1)
        };

        public Test()
        {
            _program = new ProgramBuilder()
                .WithFileShader(ShaderType.VertexShader, "./Shaders/gradient.vert")
                .WithFileShader(ShaderType.FragmentShader, "./Shaders/gradient.frag")
                .Build();
            
            _program.Bind();

            var m4 = Matrix4.Identity;
            _program.SetUniformValue("view", ref m4);
            
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

        private float _time;
        public void Update(FrameEventArgs e)
        {
            _time = (_time + (float) e.Time) % MathHelper.TwoPi;
            _program.SetUniformValue("time", _time);
        }
    }
}