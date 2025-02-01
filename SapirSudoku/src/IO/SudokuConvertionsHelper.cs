using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SapirSudoku.src.Exceptions;
using SapirSudoku.src.Utilities;

namespace SapirSudoku.src.IO
{
    public static class SudokuConvertionsHelper
    {
        public static Sudoku ConvertStringToSudoku(String sudokuString)
        {
            int fullLength = sudokuString.Length;
            if (!MathUtilities.IsPerfectSquareRoot(fullLength))
            {
                (int smaller, int bigger) = MathUtilities.ClosestMultiplications(fullLength);
                throw new InvalidSudokuSizeException($"Sudoku length must be N*N, " +
                    $"but got length: {fullLength} as the full sudoku, which doesnt have an integer square root");
            }


            int length = (int)Math.Sqrt(fullLength);
            Sudoku sudoku = new Sudoku((int)Math.Sqrt(fullLength));

            for (int index = 0; index < fullLength; index++)
                sudoku.Insert(sudokuString[index] - '0', index / length, index % length);

            return sudoku;
        }

        public static String ConvertSudokuToString(Sudoku sudoku)
        {
            int[,] sudokuBoard = sudoku.Board;
            char[] stringBoard = new char[sudokuBoard.Length];

            for (int row = 0; row < sudokuBoard.GetLength(0); row++)
            {
                for(int col = 0; col< sudokuBoard.GetLength(1); col++)
                {
                    stringBoard[row * sudokuBoard.GetLength(0) + col] = (char)(sudokuBoard[row, col] + '0');
                }
            }

            return new string(stringBoard);
        }
    }
}
