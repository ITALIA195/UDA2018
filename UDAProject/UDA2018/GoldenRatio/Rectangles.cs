using System.Collections.Generic;
using System.Linq;
using UDA2018.GoldenRatio.Graphics;

namespace UDA2018.GoldenRatio
{
    public class Rectangles : List<GoldenRectangle>
    {
        private readonly Tracker<GoldenRectangle> _tracker;
        private const int RectanglesNumber = 16;

        public Rectangles()
        {
            while (Count < RectanglesNumber)
            {
                if (this.LastOrDefault() is GoldenRectangle rectangle)
                    Add(rectangle.Next);
                else
                    Add(new GoldenRectangle(Side.Right, null, Window.Height - 20));
            }

            _tracker = new Tracker<GoldenRectangle>(this);
        }

        public void Update()
        {
            foreach (GoldenRectangle rectangle in this)
                rectangle.Update();

            _tracker.Update();
        }
    }
}
