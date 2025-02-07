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
        /// <param name="number"> The value to find the closest multiplication from </param>
        /// <returns>A tuple that contains the tow closest numbers that thier multiplication is 'number'</returns>
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

        /// <returns> True if the number is a perfect square root</returns>
        public static bool IsPerfectSquareRoot(int num)
            => Math.Sqrt(num) == (int)Math.Sqrt(num);

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

    }
}
