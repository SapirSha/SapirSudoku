namespace SapirSudoku.src.Exceptions
{
    public class InvalidBitSetValueException : Exception
    {
        public InvalidBitSetValueException() { }
        public InvalidBitSetValueException(String msg) : base(msg) { }
    }
}
