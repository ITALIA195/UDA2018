using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace UDA2018.GoldenRatio.Graphics
{
    public class GoldenRectangle : IDrawable
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

        public CColor _gradientStart = new CColor(2f);
        public CColor _gradientEnd = new CColor(3f);

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

            colors = new[]
            {
                CColor.Cyan, CColor.Empty, CColor.Cyan, CColor.Empty, CColor.Cyan, CColor.Cyan
            };

            uint[] indexes;
            if (IsRightOrLeft)
                indexes = new uint[] {0, 5, 4, 2};
            else
                indexes = new uint[] {0, 4, 5, 1};


            _squareBuffer1 = new Buffers(Buffer.Index | Buffer.Color);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _squareBuffer1.IndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexes.Length * sizeof(uint), indexes, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _squareBuffer1.ColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Length * CColor.Size, colors, BufferUsageHint.StaticDraw);

            colors = new[]
            {
                CColor.Empty, CColor.Teal, CColor.Empty, CColor.Teal, CColor.Teal, CColor.Teal
            };

            if (IsRightOrLeft)
                indexes = new uint[] { 5, 1, 3, 4 };
            else
                indexes = new uint[] { 4, 2, 3, 5 };

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
            GL.DrawElements(PrimitiveType, _indexes.Length, DrawElementsType.UnsignedInt, 0);

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
                _alpha += Window.DeltaTime * 6f;
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

        public static float Zoom { get; set; } = 1f;
        public static float Rotation { get; set; } = 0f;
        public static Vector2 Translate { get; set; } = new Vector2(0, 0);

        private static void CreateZoomMatrix(out Matrix4 matrix) //TODO: Create animation
        {
            matrix = Matrix4.Identity;
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateTranslation(-Translate.X/Window.ScreenWidth, Translate.Y/Window.ScreenHeight, 0f));
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateRotationZ(Rotation)); //TODO: Maybe rotate too?
            GoldenMath.MatrixMult(ref matrix, Matrix4.CreateScale(Zoom, Zoom, 1f));
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
        public Vector2 SubRectangle //TODO: Might be obsolete
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
