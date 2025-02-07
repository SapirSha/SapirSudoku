namespace SapirSudoku.src.Exceptions
{
    public class InvalidBitSetSizeException : Exception
    {
        public InvalidBitSetSizeException() { }
        public InvalidBitSetSizeException(String msg) : base(msg) { }
    }
}
