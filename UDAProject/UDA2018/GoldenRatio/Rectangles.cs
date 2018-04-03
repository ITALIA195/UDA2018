using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using UDA2018.GoldenRatio.Graphics;

namespace UDA2018.GoldenRatio
{
    public class Rectangles : List<GoldenRectangle>
    {
        private readonly Tracker<GoldenRectangle> _tracker;
        private const int RectanglesNumber = 16;

        public Rectangles()
        {
            GoldenRectangle.ProgramID = GL.CreateProgram();
            GoldenRectangle.Shader = new Shaders("vertex", "fragment");
            GoldenRectangle.Shader.LinkProgram(GoldenRectangle.ProgramID);
            GoldenRectangle.SquareShader = new Shaders("squareVertex", "squareFragment");
            while (Count < RectanglesNumber)
            {
                if (this.LastOrDefault() is GoldenRectangle rectangle)
                    base.Add(rectangle.Next);
                else
                    base.Add(new GoldenRectangle(Side.Right, null, 1.9f));
            }

            _tracker = new Tracker<GoldenRectangle>(this);
        }

        public void Update()
        {
            foreach (GoldenRectangle rectangle in this)
                rectangle.Update();

            _tracker.Update();

            GoldenRectangle.GradientStartColor.Fade();
            GoldenRectangle.GradientEndColor.Fade();

            RemoveAll(x => !x.IsVisible);
            if (Count < RectanglesNumber)
                Add(this.LastOrDefault()?.Next);
        }

        public new void Add(GoldenRectangle rectangle)
        {
            if (rectangle == null) return;
            base.Add(rectangle);
            _tracker.Enqueue(rectangle);
        }
    }
}
