using System;
using Hawk.Framework;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

namespace GoldenRatio.Objects
{
    public class GoldenRectangle
    {
        private const float GoldenRatio = 1.618033f;
        
        private readonly Program _rectangleProgram;
        private readonly Program _squareProgram;

        private int _sharedVertexBufferObject;

        private GLObjects _rectangleBuffers;
        private GLObjects _squareBuffers;

        private float _width;
        private float _height;
        
        private bool _highlighted;

        private Vector2[] _vertices;
        
        private uint[] _rectangleIndices;
        private uint[] _squareIndices;
   
        public GoldenRectangle(Program rectangleProgram, Program squareProgram)
        {
            _rectangleProgram = rectangleProgram;
            _squareProgram = squareProgram;
        }
        
        public Side Side { get; set; }
        
        public float X { get; set; }
        
        public float Y { get; set; }

        public float Width
        {
            set
            {
                _width = value;
                _height = value;
                
                if ((Side & Side.Vertical) != 0)
                    _height *= GoldenRatio;
                else
                    _height /= GoldenRatio;
            }
        }
        
        public float Height
        {
            set
            {
                _width = value;
                _height = value;
                
                if ((Side & Side.Horizontal) != 0)
                    _width *= GoldenRatio;
                else
                    _width /= GoldenRatio;
            }
        }
        
        
        public void Render()
        {
            if (true)
            {
                _squareProgram.Bind();

                GL.BindBuffer(BufferTarget.ArrayBuffer, _sharedVertexBufferObject);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _squareBuffers.ElementBuffer);
                
                GL.DrawElements(PrimitiveType.TriangleFan, _squareIndices.Length, DrawElementsType.UnsignedInt, 0);
            }

            _rectangleProgram.Bind();
            
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _sharedVertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _rectangleBuffers.ElementBuffer);
            
            GL.DrawElements(PrimitiveType.Lines, _rectangleIndices.Length, DrawElementsType.UnsignedInt, 0);
            
