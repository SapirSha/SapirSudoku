using SapirSudoku.src.Exceptions;

namespace SapirSudoku.src.IO
{
    /// <summary>
    /// The Class where the UI itself happens
    /// </summary>
    public static class UI
    {
        private const String MENU =
            "\n\n - - - - - Menu: \n" +
            $"Enter Sudoku String Or Text File Path (\"{EXIT_STRING}\" to end):\n";

        /// <summary>
        /// The String that needs to be entered when wanting to exit the program
        /// </summary>
        private const String EXIT_STRING = "sapir";

        /// <summary>
        /// The function that handles the UI
        /// </summary>
        public static void StartUI()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine(MENU);
                    String? input = GetInput();

                    if (input is null)
                        throw new EndOfInputException();

                    input = input.Trim();
                    if (input.ToLower().Equals("sapir"))
                        throw new EndOfInputException();


                    // Assign to the handler depending on the type of input
                    InputOutput? handler = input switch
                    {
                        var possibleSudoku when TypeOfInput.IsSudoku(possibleSudoku) => new ConsoleSudokuIO(),
                        var possiblePath when TypeOfInput.IsFilePath(possiblePath) => new FileSudokuIO(),
                        _ => null
                    };

                    if (handler is null)
                    {
                        Console.WriteLine("Invalid Input! (Notice: files need to end with .txt)");
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



        /// <summary>
        /// Get Input from the user
        /// </summary>
        /// <returns>Returns the input, or null if an Exception accrued</returns>
        public static String? GetInput()
        {
            // stop Cancel key from crashing the program
            static void ExitKeyHandler(object? sender, ConsoleCancelEventArgs args) => args.Cancel = true;
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
