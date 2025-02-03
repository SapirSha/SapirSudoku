using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            StringBuilder menu = new StringBuilder("\n\n - - - - - Menu: ");
            menu.AppendLine();
            menu.AppendLine("Enter Sudoku String Or File Path (\"Sapir\" to end):");
            return menu.ToString();
        }
        
        public static bool IsSudoku(String str)
        {
            int length = str.Length;
            char maxLetterPossible = (char)(Math.Ceiling(Math.Sqrt(length) + '0') + 1);
            //if (maxLetterPossible - '0' > Sudoku.MAX_SUDOKU_LENGTH) return false;
            String stringSudokuRegex = "^[";

            if (maxLetterPossible < '9') maxLetterPossible = '9';

            for (char sudokuLetter = '0'; sudokuLetter <= maxLetterPossible; sudokuLetter++)
                stringSudokuRegex += sudokuLetter;
            stringSudokuRegex += "]+$";

            Regex sudokuRegex = new Regex(@stringSudokuRegex);

            return sudokuRegex.IsMatch(@str);
        }

        public static bool IsFilePath(String potentialPath)
        {
            return true;
        }

        private static void PrintSudokuAnswer(Sudoku answer)
        {
            String answerString = SudokuConvertionsHelper.ConvertSudokuToString(answer);
            Console.WriteLine(answerString);
        }

        public static Sudoku? GetSudoku(String sudokuString)
        {
            try
            {
                return SudokuConvertionsHelper.ConvertStringToSudoku(sudokuString);
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

        public static void handleSudoku(String sudokuString)
        {
            Sudoku? sudoku = GetSudoku(sudokuString);
            if (sudoku is not null)
            {
                if (!sudoku.IsValid())
                {
                    Console.WriteLine("SUDOKU Was invalid at input!");
                    return;
                }

                int count = 0;
                int max_answers = 3;
                foreach(Sudoku answer in sudoku.Answers)
                {
                    PrintSudokuAnswer(answer);
                    if (++count >= max_answers)
                    {
                        Console.WriteLine($"Currently viewing at most {max_answers} answers, check for how much more?");
                        int? input = ConsoleInput.GetInputInt();
                        if (input is null || input <= 0)
                        {
                            Console.WriteLine(" - - - - - Canceling");
                            break;
                        }
                        else
                            max_answers += (int)input;
                    }
                }

                if (count == 0)
                    Console.WriteLine("SUDOKU Is not a solvable board!");
            }
        }

        public static void handlePath(String pathFile)
        {
            IEnumerable<String>? linesList = FileInput.GetLines(pathFile);
            if (linesList is null) Console.WriteLine("INVALID FILE PATH!");
            else
            {
                foreach (String line in linesList)
                {
                    if (line.Equals("")) continue;
                    Console.WriteLine(line + "\n");
                    handleSudoku(line);
                    Console.WriteLine("\n");
                }
            }
        }

        public static void handleError(String input)
        {
            Console.WriteLine("INVALID INPUT!");
        }

        public static void UI()
        {
            while (true)
            {
                Console.WriteLine(Menu());

                String? input = ConsoleInput.GetInput();


                Console.WriteLine();

                if (input is null)
                {
                    Console.WriteLine("- - - - - Exiting Program");
                    break;
                }
                input = input.Trim();
                if (input.Equals(""))
                {
                    Console.WriteLine("Please Provide input");
                    continue;
                }
                else if (input.ToLower().Equals("sapir"))
                {
                    Console.WriteLine("- - - - - Exiting Program");
                    break;
                }

                Action<String> handler = input switch
                {
                    var gotSudoku when IsSudoku(gotSudoku) => handleSudoku,
                    var gotFilePath when IsFilePath(gotFilePath) => handlePath,
                    _ => handleError
                };

                handler(input);
            }

        }
    }
}
