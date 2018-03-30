using System.Data;
using System.Runtime.InteropServices;
using OpenTK;

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
            _hue = (r + g + b) * 6;
            _r = r;
            _g = g;
            _b = b;
        }

        public CColor(float hue)
        {
            _r = _g = _b = 0;
            _hue = hue;
        }

        public static CColor Lerp(CColor c1, CColor c2, float q)
        {
            return new CColor(
                GoldenMath.Lerp(c1._r, c2._r, q),
                GoldenMath.Lerp(c1._g, c2._g, q),
                GoldenMath.Lerp(c1._b, c2._b, q)
            );
        }

        private void FromCColor(CColor color)
        {
            _r = color._r;
            _g = color._g;
            _b = color._b;
        }

        private float _hue;
        public void Fade()
        {
            _hue += Window.DeltaTime * 2;
            float t = _hue % 1;
            switch ((int)_hue)
            {
                case 0:
                    FromCColor(Lerp(new CColor(0.54f, 0f, 1f), new CColor(1f, 0f, 0f), t));
                    break;
                case 1:
                    FromCColor(Lerp(new CColor(1f, 0f, 0f), new CColor(1f, 0.45f, 0f), t));
                    break;
                case 2:
                    FromCColor(Lerp(new CColor(1f, 0.45f, 0f), new CColor(1f, 1f, 0f), t));
                    break;
                case 3:
                    FromCColor(Lerp(new CColor(1f, 1f, 0f), new CColor(0f, 1f, 0f), t));
                    break;
                case 4:
                    FromCColor(Lerp(new CColor(0f, 1f, 0f), new CColor(0f, 1f, 1f), t));
                    break;
                case 5:
                    FromCColor(Lerp(new CColor(0f, 1f, 1f), new CColor(0f, 0f, 1f), t));
                    break;
                case 6:
                    FromCColor(Lerp(new CColor(0f, 0f, 1f), new CColor(0.54f, 0f, 1f), t));
                    break;
                default:
                    _hue = 0;
                    break;
            }
        }


        public static implicit operator Vector3(CColor color)
        {
            return new Vector3(color._r, color._g, color._b);
        }

        public override string ToString() => $"{{{_r:0.00}, {_g:0.00}, {_b:0.00}, {_hue:0.00}}}";

        public static CColor Empty => new CColor();
        public static CColor Cyan => new CColor(0, 1, 1);
        public static CColor Teal => new CColor(0, 0.5f, 0.5f);
        public static CColor Red => new CColor(1, 0, 0);
        public static CColor Green => new CColor(0, 1, 0);
        public static CColor Blue => new CColor(0, 0, 1);
        public static CColor Black => new CColor(0, 0, 0);
        public static CColor White => new CColor(1, 1, 1);
    }
}
