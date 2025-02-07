using SapirSudoku.src.Exceptions;
using SapirSudoku.src.Utilities;

namespace SapirSudoku.src.IO
{
    /// <summary>
    /// Helper class designed to convert sudoku representations from one to another
    /// </summary>
    public static class SudokuConvertionsHelper
    {
        /// <summary>
        /// Converts a String representing a sudoku, into a sudoku instance
        /// </summary>
        /// <param name="sudokuString"> The string representing a sudoku </param>
        /// <returns> A sudoku instance, with the values of the string </returns>
        /// <exception cref="InvalidSudokuSizeException"> 
        /// Thrown when the size of the string doesnt match a valid sudoku
        /// </exception>
        public static Sudoku ConvertStringToSudoku(String sudokuString)
        {
            int fullLength = sudokuString.Length;

            if (!MathUtilities.IsPerfectSquareRoot(fullLength))
            {
                // used for better message only
                (int smaller, int bigger) = MathUtilities.ClosestMultiplications(fullLength);

                throw new InvalidSudokuSizeException($"Sudoku length must be N*N, " +
                    $"but got length: {fullLength} as the full sudoku, which does not have an integer square root");
            }

            // represents the width and height of the sudoku
            int length = (int)Math.Sqrt(fullLength);

            Sudoku sudoku = new Sudoku((int)Math.Sqrt(fullLength));

            // converts all the values into 
            for (int index = 0; index < fullLength; index++)
                sudoku.Insert(ValueConvertor.ConvertToInt(sudokuString[index]), index / length, index % length);

            return sudoku;
        }


        /// <summary>
        /// Convert a Sudoku instance into a representing Sudoku string
        /// </summary>
        /// <param name="sudoku"> The instance of a sudoku to be converted to string</param>
        /// <returns> A string representing the sudoku </returns>
        public static String ConvertSudokuToString(Sudoku sudoku)
        {
            int[,] sudokuBoard = sudoku.Board;
            char[] stringBoard = new char[sudokuBoard.Length];

            for (int row = 0; row < sudokuBoard.GetLength(0); row++)
            {
                for(int col = 0; col< sudokuBoard.GetLength(1); col++)
                {
                    // put all the values of the sudoku into one longer array of chars
                    stringBoard[row * sudokuBoard.GetLength(0) + col] = (char)ValueConvertor.ConvertToChar(sudokuBoard[row, col]);
                }
            }

            return new string(stringBoard);
        }
    }
}
