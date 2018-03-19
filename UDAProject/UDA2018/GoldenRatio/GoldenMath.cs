using OpenTK;

namespace UDA2018.GoldenRatio
{
    public class GoldenMath
    {
        public const float Ratio = 1.61803398874989484820458683436f;

        public static void Clamp(ref float a, float min, float max)
        {
            a = a > max ? max : (a < min ? min : a);
        }

        public static float Lerp(float a, float b, float t)
        {
            t = t < 0f ? 0f : (t > 1f ? 1f : t);
            return a + t * (b - a);
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            t = t < 0f ? 0f : (t > 1f ? 1f : t);
            return new Vector2((a.X + (b.X - a.X) * t), (a.Y + (b.Y - a.Y) * t));
        }

        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        public static bool IsInside(Vector2 point, Rect rect)
        {
            if (point.X < rect.X)
                return false;
            if (point.X > rect.X + rect.Width)
                return false;
            if (point.Y < rect.Y)
                return false;
            if (point.Y > rect.Y + rect.Height)
                return false;
            return true;
        }
    }
}
