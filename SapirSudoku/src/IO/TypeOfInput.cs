using System.Text.RegularExpressions;

namespace SapirSudoku.src.IO
{
    /// <summary>
    /// A class that determines the type of input the was recieved
    /// </summary>
    public static class TypeOfInput
    {
        /// <summary>
        /// Determines how much outside the bounds of a sudoku,
        /// is a string representing a sudoku, still considered a sudoku
        /// </summary>
        private const int MAX_DIVIATION_FOR_SUDOKU = 10;
        /// <summary>
        /// Determines if a string is representing a sudoku
        /// </summary>
        /// <param name="str"> The possible sudoku String</param>
        /// <returns> True if the string is representing a sudoku, flase otherwise </returns>
        public static bool IsSudoku(String str)
        {
            int length = str.Length;
            char maxLetterPossible = (char)(Math.Ceiling(Math.Sqrt(length) + ValueConvertor.MINIMUM_VALUE) + MAX_DIVIATION_FOR_SUDOKU);
            char minLetterPossible = (char)(ValueConvertor.MINIMUM_VALUE - MAX_DIVIATION_FOR_SUDOKU);
            
            String stringSudokuRegex = "^[";

            for (char sudokuLetter = minLetterPossible; sudokuLetter <= maxLetterPossible; sudokuLetter++)
                stringSudokuRegex += sudokuLetter;
            stringSudokuRegex += "]+$";

            Regex sudokuRegex = new Regex(stringSudokuRegex);
            // final regex = "[^(min-diviation)......(max+diviation)]+$"

            return sudokuRegex.IsMatch(str);
        }


        /// <summary>
        /// Determines if the string is representing a text file path
        /// </summary>
        /// <param name="potentialPath"> The potential text file path </param>
        /// <returns>True if representing a text file path, false otherwise</returns>
        public static bool IsFilePath(String potentialPath)
        {
            return potentialPath.EndsWith(".txt");
        }
    }
}
