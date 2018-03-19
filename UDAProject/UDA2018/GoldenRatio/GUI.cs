using OpenTK.Graphics.OpenGL;

namespace UDA2018.GoldenRatio
{
    public static class GUI
    {
        private static float Width;
        private static float Height;

        public static void Begin(int width, int height)
        {
            Width = width;
            Height = height;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.Ortho(left: -Width / 2f, right: Width / 2f, bottom: Height / 2f, top: -Height / 2f, zNear: 0, zFar: 1);
//            GL.Ortho(0f, Width, Height, 0f, 0f, 1f);
        }

//        public static Drawable CreateDrawable(GoldenRectangle rect, Color color)
//        {
//            Vector2[] vertices = new Vector2[10];
//
//            vertices[0] = new Vector2(0, 0);
//            vertices[1] = new Vector2(rect.Width, 0);
//            vertices[2] = new Vector2(rect.Width, 0);
//            vertices[3] = new Vector2(rect.Width, rect.Height);
//            vertices[4] = new Vector2(rect.Width, rect.Height);
//            vertices[5] = new Vector2(0, rect.Height);
//            vertices[6] = new Vector2(0, rect.Height);
//            vertices[7] = new Vector2(0, 0);
//
//            switch (rect.Side)
//            {
//                case Side.Top:
//                    vertices[8] = new Vector2(0, rect.Height - rect.Width);
//                    vertices[9] = new Vector2(rect.Width, rect.Height - rect.Width);
//                    break;
//                case Side.Bottom:
//                    vertices[8] = new Vector2(0, rect.Width);
//                    vertices[9] = new Vector2(rect.Width, rect.Width);
//                    break;
//                case Side.Right:
//                    vertices[8] = new Vector2(rect.Height, 0); // x+h y
//                    vertices[9] = new Vector2(rect.Height, rect.Height); // x+h y+h
//                    break;
//                case Side.Left:
//                    vertices[8] = new Vector2(rect.Width - rect.Height, 0); // Width * Ratio, 0
//                    vertices[9] = new Vector2(rect.Width - rect.Height, rect.Height); // Width * Ratio, Height
//                    break;
//            }
//
//            for (int i = 0; i < vertices.Length; i++)
//                vertices[i] += new Vector2(rect.Position.X, rect.Position.Y);
//
//            return new Drawable
//            {
//                GoldenRectangle = rect,
//                Color = color,
//                PrimitiveType = PrimitiveType.Lines,
//                Vertices = vertices
//            };
//        }

//        public static Drawable CreateDrawable(Rect rect, Color color, Vector2 scale)
//        {
//            Vector2[] vertices = {
//                new Vector2(0, 0),
//                new Vector2(1, 0),
//                new Vector2(1, 1),
//                new Vector2(0, 1)
//            };
//
//            for (int i = 0; i < vertices.Length; i++)
//            {
//                vertices[i].X *= rect.Width;
//                vertices[i].Y *= rect.Height;
//                vertices[i] *= scale;
//                vertices[i] += new Vector2(rect.X, rect.Y);
//            }
//
//            return new Drawable
//            {
//                Color = color,
//                PrimitiveType = PrimitiveType.Quads,
//                Vertices = vertices
//            };
//        }
    }
}
