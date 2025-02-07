namespace SapirSudoku.src.Exceptions
{
    public class EndOfInputException : Exception
    {
        public EndOfInputException() { }
        public EndOfInputException(String msg) : base(msg) { }
    }
}
