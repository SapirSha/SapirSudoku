using System;

namespace CustomExceptions
{
    public class PrimeNumberException : Exception
    {
        public PrimeNumberException() { }
        public PrimeNumberException(string message) : base(message) { }
    }

}
