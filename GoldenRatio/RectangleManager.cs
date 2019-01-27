using GoldenRatio.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace GoldenRatio
{
    public class RectangleManager
    {
        private const int RectanglesNumber = 12;
        private readonly GoldenRectangle[] _rectangles = new GoldenRectangle[RectanglesNumber];

        private readonly Shaders _rectangle;
        private readonly Shaders _square;

        private const int RectPosition = 0;
        private readonly int _rectView;
        private readonly int _rectGradientStart;
        private readonly int _rectGradientEnd;

        private const int SquarePosition = 0;
        private readonly int _squareView;
        private readonly int _squareAlpha;

        private readonly CColor _start = new CColor(0, 1, 0);
        private readonly CColor _end = new CColor(1, 0, 0);
        
        public RectangleManager()
        {
            // Rectangle Shaders
            _rectangle = new Shaders("rectangle");
            _rectangle.Bind();
            
            _rectView = _rectangle.GetUniformLocation("view");
            _rectGradientStart = _rectangle.GetUniformLocation("gradientStart");
            _rectGradientEnd = _rectangle.GetUniformLocation("gradientEnd");
            
            GL.VertexAttribPointer(RectPosition, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            GL.EnableVertexAttribArray(RectPosition);
            
            // Square Shaders
            _square = new Shaders("square");
            _square.Bind();

            _squareView = _square.GetUniformLocation("view");
            _squareAlpha = _square.GetUniformLocation("alpha");
            
            GL.VertexAttribPointer(SquarePosition, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            GL.EnableVertexAttribArray(SquarePosition);
            
            // Rectangles Initialization
            CreateRectangles();
        }

        public void Draw()
        {
            foreach (var rectangle in _rectangles)
                rectangle.Draw();
        }
        
        public void Update()
        {
            var matrix = Matrix4.Identity * 
                         Matrix4.CreateScale(Window.OneOverScreenRatio, 1, 0);
            
            foreach (var rectangle in _rectangles)
                rectangle.Update(_squareAlpha);
            
            _rectangle.Bind();
            GL.UniformMatrix4(_rectView, false, ref matrix);
            GL.Uniform3(_rectGradientStart, _start);
            GL.Uniform3(_rectGradientEnd, _end);
            
            _square.Bind();
            GL.UniformMatrix4(_squareView, false, ref matrix);
        }

        private void CreateRectangles()
        {
            for (int i = 0; i < RectanglesNumber; i++)
            {
                _rectangles[i] = new GoldenRectangle(_rectangle, _square)
                {
                    Side = Side.Right,
                    X = 0,
                    Y = 0,
                    Width = 1f
                };
                _rectangles[i].BindData();
            }
        }
    }
}