using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace UDA2018.GoldenRatio.Graphics
{
    public interface IDrawable
    {
        Vector2 CenterPosition { get; }
        float X { get; }
        float Y { get; }
        float Width { get; }
        float Height { get; }
        Vector2[] Vertices { get; }
        PrimitiveType PrimitiveType { get; }
        Color Color { get; }

        void Draw();
    }
}
