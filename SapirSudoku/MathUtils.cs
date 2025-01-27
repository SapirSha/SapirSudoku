using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using CustomExceptions;

namespace SapirMath
{
    public static class MathUtils
    {

        public static (int,int) ColsestDivisibles(int number)
        {
            if (number == 0) return (0, 0);

            if (number < 0) {
                (int smaller, int bigger) negative = ColsestDivisibles(number * -1) ;
                return (-1 * negative.smaller, negative.bigger);
            }

            var boundary = (int)Math.Floor(Math.Sqrt(number));
            for (int i = boundary; i > 1; i--)
                if (number % i == 0) return (i, number / i);

            return (-1, -1);
        }

        public static int Max(params int[] values)
        {
            int max = values[0];

            for (int i = 1; i < values.Length; i++)
                if (max < values[i]) max = values[i];
            return max;
        }


        public static int Min(params int[] values)
        {
            int min = values[0];

            for (int i = 1; i < values.Length; i++)
                if (min > values[i]) min = values[i];
            return min;
        }

    }
}
