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
        public static void UI()
        {
            Console.WriteLine(Menu());

            Sudoku? sudoku = null;
            try
            {
                sudoku = ConsoleInput.GetSudoku();
                
            } 
            catch(InvalidValueException invalidInsertion)
            {
                Console.WriteLine("Invalid Sudoku Board: found invalid values");
                Console.WriteLine(invalidInsertion.Message);
            }
            catch (InvalidSudokuSizeException invalidSize)
            {
                Console.WriteLine(invalidSize.Message);
            }

            if (sudoku is null)
                return;
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




        }
    }
}
