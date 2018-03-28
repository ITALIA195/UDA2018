using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace UDA2018.GoldenRatio.Graphics
{
    public class GoldenRectangle : IDrawable
    {
        public static Matrix4 GlobalMatrix = Matrix4.Identity;

        private readonly Shaders _shaders;
        private readonly Buffers _buffers;

        private readonly int _positionAttribute;
        private readonly int _colorAttribute;
        private readonly int _uniformMatrix;

        private readonly Side _side;
        private readonly float _x;
        private readonly float _y;
        private readonly float _width;
        private readonly float _height;

        private Matrix4 _matrix;
        private readonly uint[] _indexes;

        private readonly CColor _gradientStart = CColor.Red; // 1 0 0
        private readonly CColor _gradientEnd = CColor.Blue; // 0 0 1 

        //BUG: (Critical) Only the last instance of the class works properly 
        public GoldenRectangle(Side side, float x, float y, float? width, float? height)
        {
            if (!width.HasValue && !height.HasValue)
                throw new ArgumentNullException($"The arguments {nameof(width)} and {nameof(height)} cannot be null at the same time.");
            _side = side;
            _x = x;
            _y = y;
            if (IsRightOrLeft)
            {
                _width = (width ?? height * GoldenMath.Ratio).Value;
                _height = (height ?? width / GoldenMath.Ratio).Value;
            }
            else
            {
                _width = (width ?? height / GoldenMath.Ratio).Value;
                _height = (height ?? width * GoldenMath.Ratio).Value;
            }
            Vector2[] vertices = new Vector2[6];
            float dW = _width / 2f;
            float dH = _height / 2f;
            vertices[0] = new Vector2(-dW + _x, dH + _y); // Top Left
            vertices[1] = new Vector2(dW + _x, dH + _y); // Top Right
            vertices[2] = new Vector2(-dW + _x, -dH + _y); // Bottom Left
            vertices[3] = new Vector2(dW + _x, -dH + _y); // Bottom Right

            CColor[] colors = new CColor[6];
            colors[0] = _gradientStart;
            colors[1] = _gradientEnd;
            colors[2] = _gradientStart;
            colors[3] = _gradientEnd;

            switch (_side)
            {
                case Side.Right:
                    colors[4] = colors[5] = CColor.Lerp(_gradientStart, _gradientEnd, GoldenMath.OneOverRatio);
                    vertices[4] = new Vector2(_width / GoldenMath.Ratio - dW + _x, -dH + _y);
                    vertices[5] = new Vector2(_width / GoldenMath.Ratio - dW + _x, dH + _y);
                    break;
                case Side.Bottom:
                    colors[4] = _gradientStart;
                    colors[5] = _gradientEnd;
                    vertices[4] = new Vector2(-dW + _x, dH - _height / GoldenMath.Ratio + _y);
                    vertices[5] = new Vector2(dW + _x, dH - _height / GoldenMath.Ratio + _y);
                    break;
                case Side.Left:
                    colors[4] = colors[5] = CColor.Lerp(_gradientStart, _gradientEnd, 1f - GoldenMath.OneOverRatio);
                    vertices[4] = new Vector2(dW - _width / GoldenMath.Ratio + _x, -dH + _y);
                    vertices[5] = new Vector2(dW - _width / GoldenMath.Ratio + _x, dH + _y);
                    break;
                case Side.Top:
                    colors[4] = _gradientStart;
                    colors[5] = _gradientEnd;
                    vertices[4] = new Vector2(-dW + _x, _height / GoldenMath.Ratio - dH + _y);
                    vertices[5] = new Vector2(dW + _x, _height / GoldenMath.Ratio - dH + _y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _indexes = new uint[]
            {
                0, 1, // Top
                1, 3, // Right
                3, 2, // Bottom
                2, 0, // Left
                4, 5, // Line
            };

            CreateMatrix(out _matrix);

            _shaders = new Shaders("vertex", "fragment"); //BUG: Shaders gets read from file every instance of GoldenRectangle
            GL.LinkProgram(_shaders.ProgramID);

            _positionAttribute = GL.GetAttribLocation(_shaders.ProgramID, "position");
            _colorAttribute = GL.GetAttribLocation(_shaders.ProgramID, "colorIn");
            _uniformMatrix = GL.GetUniformLocation(_shaders.ProgramID, "modelView");
            if (_positionAttribute < 0 || _colorAttribute < 0 || _uniformMatrix < 0)
                throw new Exception("Invalid shader supplied! Program cannot continue.");

            _buffers = new Buffers(Buffer.Index | Buffer.Vertex | Buffer.Color);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _buffers.IndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indexes.Length * sizeof(uint), _indexes, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffers.ColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Length * CColor.Size, colors, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffers.VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vector2.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
        }

        public GoldenRectangle(Side side, float? width, float? height) : this(side, 0, 0, width, height)
        {}

        public override void Draw()
        {
            GL.UseProgram(_shaders.ProgramID);
            GL.UniformMatrix4(_uniformMatrix, false, ref _matrix);

            GL.EnableVertexAttribArray(_colorAttribute);
            GL.EnableVertexAttribArray(_positionAttribute);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffers.ColorBuffer);
            GL.VertexAttribPointer(_colorAttribute, 3, VertexAttribPointerType.Float, true, CColor.Size, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffers.VertexBuffer);
            GL.VertexAttribPointer(_positionAttribute, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _buffers.IndexBuffer);
            GL.DrawElements(PrimitiveType, _indexes.Length, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(_positionAttribute);
            GL.DisableVertexAttribArray(_colorAttribute);
        }

        public override void Update()
        {
            CreateMatrix(out _matrix);
        }

        private void CreateMatrix(out Matrix4 matrix)
        {
            matrix = Matrix4.Identity;
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateTranslation(0f, 0f, 0f));
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateRotationZ(0f));
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateScale(1f / Window.ScreenWidth * 2f, 1f / Window.ScreenHeight * 2f, 1f)); //TODO: Remove * 2f

            Matrix4.Mult(ref matrix, ref GlobalMatrix, out matrix);
        }

        public bool IsRightOrLeft => ((int)_side & 1) == 0;
        public GoldenRectangle Next
        {
            get
            {
                switch (_side)
                {
                    case Side.Right:
                        return new GoldenRectangle(Side.Bottom, x: _x + _width / 2f - (_width - _height) / 2f, y: _y, width: null, height: _height);
                    case Side.Bottom:
                        return new GoldenRectangle(Side.Left, _x, _y - _height / 2f + (_height - _width) / 2f, _width, null);
                    case Side.Left:
                        return new GoldenRectangle(Side.Top, _x - _width / 2f + (_width - _height) / 2f, _y, null, _height);
                    case Side.Top:
                        return new GoldenRectangle(Side.Right, _x, _y + _height / 2f - (_height - _width) / 2f, _width, null);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_side));
                }
            }
        }
        public Vector2 SubRectangle
        {
            get
            {
                Vector2 vector;
                switch (_side)
                {
                    case Side.Right:
                        vector = new Vector2(_x + _height + (_width - _height) / 2f, _y + _height / 2f);
                        break;
                    case Side.Bottom:
                        vector = new Vector2(_x + _width / 2f, _y + _width + (_height - _width) / 2f);
                        break;
                    case Side.Left:
                        vector = new Vector2(_x + (_width - _height) / 2f, _y + _height / 2f);
                        break;
                    case Side.Top:
                        vector = new Vector2(_x + _width / 2f, _y + (_height - _width) / 2f);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_side));
                }
                vector += new Vector2(-Window.ScreenWidth / 2f, -Window.ScreenHeight / 2f);
                return vector;
            }
        }

        public PrimitiveType PrimitiveType => PrimitiveType.Lines;
        public float X => _x;
        public float Y => _y;
        public float Width => _width;
        public float Height => _height;
    }

    public enum Side
    {
        Right,
        Bottom,
        Left,
        Top
    }
}
