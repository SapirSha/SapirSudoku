using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SapirSudoku.src.Exceptions;

namespace SapirSudoku.src.IO
{
    public static class ConsoleInput
    {
        public static Sudoku GetSudoku()
        {
            String input = Console.ReadLine();
            if (input is null) throw new EndOfStreamException();
            Sudoku sudoku = SudokuConvertionsHelper.ConvertStringToSudoku(input);
            return sudoku;
        }
    }
}
