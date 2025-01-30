using System;
using System.Collections.Generic;
using System.Linq;
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
            int length = (int)Math.Sqrt(fullLength);
            if (Math.Sqrt(fullLength) != length)
            {
                (int smaller, int bigger) = MathUtilities.ClosestMultiplications(fullLength);
                throw new InvalidSudokuSizeException($"Sudoku length must be N*N, " +
                    $"but got length: {fullLength} as the full sudoku, which doesnt have an integer square root");
            }

            Sudoku sudoku = new Sudoku((int)Math.Sqrt(fullLength));

            for (int index = 0; index < fullLength; index++)
                sudoku.Insert(sudokuString[index] - '0', index / length, index % length);

            return sudoku;
        }

    }
}
