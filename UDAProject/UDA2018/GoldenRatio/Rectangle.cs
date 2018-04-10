namespace UDA2018.GoldenRatio
{
    public struct Rectangle
    {
        private readonly float _x;
        private readonly float _y;
        private readonly float _width;
        private readonly float _height;

        public Rectangle(int x, int y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public Rectangle(float x, float y, float width, float height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public float X => _x;
        public float Y => _y;
        public float Width => _width;
        public float Height => _height;
        public float Left => _x;
        public float Top => _y;
        public float Bottom => _y + _height;
        public float Right => _x + _width;

        public static implicit operator Rectangle(System.Drawing.Rectangle rectangle)
        {
            return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}
