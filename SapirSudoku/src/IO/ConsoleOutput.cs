using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SapirSudoku.src.Exceptions;

namespace SapirSudoku.src.IO
{
    public static class ConsoleOutput
    {
        private static String Menu()
        {
            StringBuilder menu = new StringBuilder(" - - - - - Menu: ");
            menu.AppendLine();
            menu.AppendLine("Enter Sudoku String:");
            return menu.ToString();
        }

        private static Sudoku? TryGetSudoku()
        {
            try{
                String input = ConsoleInput.GetInput();
                if (input is null) return null;

                Sudoku? sudoku = null;


                if (IsSudoku(input))
                    sudoku = SudokuConvertionsHelper.ConvertStringToSudoku(input);
                else //if (IsFilePath(input))
                {
                    sudoku = FileInput.GetSudoku(input);
                    if (sudoku is null)
                    {
                        Console.WriteLine("INFALID FILE PATH!");
                        return null;
                    }
                }
                return sudoku;
            }
            catch (InvalidValueException invalidInsertion)
            {
                Console.WriteLine("Invalid Sudoku Board: found invalid values");
                Console.WriteLine(invalidInsertion.Message);
            }
            catch (InvalidSudokuSizeException invalidSize)
            {
                Console.WriteLine(invalidSize.Message);
            }
            return null;
        }

        public static bool IsSudoku(String str)
        {
            int length = str.Length;
            char maxLetterPossible = (char)(Math.Ceiling(Math.Sqrt(length) + '0') + 1);
            if (maxLetterPossible - '0' > Sudoku.MAX_SUDOKU_LENGTH) return false;
            String stringSudokuRegex = "^[";

            if (maxLetterPossible < '9') maxLetterPossible = '9';

            for (char sudokuLetter = '0'; sudokuLetter <= maxLetterPossible; sudokuLetter++)
                stringSudokuRegex += sudokuLetter;
            stringSudokuRegex += "]+$";

            Console.WriteLine("STRING REGEX = " + stringSudokuRegex);

            Console.WriteLine(str);
            Regex sudokuRegex = new Regex(@stringSudokuRegex);

            return sudokuRegex.IsMatch(@str);
        }

        public static bool IsFilePath(String str)
        {
            return !IsSudoku(str);
        }

        public static void UI()
        {
            while (true)
            {
                Console.WriteLine(Menu());

                Sudoku? sudoku = TryGetSudoku();
                
                
                Console.WriteLine(sudoku is null ? "NULL" : sudoku);
            }
        }
    }
}
