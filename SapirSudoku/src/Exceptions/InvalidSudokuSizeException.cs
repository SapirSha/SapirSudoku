namespace SapirSudoku.src.Exceptions
{
    public class InvalidSudokuSizeException : Exception
    {
        public InvalidSudokuSizeException() { }
        public InvalidSudokuSizeException(string message) : base(message) { }
    }
}
