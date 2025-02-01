using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapirSudoku.src.IO
{
    public static class FileInput
    {
        public static Sudoku GetSudoku(String? pathFile)
        {
            if (pathFile is null)
                pathFile = AppDomain.CurrentDomain.BaseDirectory;
            String fileContent = File.ReadAllText(pathFile);
            Sudoku sudoku = SudokuConvertionsHelper.ConvertStringToSudoku(fileContent);
            return sudoku;
        }
    }
}
