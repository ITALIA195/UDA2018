using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace GoldenRatio.Graphics
{
    public class GoldenRectangle
    {
        private readonly Shaders _rectangleProgram;
        private readonly Shaders _squareProgram;

        private int _vertexBufferObject;
        
        private int _rectangleVertexArray;
        private int _rectangleElementArray;
        
        private int _squareVertexArray;
        private int _squareElementArray;
        
        private Side _side;
        private float _x;
        private float _y;
        private float _width;
        private float _height;

        private static Vector2[] GetVertices(Side side, float x, float y, float width, float height)
        {
            var vertices = new Vector2[6];
            float dW = width / 2f;
            float dH = height / 2f;
            vertices[0] = new Vector2(-dW + x, +dH + y); // Top Left
            vertices[1] = new Vector2(+dW + x, +dH + y); // Top Right
            vertices[2] = new Vector2(+dW + x, -dH + y); // Bottom Right
            vertices[3] = new Vector2(-dW + x, -dH + y); // Bottom Left
            
            switch (side)
            {
                case Side.Right:
                    vertices[4] = new Vector2(width / GoldenMath.Ratio - dW + x, +dH + y); // Top
                    vertices[5] = new Vector2(width / GoldenMath.Ratio - dW + x, -dH + y); // Bottom
                    break;
                case Side.Bottom:
                    vertices[4] = new Vector2(-dW + x, dH - height / GoldenMath.Ratio + y); // Left
                    vertices[5] = new Vector2(+dW + x, dH - height / GoldenMath.Ratio + y); // Right
                    break;
                case Side.Left:
                    vertices[4] = new Vector2(dW - width / GoldenMath.Ratio + x, +dH + y); // Top
                    vertices[5] = new Vector2(dW - width / GoldenMath.Ratio + x, -dH + y); // Bottom
                    break;
                case Side.Top:
                    vertices[4] = new Vector2(-dW + x, height / GoldenMath.Ratio - dH + y); // Left
                    vertices[5] = new Vector2(+dW + x, height / GoldenMath.Ratio - dH + y); // Right
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return vertices;
        }

        public void BindData()
        {
            // VBO data (Used by both Rectangle and Square)
            var vertices = GetVertices(_side, _x, _y, _width, _height);

            // Bind VBO
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vector2.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
            
            // ===============
            // == Rectangle ==
            // ===============
            
            // EBO indexes
            var indices = new uint[]
            {
                0, 1, // Top
                1, 2, // Right
                2, 3, // Bottom
                3, 0, // Left
                
                4, 5  // Line
            };

            // Bind EBO
            _rectangleElementArray = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _rectangleElementArray);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            
            // Bind VAO
            _rectangleVertexArray = GL.GenVertexArray();
            GL.BindVertexArray(_rectangleVertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _rectangleElementArray);
            
            // ============
            // == Square ==
            // ============

            switch (_side)
            {
                case Side.Right:
                    indices = new uint[] { 4, 5, 2, 1 };
                    break;
                case Side.Bottom:
                    indices = new uint[] { 4, 5, 2, 3 };
                    break;
                case Side.Left:
                    indices = new uint[] { 4, 5, 3, 0 };
                    break;
                case Side.Top:
                    indices = new uint[] { 4, 5, 1, 0 };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Bind EBO
            _squareElementArray = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _squareElementArray);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            
            // Bind VAO
            _squareVertexArray = GL.GenVertexArray();
            GL.BindVertexArray(_squareVertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _squareElementArray);
        }
        
        public GoldenRectangle(Shaders rectangleProgram, Shaders squareProgram)
        {
            _rectangleProgram = rectangleProgram;
            _squareProgram = squareProgram;
        }

        public void Draw()
        {
            if (_highlighted)
                DrawQuads();

            _rectangleProgram.Bind();

            GL.BindVertexArray(_rectangleVertexArray);
            GL.DrawElements(PrimitiveType.Lines, 10, DrawElementsType.UnsignedInt, 0);
        }

        private void DrawQuads()
        {
            _squareProgram.Bind();

            GL.BindVertexArray(_squareVertexArray);
            GL.DrawElements(PrimitiveType.Quads, 4, DrawElementsType.UnsignedInt, 0);
        }

        private bool _highlighted;
        private float _alpha;
        public void Update(int squareAlpha)
        {
            if (!_highlighted)
                return;
            
            _squareProgram.Bind();
            GL.Uniform1(squareAlpha, Alpha);
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

        public GoldenRectangle Next
        {
            get
            {
                switch (_side)
                {
                    case Side.Right:
                        return new GoldenRectangle(_rectangleProgram, _squareProgram)
                        {
                            Side = Side.Bottom,
                            X = _x + _width / 2f - (_width - _height) / 2f,
                            Y = _y,
                            Height = _height
                        };
                    case Side.Bottom:
                        return new GoldenRectangle(_rectangleProgram, _squareProgram)
                        {
                            Side = Side.Left,
                            X = _x,
                            Y = _y - _height / 2f + (_height - _width) / 2f,
                            Width = _width
                        };
                    case Side.Left:
                        return new GoldenRectangle(_rectangleProgram, _squareProgram)
                        {
                            Side = Side.Top,
                            X = _x - _width / 2f + (_width - _height) / 2f,
                            Y = _y,
                            Height = _height
                        };
                    case Side.Top:
                        return new GoldenRectangle(_rectangleProgram, _squareProgram)
                        {
                            Side = Side.Right,
                            X = _x,
                            Y = _y + _height / 2f - (_height - _width) / 2f,
                            Width = _width
                        };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_side));
                }
            }
        }


        public Vector2 OffsetPosition
        {
            get
            {
                switch (_side)
                {
                    case Side.Right:
                        return new Vector2((_width - _width / 2f - (_width - _height) / 2f) * Window.OneOverScreenRatio, 0);
                    case Side.Bottom:
                        return new Vector2(0, -(_height - _height / 2f - (_height - _width) / 2f));
                    case Side.Left:
                        return new Vector2(-(_width - _width / 2f - (_width - _height) / 2f) * Window.OneOverScreenRatio, 0);
                    case Side.Top:
                        return new Vector2(0, _height - _height / 2f - (_height - _width) / 2f);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_side));
                }
            }
        }

        public Rectangle SubRectangle
        {
            get
            {
                switch (_side)
                {
                    case Side.Right:
                        return new Rectangle(_x + _height, _y, _width - _height, _height);
                    case Side.Bottom:
                        return new Rectangle(_x, _y + _width, _width, _height - _width);
                    case Side.Left:
                        return new Rectangle(_x, _y, _width - _height, _height);
                    case Side.Top:
                        return new Rectangle(_x, _y, _width, _height - _width);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_side));
                }
            }
        }
        
        public Side Side
        {
            set => _side = value;
        }

        public float X
        {
            set => _x = value;
        }

        public float Y
        {
            set => _y = value;
        }

        public float Width
        {
            set
            {
                _width = value;
                _height = value;
                
                if ((_side & Side.Vertical) != 0)
                    _height *= GoldenMath.Ratio;
                else
                    _height /= GoldenMath.Ratio;
            }
        }
        
        public float Height
        {
            set
            {
                _width = value;
                _height = value;
                
                if ((_side & Side.Horizontal) != 0)
                    _width *= GoldenMath.Ratio;
                else
                    _width /= GoldenMath.Ratio;
            }
        }
    }
}
