using GoldenRatio.Objects;
using Hawk.Framework;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace GoldenRatio
{
    public class RectangleManager
    {
        private const int RectanglesNumber = 1;
        private readonly GoldenRectangle[] _rectangles = new GoldenRectangle[RectanglesNumber];

        private readonly Program _rectangle;
        private readonly Program _square;

        private readonly int _squareTimeLocation;
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
            _square.Bind();

            _squareTimeLocation = _square.GetUniformLocation("time");
            
            // Rectangles Initialization
            CreateRectangles();
        }

        public void Render(FrameEventArgs e)
        {
            foreach (var rectangle in _rectangles)
                rectangle.Render();
        }
        
        public void Update(FrameEventArgs e)
        {
            const float step = 10 / 256f;
            _alphaTime += step;
            
            GL.Uniform1(_squareTimeLocation, _alphaTime);
        }

        public void OnResize(ResizeEventArgs e)
        {
            var matrix = Matrix4.CreateScale(e.Height / (float) e.Width, 1, 1);
            
            _rectangle.Bind();
            _rectangle.SetUniformValue("view", ref matrix);
            
            _square.Bind();
            _square.SetUniformValue("view", ref matrix);
        }
        
        private void CreateRectangles()
        {
            for (var i = 0; i < RectanglesNumber; i++)
            {
                _rectangles[i] = new GoldenRectangle(_rectangle, _square)
                {
                    Side = Side.Right,
                    X = 0,
                    Y = 0,
                    Height = 1f
                };
                _rectangles[i].BindData();
            }
        }
    }
}