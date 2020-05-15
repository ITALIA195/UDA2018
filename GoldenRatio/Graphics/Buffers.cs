using System;
using OpenToolkit.Graphics.OpenGL4;

namespace GoldenRatio.Graphics
{
    public class Buffers
    {
        private readonly int _vertexBuffer = -1;
        private readonly int _elementBuffer = -1;
        private readonly int _colorBuffer = -1;
        private readonly int _textureBuffer = -1;
        private readonly int _vertexArray = -1;

        public Buffers(Buffer buffer)
        {
            if ((buffer & Buffer.VertexBuffer) != 0)
                _vertexBuffer = GL.GenBuffer();
            if ((buffer & Buffer.ElementBuffer) != 0)
                _elementBuffer = GL.GenBuffer();
            if ((buffer & Buffer.ColorBuffer) != 0)
                _colorBuffer = GL.GenBuffer();
            if ((buffer & Buffer.TextureBuffer) != 0)
                _textureBuffer = GL.GenBuffer();
            if ((buffer & Buffer.VertexArray) != 0)
                _vertexArray = GL.GenVertexArray();
        }

        public int VertexBuffer
        {
            get
            {
                if (_vertexBuffer == -1)
                    throw new Exception("Accessing nonexistent buffer");
                return _vertexBuffer;
            }
        }

        public int ColorBuffer
        {
            get
            {
                if (_colorBuffer == -1)
                    throw new Exception("Accessing nonexistent buffer");
                return _colorBuffer;
            }
        }

        public int ElementBuffer
        {
            get
            {
                if (_elementBuffer == -1)
                    throw new Exception("Accessing nonexistent buffer");
                return _elementBuffer;
            }
        }

        public int TextureBuffer
        {
            get
            {
                if (_textureBuffer == -1)
                    throw new Exception("Accessing nonexistent buffer");
                return _textureBuffer;
            }
        }

        public int VertexArray
        {
            get
            {
                if (_vertexArray == -1)
                    throw new Exception("Accessing nonexistent buffer");
                return _vertexArray;
            }
        }
    }

    [Flags]
    public enum Buffer
    {
        VertexBuffer = 1 << 1,
        ElementBuffer = 1 << 2,
        ColorBuffer = 1 << 3,
        TextureBuffer = 1 << 4,
        VertexArray = 1 << 5,

        VBO = VertexBuffer,
        EBO = ElementBuffer,
        CBO = ColorBuffer,
        TBO = TextureBuffer,
        VAO = VertexArray,

        VertexElement = VBO | EBO,
        VertElementsCol = VBO | EBO | CBO
}
}