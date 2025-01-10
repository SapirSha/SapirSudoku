using System;

namespace CustomExceptions
{
    public class InvalidValueException : Exception
    {
        public InvalidValueException() { }
        public InvalidValueException(string message) : base(message) { }
    }
}
