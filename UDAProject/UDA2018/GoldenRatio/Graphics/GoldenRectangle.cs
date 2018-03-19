using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace UDA2018.GoldenRatio.Graphics
{
    public class GoldenRectangle : IDrawable
    {
        private readonly Side _side;
        private readonly float _x;
        private readonly float _y;
        private readonly float _width;
        private readonly float _height;

        public GoldenRectangle(Side side, float? width, float? height)
        {
            if (!width.HasValue && !height.HasValue)
                throw new ArgumentNullException($"The arguments {nameof(width)} and {nameof(height)} cannot be null at the same time.");
            _side = side;
            if (width.HasValue)
            {
                _width = width.Value;
                _height = ((int)side & 1) == 0 ? _width / GoldenMath.Ratio : _width * GoldenMath.Ratio;
            }
            else
            {
                _height = height.Value;
                _width = ((int)side & 1) == 0 ? _height * GoldenMath.Ratio : _height / GoldenMath.Ratio;
            }
            _x = (Window.ScreenWidth - _width) / 2f;
            _y = (Window.ScreenHeight - _height) / 2f;
        }

        public GoldenRectangle(Side side, float x, float y, float? width, float? height)
        {
            if (!width.HasValue && !height.HasValue)
                throw new ArgumentNullException($"The arguments {nameof(width)} and {nameof(height)} cannot be null at the same time.");
            _side = side;
            _x = x;
            _y = y;
            if (width.HasValue)
            {
                _width = width.Value;
                _height = ((int)side & 1) == 0 ? _width / GoldenMath.Ratio : _width * GoldenMath.Ratio;
            }
            else
            {
                _height = height.Value;
                _width = ((int)side & 1) == 0 ? _height * GoldenMath.Ratio : _height / GoldenMath.Ratio;
            }
        }

        public bool IsLeftOrRight => ((int) _side & 1) == 1;

        /// <summary>
        /// The coordinate of the center of the <see cref="GoldenRectangle"/>, relative to the center of the window (0, 0)
        /// </summary>
        public Vector2 CenterPosition => new Vector2(-Window.ScreenWidth / 2f + (_x + _width / 2f), -Window.ScreenHeight / 2f + (_y + _height / 2f));

        /// <summary>
        /// The coordinate of the center of the sub-<see cref="GoldenRectangle"/>, relative to the center of the window (0, 0)
        /// </summary>
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

        public float X => _x;
        public float Y => _y;
        public float Width => _width;
        public float Height => _height;

        public GoldenRectangle Next
        {
            get
            {
                switch (_side)
                {
                    case Side.Right:
                        return new GoldenRectangle(Side.Bottom, _x + _height, Y, null, _height);
                    case Side.Bottom:
                        return new GoldenRectangle(Side.Left, _x, _y + _width, _width, null);
                    case Side.Left:
                        return new GoldenRectangle(Side.Top, _x, _y, null, _height);
                    case Side.Top:
                        return new GoldenRectangle(Side.Right, _x, _y, _width, null);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_side));
                }
            }
        }

        public Vector2[] Vertices
        {
            get // Sooooo much math x_x
            {
                Vector2[] vertices = new Vector2[10];
                float dW, dH;
                vertices[0] = new Vector2(-(dW = Window.ScreenWidth / 2f - _x), -(dH = Window.ScreenHeight / 2f - _y));  // top-left
                vertices[1] = new Vector2(_width - dW, -dH); // top-right
                vertices[2] = vertices[1]; // top-right
                vertices[3] = new Vector2(_width - dW, _height - dH); // bot-right
                vertices[4] = vertices[3]; // bot-right 
                vertices[5] = new Vector2(-dW, _height - dH); // bot-left
                vertices[6] = vertices[5]; // bot-left
                vertices[7] = vertices[0]; //top-left
                switch (_side)
                {
                    case Side.Top:
                        vertices[8] = new Vector2(-dW, _height - _height / GoldenMath.Ratio - dH);
                        vertices[9] = new Vector2(_width - dW, _height - _height / GoldenMath.Ratio - dH);
                        break;
                    case Side.Bottom:
                        vertices[8] = new Vector2(-dW, _height / GoldenMath.Ratio - dH);
                        vertices[9] = new Vector2(_width - dW, _height / GoldenMath.Ratio - dH);
                        break;
                    case Side.Right:
                        vertices[8] = new Vector2(_width / GoldenMath.Ratio - dW, -dH);
                        vertices[9] = new Vector2(_width / GoldenMath.Ratio - dW, _height - dH);
                        break;
                    case Side.Left:
                        vertices[8] = new Vector2(_width - _width / GoldenMath.Ratio - dW, -dH);
                        vertices[9] = new Vector2(_width - _width / GoldenMath.Ratio - dW, _height - dH);
                        break;
                }
                return vertices;
            }
        }

        public PrimitiveType PrimitiveType => PrimitiveType.Lines;
        public Color Color => Color.Black;
        public bool Highlight { get; set; }
        private float _highlight; // Alpha: 0 1 0 1 0

        public void Draw()
        {
            GL.Color3(Color);
            GL.Begin(PrimitiveType);

            switch (_side)
            {
                case Side.Right:
                    foreach (Vector2 vertex in Vertices)
                        GL.Vertex2(vertex);
                    break;
                case Side.Bottom: // Draw only mid line
                    GL.Vertex2(Vertices[8]);
                    GL.Vertex2(Vertices[9]);
                    break;
                case Side.Left:
                    GL.Vertex2(Vertices[8]);
                    GL.Vertex2(Vertices[9]);
                    break;
                case Side.Top:
                    GL.Vertex2(Vertices[8]);
                    GL.Vertex2(Vertices[9]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

//            foreach (Vector2 vertex in Vertices)
//                GL.Vertex2(vertex);

            GL.End();

            if (!Highlight) return;
            if (_highlight >= 1f)
            {
                Highlight = false;
                return;
            }

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Begin(PrimitiveType.Quads);
            
            GL.Color4(255, 0, 0, 255);
            GL.Vertex2(Vertices[0]);
            GL.Vertex2(Vertices[5]);
            GL.Vertex2(Vertices[9]);
            GL.Vertex2(Vertices[8]);

            GL.Color4(0, 255, 0, 255);
            GL.Vertex2(Vertices[0]);
            GL.Vertex2(Vertices[6]);
            GL.Vertex2(Vertices[1]);
            GL.Vertex2(Vertices[9]);

            _highlight += Window.DeltaTime * .2f / 5f;
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
