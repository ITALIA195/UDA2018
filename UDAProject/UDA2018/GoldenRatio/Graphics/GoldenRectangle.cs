using System;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace UDA2018.GoldenRatio.Graphics
{
    public class GoldenRectangle : IDrawable, ITrackable, IDisposable
    {
        public static Shaders Shader;
        public static Shaders SquareShader;

        public static int ProgramID;
        private readonly int _squareProgramId;

        private readonly Buffers _buffers;
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
        private readonly Vector2[] _vertices;

        private readonly CColor _squareColor = new CColor(0f, 1f, 1f);
        private readonly CColor _rectangleColor = new CColor(1f, 0.3f, 0.3f);

        public static CColor GradientStartColor = new CColor(2f);
        public static CColor GradientEndColor = new CColor(3f);

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

            _vertices = new Vector2[6];
            float dW = _width / 2f;
            float dH = _height / 2f;
            _vertices[0] = new Vector2(-dW + _x, dH + _y); // Top Left
            _vertices[1] = new Vector2(dW + _x, dH + _y); // Top Right
            _vertices[2] = new Vector2(-dW + _x, -dH + _y); // Bottom Left
            _vertices[3] = new Vector2(dW + _x, -dH + _y); // Bottom Right

            switch (_side)
            {
                case Side.Right:
                    _vertices[4] = new Vector2(_width / GoldenMath.Ratio - dW + _x, -dH + _y);
                    _vertices[5] = new Vector2(_width / GoldenMath.Ratio - dW + _x, dH + _y);
                    break;
                case Side.Bottom:
                    _vertices[4] = new Vector2(-dW + _x, dH - _height / GoldenMath.Ratio + _y);
                    _vertices[5] = new Vector2(dW + _x, dH - _height / GoldenMath.Ratio + _y);
                    break;
                case Side.Left:
                    _vertices[4] = new Vector2(dW - _width / GoldenMath.Ratio + _x, -dH + _y);
                    _vertices[5] = new Vector2(dW - _width / GoldenMath.Ratio + _x, dH + _y);
                    break;
                case Side.Top:
                    _vertices[4] = new Vector2(-dW + _x, _height / GoldenMath.Ratio - dH + _y);
                    _vertices[5] = new Vector2(dW + _x, _height / GoldenMath.Ratio - dH + _y);
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

            GL.LinkProgram(ProgramID);

            _positionAttribute = GL.GetAttribLocation(ProgramID, "position");
            _gradientStartUniform = GL.GetUniformLocation(ProgramID, "gradientStart");
            _gradientEndUniform = GL.GetUniformLocation(ProgramID, "gradientEnd");
            _uniformMatrix = GL.GetUniformLocation(ProgramID, "modelView");
            if (_positionAttribute < 0 || _gradientStartUniform < 0 || _gradientEndUniform < 0 || _uniformMatrix < 0)
                throw new Exception("Invalid shader supplied! Program cannot continue.");

            #endregion

            #region Buffers

            _buffers = new Buffers(Buffer.Index | Buffer.Vertex);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _buffers.IndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indexes.Length * sizeof(uint), _indexes, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffers.VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * Vector2.SizeInBytes, _vertices, BufferUsageHint.StaticDraw);

            #endregion

            #endregion

            #region Squares

            #region Shader

            _squareProgramId = GL.CreateProgram();
            SquareShader.LinkProgram(_squareProgramId);
            GL.LinkProgram(_squareProgramId);

            _squarePositionAttribute = GL.GetAttribLocation(_squareProgramId, "position");
            _squareColorAttribute = GL.GetAttribLocation(_squareProgramId, "colorIn");
            _squareAlphaUniform = GL.GetUniformLocation(_squareProgramId, "alpha");
            _squareUniformMatrix = GL.GetUniformLocation(_squareProgramId, "modelView");
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

            CColor[] colors = {
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
            if (!IsVisible) return;
            GL.UseProgram(ProgramID);

            GL.EnableVertexAttribArray(_positionAttribute);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _buffers.VertexBuffer);
            GL.VertexAttribPointer(_positionAttribute, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _buffers.IndexBuffer);
            GL.DrawElements(PrimitiveType.Lines, _indexes.Length, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(_positionAttribute);

            if (!_highlighted) return;
            GL.UseProgram(_squareProgramId);

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
        private float _alpha;
        public override void Update()
        {
            CreateZoomMatrix(out Matrix4 zoomMatrix);
            CreateUniformMatrix(ref zoomMatrix, out _matrix);

            GL.UseProgram(_squareProgramId);
            GL.Uniform1(_squareAlphaUniform, _highlighted ? Alpha : 0);
            GL.UniformMatrix4(_squareUniformMatrix, false, ref _matrix);

            GL.UseProgram(ProgramID);
            GL.UniformMatrix4(_uniformMatrix, false, ref _matrix);

            GL.Uniform3(_gradientStartUniform, GradientStartColor);
            GL.Uniform3(_gradientEndUniform, GradientEndColor);
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
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateTranslation(-_translation.X/Window.Width, -_translation.Y/Window.Height, 0f));
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateRotationZ(_rotation));
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateScale(_zoom, _zoom, 1f));
        }

        private static void CreateUniformMatrix(ref Matrix4 zoomMatrix, out Matrix4 matrix)
        {
            matrix = Matrix4.Identity;
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateScale(1f / Window.Width * 2f, 1f / Window.Height * 2f, 1f)); //TODO: Remove * 2f
            Matrix4.Mult(ref matrix, ref zoomMatrix, out matrix);
        }

        public bool IsVisible
        {
            get
            {
                float width = Window.Width / _zoom;
                float height = Window.Height / _zoom;
                Rect windowRect = new Rect(_translation.X / 2f + -width / 2f, _translation.Y / 2f + -height / 2f, width, height);
                return _vertices.Any(vertex => GoldenMath.IsInside(vertex, windowRect));
            }
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
                    ZoomOffset = GoldenMath.Min(Window.Width / SubRectangle.Width, Window.Height / SubRectangle.Height) - (Window.Height / (Window.Height - 20f) - 1f)
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

        public CallbackTrackFinish TrackFinish()
        {
            _highlighted = true;
            return () => _highlighted;
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

        public void Dispose()
        {
            GL.DeleteBuffer(_buffers.VertexBuffer);
            GL.DeleteBuffer(_buffers.IndexBuffer);
            GL.DeleteBuffer(_squareBuffer1.IndexBuffer);
            GL.DeleteBuffer(_squareBuffer1.ColorBuffer);
            GL.DeleteBuffer(_squareBuffer2.IndexBuffer);
            GL.DeleteBuffer(_squareBuffer2.ColorBuffer);
            GL.DeleteProgram(_squareProgramId);
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
