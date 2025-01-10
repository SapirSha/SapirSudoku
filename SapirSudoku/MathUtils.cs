using System;
using System.Collections.Generic;
using CustomExceptions;

namespace SapirSudoku
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
            int bigger = (int)number / smaller;
            return (smaller, bigger);
        }


    }
}
