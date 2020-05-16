using GoldenRatio.Objects;
using Hawk.Framework;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace GoldenRatio
{
    public class RectangleManager
    {
        private const int RectanglesNumber = 12;
        private readonly GoldenRectangle[] _rectangles = new GoldenRectangle[RectanglesNumber];

        private readonly Program _rectangle;
        private readonly Program _square;

        private float _alphaTime;
        
        public RectangleManager()
        {
            // Rectangle Shaders
            _rectangle = new ProgramBuilder()
                .WithFileShader(ShaderType.VertexShader, "./Shaders/gradient.vert")
                .WithFileShader(ShaderType.FragmentShader, "./Shaders/gradient.frag")
                .Build();
            
            // Square Shaders
            _square = new ProgramBuilder()
                .WithFileShader(ShaderType.VertexShader, "./Shaders/blink.vert")
                .WithFileShader(ShaderType.FragmentShader, "./Shaders/blink.frag")
                .Build();
            
            // Rectangles Initialization
            CreateRectangles();
        }

        public void Render(FrameEventArgs e)
        {
            _rectangle.Bind();
            
            foreach (var rectangle in _rectangles)
            {
                GL.BindVertexArray(rectangle.RectangleVertexArray);
                GL.DrawElements(PrimitiveType.Lines, rectangle.RectangleVertices, DrawElementsType.UnsignedInt, 0);
            }
            
            _square.Bind();
            
            foreach (var rectangle in _rectangles)
            {
                GL.BindVertexArray(rectangle.SquareVertexArray);
                GL.DrawElements(PrimitiveType.TriangleFan, rectangle.SquareVertices, DrawElementsType.UnsignedInt, 0);
            }
        }
        
        public void Update(FrameEventArgs e)
        {
            const float step = 10 / 256f;
            _alphaTime = (_alphaTime + step) % MathHelper.TwoPi;
            
            _square.Bind();
            _square.SetUniformValue("time", _alphaTime);
            
            _rectangle.Bind();
            _rectangle.SetUniformValue("time", _alphaTime);
        }

        public void OnResize(ResizeEventArgs e)
        {
            var matrix = Matrix4.CreateScale(1, e.Width / (float) e.Height, 1);
            
            _rectangle.Bind();
            _rectangle.SetUniformValue("view", ref matrix);
            
            _square.Bind();
            _square.SetUniformValue("view", ref matrix);
        }
        
        private void CreateRectangles()
        {
            _rectangles[0] = new GoldenRectangle(_rectangle, _square)
            {
                Side = Side.Right,
                X = 0,
                Y = 0,
                Height = 1f
            };
            _rectangles[0].CreateBuffers();
            
            for (var i = 1; i < RectanglesNumber; i++)
            {
                _rectangles[i] = _rectangles[i - 1].Next;
                _rectangles[i].CreateBuffers();
            }
        }
    }
}