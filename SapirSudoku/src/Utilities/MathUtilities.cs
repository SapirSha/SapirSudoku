using SapirSudoku.src.Exceptions;

namespace SapirSudoku.src.Utilities
{
    /// <summary>
    /// Math Utilities Class to help with math
    /// </summary>
    public static class MathUtilities
    {
        /// <summary>
        /// A function that returns the two closest numbers that thier multiplication is value number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static (int, int) ClosestMultiplications(int number)
        {
            if (number == 0) return (0, 0);

            // if less then zero, return the result of positive, but one of the numbers are negative
            if (number < 0)
            {
                (int smaller, int bigger) negative = ClosestMultiplications(number * -1);
                return (-1 * negative.smaller, negative.bigger);
            }

            // start from square of number until found
            var boundary = (int)Math.Floor(Math.Sqrt(number));
            for (int i = boundary; i >= 1; i--)
                if (number % i == 0) return (i, number / i);

            // Should never reach here since every number is divisible by one
            throw new InvalidValueException();
        }

        /// <summary>
        /// Get the maximum value from a multiple values
        /// </summary>
        /// <param name="values"> the values to find the maximum from</param>
        /// <returns> the most significant value </returns>
        public static int Max(params int[] values)
        {
            int max = values[0];

            for (int i = 1; i < values.Length; i++)
                if (max < values[i]) max = values[i];
            return max;
        }

        /// <summary>
        /// Get the minimum value from multiple values
        /// </summary>
        /// <param name="values"> the values to find the minimum from</param>
        /// <returns> the least significant value </returns>
        public static int Min(params int[] values)
        {
            int min = values[0];

            for (int i = 1; i < values.Length; i++)
                if (min > values[i]) min = values[i];
            return min;
        }

    }
}
