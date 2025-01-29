
namespace SapirSudoku.src.Exceptions
{
    public class InvalidInsertionException : Exception
    {
        public InvalidInsertionException() { }
        public InvalidInsertionException(string message) : base(message) { }
    }
}
