using System;
using OpenTK.Graphics.OpenGL4;

namespace GoldenRatio.Graphics
{
    public class Buffers
    {
        public int VertexBuffer { get; }
        public int ColorBuffer { get; }
        public int IndexBuffer { get; }
        public int TextureBuffer { get; }

        public Buffers(Buffer buffers)
        {
            if ((buffers & Buffer.Color) != 0)
                ColorBuffer = GL.GenBuffer();
            if ((buffers & Buffer.Vertex) != 0)
                VertexBuffer = GL.GenBuffer();
            if ((buffers & Buffer.Index) != 0)
                IndexBuffer = GL.GenBuffer();
            if ((buffers & Buffer.Texture) != 0)
                TextureBuffer = GL.GenBuffer();
        }
    }

    [Flags]
    public enum Buffer
    {
        Vertex = 1 << 0,
        Color = 1 << 1,
        Index = 1 << 2,
        Texture = 1 << 3
    }
}
