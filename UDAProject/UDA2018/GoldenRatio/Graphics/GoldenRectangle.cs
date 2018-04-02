using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace UDA2018.GoldenRatio.Graphics
{
    public class GoldenRectangle : IDrawable, ITrackable
    {
        private readonly Shaders _shaders;
        private readonly Buffers _buffers;

        private readonly Shaders _squareShaders;
        private readonly Buffers _squareBuffer1;
        private readonly Buffers _squareBuffer2;

        private readonly int _squarePositionAttribute;
        private readonly int _squareColorAttribute;
        private readonly int _squareAlphaUniform;
        private readonly int _squareUniformMatrix;

        private readonly int _positionAttribute;
        private readonly int _gradientStartUniform;
        private readonly int _gradientEndUniform;
        private readonly int _uniformMatrix;

        private readonly Side _side;
        private readonly float _x;
        private readonly float _y;
        private readonly float _width;
        private readonly float _height;

        private Matrix4 _matrix;
        private readonly uint[] _indexes;

        private readonly CColor _squareColor = new CColor(0f, 1f, 1f);
        private readonly CColor _rectangleColor = new CColor(1f, 0.3f, 0.3f);

        public CColor _gradientStart = new CColor(2f);
        public CColor _gradientEnd = new CColor(3f);

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

            #region Vertices, Color and Index Buffer data

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

            #endregion

            #region Create matrices

            CreateZoomMatrix(out Matrix4 zoomMatrix);
            CreateUniformMatrix(ref zoomMatrix, out _matrix);

            #endregion

            #region Golden Rectangle

            #region Shaders
            
            _shaders = new Shaders("vertex", "fragment"); //BUG: Shaders gets read from file every instance of GoldenRectangle
            GL.LinkProgram(_shaders.ProgramID);

            _positionAttribute = GL.GetAttribLocation(_shaders.ProgramID, "position");
            _gradientStartUniform = GL.GetUniformLocation(_shaders.ProgramID, "gradientStart");
            _gradientEndUniform = GL.GetUniformLocation(_shaders.ProgramID, "gradientEnd");
            _uniformMatrix = GL.GetUniformLocation(_shaders.ProgramID, "modelView");
            if (_positionAttribute < 0 || _gradientStartUniform < 0 || _gradientEndUniform < 0 || _uniformMatrix < 0)
                throw new Exception("Invalid shader supplied! Program cannot continue.");

            #endregion

            #region Buffers

            _buffers = new Buffers(Buffer.Index | Buffer.Vertex);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _buffers.IndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indexes.Length * sizeof(uint), _indexes, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffers.VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vector2.SizeInBytes, vertices, BufferUsageHint.StaticDraw);

            #endregion

            #endregion

            #region Squares

            #region Shader

            _squareShaders = new Shaders("squareVertex", "squareFragment"); //BUG: Shaders gets read from file every instance of GoldenRectangle
            GL.LinkProgram(_squareShaders.ProgramID);

            _squarePositionAttribute = GL.GetAttribLocation(_squareShaders.ProgramID, "position");
            _squareColorAttribute = GL.GetAttribLocation(_squareShaders.ProgramID, "colorIn");
            _squareAlphaUniform = GL.GetUniformLocation(_squareShaders.ProgramID, "alpha");
            _squareUniformMatrix = GL.GetUniformLocation(_squareShaders.ProgramID, "modelView");
            if (_squarePositionAttribute < 0 || _squareColorAttribute < 0 || _squareUniformMatrix < 0)
                throw new Exception("Invalid shader supplied! Program cannot continue.");

            #endregion

            #region Buffers
            
            uint[] indexes;
            switch (_side)
            {
                case Side.Right:
                    indexes = new uint[] { 0, 5, 4, 2 };
                    break;
                case Side.Bottom:
                    indexes = new uint[] { 0, 4, 5, 1 };
                    break;
                case Side.Left:
                    indexes = new uint[] { 5, 1, 3, 4 };
                    break;
                case Side.Top:
                    indexes = new uint[] { 4, 2, 3, 5 };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            colors = new[] // We could use new CColor[6].Select(xa => _squareColor).ToArray() for readability
            {
                _squareColor,
                _squareColor,
                _squareColor,
                _squareColor,
                _squareColor,
                _squareColor
            };

            _squareBuffer1 = new Buffers(Buffer.Index | Buffer.Color);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _squareBuffer1.IndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexes.Length * sizeof(uint), indexes, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _squareBuffer1.ColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Length * CColor.Size, colors, BufferUsageHint.StaticDraw);

            switch (_side)
            {
                case Side.Right:
                    indexes = new uint[] { 5, 1, 3, 4 };
                    break;
                case Side.Bottom:
                    indexes = new uint[] { 4, 2, 3, 5 };
                    break;
                case Side.Left:
                    indexes = new uint[] { 0, 5, 4, 2 };
                    break;
                case Side.Top:
                    indexes = new uint[] { 0, 1, 5, 4 };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            colors = new[]
            {
                _rectangleColor,
                _rectangleColor,
                _rectangleColor,
                _rectangleColor,
                _rectangleColor,
                _rectangleColor
            };

            _squareBuffer2 = new Buffers(Buffer.Index | Buffer.Color);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _squareBuffer2.IndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexes.Length * sizeof(uint), indexes, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _squareBuffer2.ColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Length * CColor.Size, colors, BufferUsageHint.StaticDraw);


            #endregion

            #endregion
        }

        public GoldenRectangle(Side side, float? width, float? height) : this(side, 0, 0, width, height)
        {}

        public override void Draw()
        {
            GL.UseProgram(_shaders.ProgramID);

            GL.EnableVertexAttribArray(_positionAttribute);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffers.VertexBuffer);
            GL.VertexAttribPointer(_positionAttribute, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _buffers.IndexBuffer);
            GL.DrawElements(PrimitiveType.Lines, _indexes.Length, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(_positionAttribute);

            if (!_highlighted) return;
            GL.UseProgram(_squareShaders.ProgramID);

            GL.EnableVertexAttribArray(_squarePositionAttribute);
            GL.EnableVertexAttribArray(_squareColorAttribute);

            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffers.VertexBuffer);
            GL.VertexAttribPointer(_squarePositionAttribute, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _squareBuffer1.ColorBuffer);
            GL.VertexAttribPointer(_squareColorAttribute, 3, VertexAttribPointerType.Float, true, CColor.Size, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _squareBuffer1.IndexBuffer);
            GL.DrawElements(PrimitiveType.Quads, 4, DrawElementsType.UnsignedInt, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffers.VertexBuffer);
            GL.VertexAttribPointer(_squarePositionAttribute, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _squareBuffer2.ColorBuffer);
            GL.VertexAttribPointer(_squareColorAttribute, 3, VertexAttribPointerType.Float, true, CColor.Size, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _squareBuffer2.IndexBuffer);
            GL.DrawElements(PrimitiveType.Quads, 4, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(_squareColorAttribute);
            GL.DisableVertexAttribArray(_squarePositionAttribute);
        }

        private bool _highlighted;
        private float _alpha = .3f;
        public override void Update()
        {
            CreateZoomMatrix(out Matrix4 zoomMatrix);
            CreateUniformMatrix(ref zoomMatrix, out _matrix);

            GL.UseProgram(_squareShaders.ProgramID);
            GL.Uniform1(_squareAlphaUniform, _highlighted ? Alpha : 0);
            GL.UniformMatrix4(_squareUniformMatrix, false, ref _matrix);

            GL.UseProgram(_shaders.ProgramID);
            GL.UniformMatrix4(_uniformMatrix, false, ref _matrix);

            _gradientStart.Fade();
            _gradientEnd.Fade();
            GL.Uniform3(_gradientStartUniform, _gradientStart);
            GL.Uniform3(_gradientEndUniform, _gradientEnd);
        }

        private float Alpha
        {
            get
            {
                _alpha += Window.DeltaTime * 4f;
                switch ((int)_alpha)
                {

                    case 0:
                    case 2:
                        return _alpha % 1;
                    case 1:
                    case 3:
                        return 1f - _alpha % 1;
                    default:
                        _highlighted = false;
                        _alpha = 0f;
                        return 0f;
                }
            }
        }

        private static float _zoom = 1f;
        private static float _rotation = 0f;
        private static Vector2 _translation;
        private static void CreateZoomMatrix(out Matrix4 matrix)
        {
            matrix = Matrix4.Identity;
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateTranslation(-_translation.X/Window.ScreenWidth, -_translation.Y/Window.ScreenHeight, 0f));
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateRotationZ(_rotation));
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateScale(_zoom, _zoom, 1f));
        }

        private static void CreateUniformMatrix(ref Matrix4 zoomMatrix, out Matrix4 matrix)
        {
            matrix = Matrix4.Identity;
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateScale(1f / Window.ScreenWidth * 2f, 1f / Window.ScreenHeight * 2f, 1f)); //TODO: Remove * 2f
            Matrix4.Mult(ref matrix, ref zoomMatrix, out matrix);
        }

        public bool IsRightOrLeft => ((int)_side & 1) == 0;

        public GoldenRectangle Next
        {
            get
            {
                switch (_side)
                {
                    case Side.Right:
                        return new GoldenRectangle(Side.Bottom, _x + _width / 2f - (_width - _height) / 2f, _y, null, _height);
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

        public TrackInfo TrackInfo
        {
            get
            {
                Vector2 offsetPosition;
                switch (_side)
                {
                    case Side.Right:
                        offsetPosition = new Vector2(_height, 0);
                        break;
                    case Side.Bottom:
                        offsetPosition = new Vector2(0, -_width);
                        break;
                    case Side.Left:
                        offsetPosition = new Vector2(-_height, 0);
                        break;
                    case Side.Top:
                        offsetPosition = new Vector2(0, _width);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_side));
                }

                return new TrackInfo
                {
                    OffsetPosition = offsetPosition,
                    ZoomOffset = GoldenMath.Min(Window.ScreenWidth / SubRectangle.Width, Window.ScreenHeight / SubRectangle.Height) - (Window.ScreenHeight / (Window.ScreenHeight - 20f) - 1f)
                };
            }
        }

        public Rect SubRectangle
        {
            get
            {
                switch (_side)
                {
                    case Side.Right:
                        return new Rect(_x + _height, _y, _width - _height, _height);
                    case Side.Bottom:
                        return new Rect(_x, _y + _width, _width, _height - _width);
                    case Side.Left:
                        return new Rect(_x, _y, _width - _height, _height);
                    case Side.Top:
                        return new Rect(_x, _y, _width, _height - _width);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_side));
                }
            }
        }

        public unsafe void TrackFinish(out bool* ptr)
        {
            _highlighted = true;
            fixed (bool* boolptr = &_highlighted)
                ptr = boolptr;
        }

        public Vector2 TrackPosition
        {
            get => _translation;
            set => _translation = value;
        }

        public float TrackZoom
        {
            get => _zoom;
            set => _zoom = value;
        }
    }

    public enum Side
    {
        Right,
        Bottom,
        Left,
        Top
    }
}
