using System.Runtime.InteropServices;

namespace UDA2018.GoldenRatio.Graphics
{
    public struct CColor
    {
        public static int Size = Marshal.SizeOf<CColor>();

        private float _r;
        private float _g;
        private float _b;

        public CColor(float r, float g, float b)
        {
            _stage = 0;
            _r = r;
            _g = g;
            _b = b;
        }

        public static CColor Lerp(CColor c1, CColor c2, float q)
        {
            return new CColor(
                GoldenMath.Lerp(c1._r, c2._r, q),
                GoldenMath.Lerp(c1._g, c2._g, q), 
                GoldenMath.Lerp(c1._b, c2._b, q)
            );
        }

        private float _stage;
        private void Sum(float sum)
        {
            switch (_stage)
            {
                case 0:
                    _r += sum; // +RED
                    if (_r >= 1f)
                        _stage++;
                    break;
                case 1:
                    _b += sum; // +BLUE
                    if (_b >= 1f)
                        _stage++;
                    break;
                case 2:
                    _r -= sum; // -RED
                    if (_r <= 0f)
                        _stage++;
                    break;
                case 3:
                    _g += sum; // +GREEN
                    if (_g >= 1f)
                        _stage++;
                    break;
                case 4:
                    _b -= sum; // -BLUE
                    if (_b <= 0f)
                        _stage++;
                    break;
                case 5:
                    _r += sum; // +RED
                    if (_r >= 1f)
                        _stage++;
                    break;
                case 6:
                    _g -= sum; // -GREEN
                    if (_g <= 0f)
                        _stage++;
                    break;
                case 7:
                    _r += sum; // +RED
                    if (_r >= 1f)
                        _stage = 0;
                    break;
            }

            GoldenMath.Clamp(ref _r, 0, 1);
            GoldenMath.Clamp(ref _g, 0, 1);
            GoldenMath.Clamp(ref _b, 0, 1);
        }

        public static CColor operator +(CColor color, float sum)
        {
            color.Sum(sum);
            return color;
        }

        public static CColor Red => new CColor(1, 0, 0);
        public static CColor Green => new CColor(0, 1, 0);
        public static CColor Blue => new CColor(0, 0, 1);
        public static CColor Black => new CColor(0, 0, 0);
        public static CColor White => new CColor(1, 1, 1);
    }
}
