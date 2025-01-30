using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                Sudoku sudoku = ConsoleInput.GetSudoku();
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

        public static void UI()
        {
            Console.WriteLine(Menu());

            while (true)
            {
                Sudoku? sudoku = TryGetSudoku();
                if (sudoku is null) continue;

                if (!sudoku.IsValid())
                {
                    Console.WriteLine("Invalid Sudoku! found collisions at input!");
                    continue;
                }

                int c = 0;
                int MAX = 3;

                foreach (Sudoku answer in sudoku.Answers)
                {
                    if (++c <= MAX)
                        Console.WriteLine(
                            "\n - - - - - - - - \n" +
                            answer +
                            "\n - - - - - - - - \n");
                    else break;
                }

                if (c == 0) Console.WriteLine("Unsolvable board!");

            }

        }
    }
}
