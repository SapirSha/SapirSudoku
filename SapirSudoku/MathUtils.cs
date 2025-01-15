using System;
using System.Collections.Generic;
using CustomExceptions;

namespace SapirMath
{
    public static class MathUtils
    {
        public static bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }

        public static (int,int) ColsestDivisibles(int number)
        {
            if (number == 0) return (0, 0);

            if (number < 0) throw new ArgumentOutOfRangeException("No two positive divisibles for a negative number");

            if (IsPrime((int)number))
                throw new PrimeNumberException("Cannot find closest divisibles of a prime number");

            int smaller = (int)Math.Sqrt(number);

            while (number / smaller != (float)number / smaller)
                smaller--;

            int bigger = number / smaller;

            return (smaller, bigger);
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
