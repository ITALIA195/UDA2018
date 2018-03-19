//using System;
//using System.CodeDom;
//using OpenTK;
//
//namespace UDA2018.GoldenRatio
//{
//    public struct GoldenRectangle
//    {
//        public const float Ratio = 1.6180339887f;
//
//        private readonly Side _side;
//        private readonly float _x;
//        private readonly float _y;
//        private float _width;
//        private float _height;
//
//        public GoldenRectangle NextGoldenRectangle
//        {
//            get
//            {
//                switch (_side)
//                {
//                    case Side.Right:
//                        return new GoldenRectangle(Side.Bottom, _x + _height, Y, null, _height);
//                    case Side.Bottom:
//                        return new GoldenRectangle(Side.Left, _x, _y + _width, _width, null);
//                    case Side.Left:
//                        return new GoldenRectangle(Side.Top, _x, _y, null, _height);
//                    case Side.Top:
//                        return new GoldenRectangle(Side.Right, _x, _y, _width, null);
//                    default:
//                        throw new ArgumentOutOfRangeException(nameof(_side));
//                }
//            }
//        }
//
//        public Rect SubRect
//        {
//            get
//            {
//                switch (_side)
//                {
//                    case Side.Right:
//                        return new Rect(_x + _height, Y, _height / Ratio, _height);
//                    case Side.Bottom:
//                        return new Rect(_x, _y + _width, _width, _width / Ratio);
//                    case Side.Left:
//                        return new Rect(_x, _y, _height / Ratio, _height);
//                    case Side.Top:
//                        return new Rect(_x, _y, _width, _width / Ratio);
//                    default:
//                        throw new ArgumentOutOfRangeException(nameof(_side));
//                }
//            }
//        }
//
//        public GoldenRectangle(Side side, float x, float y, float? width, float? height)
//        {
//            if (!width.HasValue && !height.HasValue) throw new ArgumentNullException($"The arguments {nameof(width)} and {nameof(height)} cannot be null at the same time.");
//            _side = side;
//            _x = x;
//            _y = y;
//            if (width.HasValue)
//            {
//                _width = width.Value;
//                _height = ((int) side & 1) == 0 ? _width / Ratio : _width * Ratio;
//            }
//            else
//            {
//                _height = height.Value;
//                _width = ((int)side & 1) == 0 ? _height * Ratio : _height / Ratio;
//            }
//        }
//
//        public GoldenRectangle(Side side, float? width, float? height)
//        {
//            if (!width.HasValue && !height.HasValue) throw new ArgumentNullException($"The arguments {nameof(width)} and {nameof(height)} cannot be null at the same time.");
//            _side = side;
//            if (width.HasValue)
//            {
//                _width = width.Value;
//                _height = ((int)side & 1) == 0 ? _width / Ratio : _width * Ratio;
//            }
//            else
//            {
//                _height = height.Value;
//                _width = ((int)side & 1) == 0 ? _height * Ratio : _height / Ratio;
//            }
//            _x = (Window.ScreenWidth - _width) / 2f;
//            _y = (Window.ScreenHeight - _height) / 2f;
//        }
//
//        public Side Side => _side;
//        public float X => _x;
//        public float Y => _y;
//        public Vector2 Position => new Vector2(X, Y);
//        public Vector2 CenterPosition => new Vector2(X + _width / 2f, Y + _height / 2f);
//
//        public float Width
//        {
//            get => _width;
//            set
//            {
//                if (((int)Side & 1) == 0) // Left or Right
//                {
//                    _width = value;
//                    _height = value / Ratio;
//                }
//                else // Top or Bottom
//                {
//                    _width = value;
//                    _height = value * Ratio;
//                }
//            }
//        }
//
//        public float Height
//        {
//            get => _height;
//            set
//            {
//                if (((int)Side & 1) == 0) // Left or Right
//                {
//                    _width = value * Ratio;
//                    _height = value;
//                }
//                else // Top or Bottom
//                {
//                    _width = value / Ratio;
//                    _height = value;
//                }
//            }
//        }
//    }
//
//    public enum Side
//    {
//        Right,
//        Bottom,
//        Left,
//        Top
//    }
//}
