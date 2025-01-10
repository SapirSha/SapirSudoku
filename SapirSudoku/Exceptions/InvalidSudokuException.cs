using System;

namespace CustomExceptions
{
    public class InvalidSudokuException : Exception
    {
        public InvalidSudokuException() { }

        public InvalidSudokuException(string message) : base(message) { }
    }
}
