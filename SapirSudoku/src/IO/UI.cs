using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SapirSudoku.src.Exceptions;

namespace SapirSudoku.src.IO
{
    public static class UI
    {
        private const String MENU =
            "\n\n - - - - - Menu: \n" +
            $"Enter Sudoku String Or File Path (\"{EXIT_STRING}\" to end):\n";

        private const String EXIT_STRING = "sapir";
        public static void StartUI()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine(MENU);
                    String? input = GetInput();

                    if (input is null || input.ToLower().Equals("sapir"))
                        throw new EndOfInputException();



                    IInputOutput? handler = input switch
                    {
                        var possibleSudoku when TypeOfInput.IsSudoku(possibleSudoku) => new ConsoleIO(),
                        var possiblePath when TypeOfInput.IsFilePath(possiblePath) => new FileIO(),
                        _ => null
                    };

                    if (handler is null)
                    {
                        Console.WriteLine("Invalid Input!");
                        continue;
                    }

                    handler.HandleInput(input);

                }
            }
            catch (EndOfInputException)
            {
                Console.WriteLine(" - - - - - Exiting");
            }
        }

        private static void ExitKeyHandler(object? sender, ConsoleCancelEventArgs args) => args.Cancel = true;
        

        public static String? GetInput()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(ExitKeyHandler);

            try
            {
                return Console.ReadLine();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
