using System.Runtime.InteropServices;
using OpenTK;

namespace GoldenRatio.Graphics
{
    public struct CColor
    {
        public static readonly int Size = Marshal.SizeOf<CColor>();

        private float _r;
        private float _g;
        private float _b;

        public CColor(float r, float g, float b)
        {
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

        public void Fade()
        {}

        public static implicit operator Vector3(CColor color)
        {
            return new Vector3(color._r, color._g, color._b);
        }
    }
}
