namespace SapirSudoku.src.Exceptions
{
    public class SetIsEmptyException : Exception
    {
        public SetIsEmptyException() { }
        public SetIsEmptyException(String msg) : base(msg) { }
    }
}
