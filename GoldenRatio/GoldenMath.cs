namespace GoldenRatio
{
    public static class GoldenMath
    {
        /// <summary>
        /// Golden ratio
        /// </summary>
        public const float Ratio = 1.61803398874989484820458683436f;

        /// <summary>
        /// One over the golden ratio
        /// </summary>
        public const float OneOverRatio = 0.61803398874989484820458683436779f;

        public static void Clamp(ref float a, float min, float max)
        {
            a = a > max ? max : (a < min ? min : a);
        }

        public static float Lerp(float a, float b, float t)
        {
            Clamp(ref t, 0f, 1f);
            return a + t * (b - a);
        }

        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }
    }
}
