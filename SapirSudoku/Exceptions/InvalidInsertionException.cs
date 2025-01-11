using System;
using System.Collections.Generic;

namespace CustomExceptions
{
    public class InvalidInsertionException : Exception
    {
        public InvalidInsertionException() { }
        public InvalidInsertionException(string message) : base(message) { }
    }
}
