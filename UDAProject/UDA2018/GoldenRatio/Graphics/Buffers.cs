using System;
using OpenTK.Graphics.OpenGL;

namespace UDA2018.GoldenRatio.Graphics
{
    public struct Buffers
    {
        public int VertexBuffer { set;get; }
        public int ColorBuffer { set;get; }
        public int IndexBuffer { set;get; }
        public int TextureBuffer { set;get; }

        public Buffers(Buffer buffers)
        {
            VertexBuffer = ColorBuffer = IndexBuffer = TextureBuffer = -1;
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
        Vertex = 0b0001,
        Color = 0b0010,
        Index = 0b0100,
        Texture = 0b1000
    }
}
