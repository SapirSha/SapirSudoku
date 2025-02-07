namespace SapirSudoku.src.IO
{
    /// <summary>
    /// Converts the value from an input value, to values in the sudoku
    /// </summary>
    public static class ValueConvertor
    {
        /// <summary>
        /// The value the sequence will start at
        /// </summary>
        public const char MINIMUM_VALUE = '0';

        /// <summary>
        /// Converts input to to sudoku value
        /// </summary>
        /// <param name="c"> The charachter to be converted into a sudoku value </param>
        /// <returns> an integer that is supposed to be entered to the sudoku </returns>
        public static int ConvertToInt(char c)
        {
            return c - MINIMUM_VALUE;
        }
        /// <summary>
        /// Coverts a sudoku value back into input
        /// </summary>
        /// <param name="i"> The sudoku value that is supposed to be converted </param>
        /// <returns> A char that represents the sudoku value </returns>
        public static int ConvertToChar(int i)
        {
            return i + MINIMUM_VALUE;
        }
    }
}
