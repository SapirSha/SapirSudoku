using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SapirSudoku.src.IO
{
    public static class TypeOfInput
    {

        public static bool IsSudoku(String str)
        {
            int MAX_DIVIATION_FOR_SUDOKU = 10;

            int length = str.Length;
            char maxLetterPossible = (char)(Math.Ceiling(Math.Sqrt(length) + '0') + MAX_DIVIATION_FOR_SUDOKU);
            String stringSudokuRegex = "^[";

            // MINIMUM ALL NUMBERS
            if (maxLetterPossible < '9') maxLetterPossible = '9';

            for (char sudokuLetter = '0'; sudokuLetter <= maxLetterPossible; sudokuLetter++)
                stringSudokuRegex += sudokuLetter;
            stringSudokuRegex += "]+$";

            Regex sudokuRegex = new Regex(@stringSudokuRegex);

            return sudokuRegex.IsMatch(@str);
        }


        public static bool IsFilePath(String potentialPath)
        {
            return !IsSudoku(potentialPath);
        }
    }
}
