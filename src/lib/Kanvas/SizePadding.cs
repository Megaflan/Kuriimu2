namespace Kanvas
{
    public static class SizePadding
    {
        /// <summary>
        /// Rounds up a given value to the next multiple of two.
        /// </summary>
        /// <param name="value">The value to round up.</param>
        /// <param name="steps">The amount of power of two's to round up to.</param>
        /// <returns>The rounded up value.</returns>
        /// <remarks>Eg. 34 with 1 step is 64, 121 with 2 steps is 256.</remarks>
        public static int MultipleOfTwo(int value, int steps)
        {
            return 2 << (int)Math.Log(value - 1, 2) << (steps - 1);
        }

        /// <summary>
        /// Rounds up a given value to the next value dividable by <param name="multiple" />.
        /// </summary>
        /// <param name="value">The value to round up.</param>
        /// <param name="multiple">The multiple to round up to.</param>
        /// <returns>The rounded up value.</returns>
        /// <remarks>Eg. 34 with multiple 3 is 36, 121 with multiple 6 is 126.</remarks>
        public static int Multiple(int value, int multiple)
        {
            return (value + (multiple - 1)) / multiple * multiple;
        }
    }
}