            GL.DisableVertexAttribArray(0);
        }

        private static Vector2[] GetVertices(Side side, float x, float y, float width, float height)
        {
            var vertices = new Vector2[6];
            var dW = width / 2f;
            var dH = height / 2f;
            vertices[0] = new Vector2(-dW + x, +dH + y); // Top Left
            vertices[1] = new Vector2(+dW + x, +dH + y); // Top Right
            vertices[2] = new Vector2(+dW + x, -dH + y); // Bottom Right
            vertices[3] = new Vector2(-dW + x, -dH + y); // Bottom Left
            
            switch (side)
            {
                case Side.Right:
                    vertices[4] = new Vector2(-dW + width / GoldenRatio + x, +dH + y); // Top
                    vertices[5] = new Vector2(-dW + width / GoldenRatio + x, -dH + y); // Bottom
                    break;
                case Side.Left:
                    vertices[4] = new Vector2(+dW - width / GoldenRatio + x, +dH + y); // Top
                    vertices[5] = new Vector2(+dW - width / GoldenRatio + x, -dH + y); // Bottom
                    break;
                case Side.Bottom:
                    vertices[4] = new Vector2(-dW + x, dH - height / GoldenRatio + y); // Left
                    vertices[5] = new Vector2(+dW + x, dH - height / GoldenRatio + y); // Right
                    break;
                case Side.Top:
                    vertices[4] = new Vector2(-dW + x, height / GoldenRatio - dH + y); // Left
                    vertices[5] = new Vector2(+dW + x, height / GoldenRatio - dH + y); // Right
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return vertices;
        }
        
        public void BindData()
        {
            // VBO data (Used by both Rectangle and Square)
            _vertices = GetVertices(Side, X, Y, _width, _height);

            // Bind VBO
            _sharedVertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _sharedVertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * Vector2.SizeInBytes, _vertices, BufferUsageHint.StaticDraw);
            
            // ===============
            // == Rectangle ==
            // ===============
            
            _rectangleBuffers = new GLObjects(GLObject.ElementBuffer);
            
            // EBO indexes
            _rectangleIndices = new uint[]
            {
                0, 1, // Top
                1, 2, // Right
                2, 3, // Bottom
                3, 0, // Left
                
                4, 5, // Line
            };

            // Bind EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _rectangleBuffers.ElementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _rectangleIndices.Length * sizeof(uint), _rectangleIndices, BufferUsageHint.StaticDraw);

            // ============
            // == Square ==
            // ============

            _squareBuffers = new GLObjects(GLObject.ElementBuffer | GLObject.VertexArray);
            
            _squareIndices = Side switch
            {
                Side.Right => new uint[] {4, 5, 2, 1},
                Side.Bottom => new uint[] {4, 5, 2, 3},
                Side.Left => new uint[] {4, 5, 3, 0},
                Side.Top => new uint[] {4, 5, 1, 0},
                _ => throw new Exception("Invalid side")
            };
            
            // Bind EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _squareBuffers.ElementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _squareIndices.Length * sizeof(uint), _squareIndices, BufferUsageHint.StaticDraw);
            
            // Bind VAO
            GL.BindVertexArray(_squareBuffers.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _sharedVertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _squareBuffers.ElementBuffer);
            
            _squareProgram.Bind();
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);
        }

        public GoldenRectangle Next
        {
            get
            { 
                switch (Side)
                {
                    case Side.Right:
                        return new GoldenRectangle(_rectangleProgram, _squareProgram)
                        {
                            Side = Side.Bottom,
                            X = X + _width / 2f - (_width - _height) / 2f,
                            Y = Y,
                            Height = _height
                        };
                    case Side.Bottom:
                        return new GoldenRectangle(_rectangleProgram, _squareProgram)
                        {
                            Side = Side.Left,
                            X = X,
                            Y = Y - _height / 2f + (_height - _width) / 2f,
                            Width = _width
                        };
                    case Side.Left:
                        return new GoldenRectangle(_rectangleProgram, _squareProgram)
                        {
                            Side = Side.Top,
                            X = X - _width / 2f + (_width - _height) / 2f,
                            Y = Y,
                            Height = _height
                        };
                    case Side.Top:
                        return new GoldenRectangle(_rectangleProgram, _squareProgram)
                        {
                            Side = Side.Right,
                            X = X,
                            Y = Y + _height / 2f - (_height - _width) / 2f,
                            Width = _width
                        };
                    default:
                        throw new ArgumentOutOfRangeException(nameof(Side));
                }
            }
        }

        public Vector2 OffsetPosition
        {
            get
            {
                switch (Side)
                {
                    case Side.Right:
                        return new Vector2((_width - _width / 2f - (_width - _height) / 2f) / Window.ScreenRatio, 0);
                    case Side.Bottom:
                        return new Vector2(0, -(_height - _height / 2f - (_height - _width) / 2f));
                    case Side.Left:
                        return new Vector2(-(_width - _width / 2f - (_width - _height) / 2f) / Window.ScreenRatio, 0);
                    case Side.Top:
                        return new Vector2(0, _height - _height / 2f - (_height - _width) / 2f);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(Side));
                }
            }
        }

        public Vector4 SubRectangle
        {
            get
            {
                switch (Side)
                {
                    case Side.Right:
                        return new Vector4(X + _height, Y, _width - _height, _height);
                    case Side.Bottom:
                        return new Vector4(X, Y + _width, _width, _height - _width);
                    case Side.Left:
                        return new Vector4(X, Y, _width - _height, _height);
                    case Side.Top:
                        return new Vector4(X, Y, _width, _height - _width);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(Side));
                }
            }
        }
    }
    
    [Flags]
    public enum Side
    {
        Top    = 1,
        Bottom = 1 << 1,
        Right  = 1 << 2,
        Left   = 1 << 3,
        
        Vertical   = Top | Bottom,
        Horizontal = Left | Right,
    }
}
