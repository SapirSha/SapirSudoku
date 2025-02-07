namespace SapirSudoku.src.Exceptions
{
    public class InvalidSudokuException : Exception
    {
        public InvalidSudokuException() { }

        public InvalidSudokuException(string message) : base(message) { }
    }
}
