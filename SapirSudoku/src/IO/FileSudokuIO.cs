using System.Diagnostics;

namespace SapirSudoku.src.IO
{
    /// <summary>
    /// The class is made to handle the sudokus from files
    /// </summary>
    public class FileSudokuIO : InputOutput
    {
        /// <summary> The file in question </summary>
        private string? filePath;

        /// <summary>
        /// This enum represents the states where the file print are at,
        /// it is used to print the fail messege only once,
        /// and then stop trying to reach the file and print only to console
        /// </summary>
        enum FilePrintStages
        {
            Failed = 0,
            Success = 1,
            Pending = 2
        }

        private FilePrintStages currentPrintStage = FilePrintStages.Pending;

        /// <summary>
        /// Output to the console without printing to the file
        /// </summary>
        /// <param name="output"> The output to print </param>
        private void PrintWithoutFile(string output)
            => Console.WriteLine(output);

        /// <summary>
        /// Breaks down the input from the file into sudoku and sends them to be solved
        /// </summary>
        /// <param name="filePath"> The path of the file recieved </param>
        public override void HandleInput(string filePath)
        {
            this.filePath = filePath;

            // Get the lines in the file
            IEnumerable<string>? lines = GetLines(filePath);
            if (lines is null)
            {
                PrintWithoutFile($"Failed to read file: {filePath}");
                return;
            }

            // Put all the lines that are sudokus into a list
            List<String> sudokuLines = lines.Where(line => TypeOfInput.IsSudoku(line.Trim())).ToList();
            
            if (sudokuLines.Count == 0)
            {
                PrintWithoutFile("No Sudoku Found in File!");
                return;
            }

            int sudokuCounter = 1;
            foreach (string line in sudokuLines)
            {
                if (sudokuLines.Count > 1)
                    // if there is more then one line with sudoku, print their number
                    HandleOutput($"\n\n - - - - - Sudoku {sudokuCounter++}:");

                PrintWithoutFile("\n");
                ProcessSudoku(line.Trim());
                PrintWithoutFile("\n");
            }
            // Resets the stage to be pending if it ever changed
            currentPrintStage = FilePrintStages.Pending;
        }
        // This function is being called from ProccessSudoku function in InputOutput class
        protected override void SolveAndPrintSudoku(Sudoku sudoku)
        {
            int countAnswers = 0;
            int maxAnswers = 3;
            Stopwatch watch = Stopwatch.StartNew();

            foreach (Sudoku answer in sudoku.Answers)
            {
                // print Sudku to console
                PrintWithoutFile("\n\n - - - - - - - - ");
                PrintWithoutFile(answer.ToString());
                // print Sudku String to file and console
                PrintSudoku(answer);
                PrintWithoutFile(" - - - - - - - - ");
                countAnswers++;

                if (countAnswers == 1)
                {
                    // Time only the first answer
                    watch.Stop();
                    PrintWithoutFile($" - - - - - Took {watch.ElapsedMilliseconds}ms to get the first answer\n");
                }

                if (countAnswers >= maxAnswers)
                {
                    PrintWithoutFile($"Currently viewing at most {maxAnswers} answers, check for how many more?");
                    int? amount = GetInputInt();

                    if (amount is null || amount <= 0)
                    {
                        if (amount is null) PrintWithoutFile(" - - - INVALID INPUT!");
                        PrintWithoutFile(" - - - - - Stopping");
                        break;
                    }

                    maxAnswers += amount.Value;
                }
            }

            if (countAnswers == 0)
                HandleOutput("Unsolvable Sudoku!");
        }
        /// <summary>
        /// Prints the output to the file, and the console
        /// </summary>
        /// <param name="output"> The out put to be delivered </param>
        public override void HandleOutput(string output)
        {
            PrintWithoutFile(output);
            // checks if the stage is not in failure and if the append succeeded
            if (currentPrintStage == FilePrintStages.Failed || filePath is null || !AppendToFile(filePath, output))
                // if not then turn to failure stage
                FilePrintFailure();
        }

        /// <summary>
        /// Turns the current file print stage into failure
        /// </summary>
        private void FilePrintFailure()
        {
            if (currentPrintStage != FilePrintStages.Failed)
            {
                PrintWithoutFile(" - - - - - Failed to print to file! *Changing to only console!*");
                currentPrintStage = FilePrintStages.Failed;
            }
        }

        /// <summary>
        /// Get all the lines in the file
        /// </summary>
        /// <param name="filePath"> the path for the file </param>
        /// <returns>IEnumerable of all the lines, or null if unsuccessful</returns>
        private static IEnumerable<string>? GetLines(string filePath)
        {
            try
            {
                return File.ReadAllLines(filePath);
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Append a string to a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        /// <returns> true if successful, false otherwise</returns>
        private static bool AppendToFile(string filePath, string content)
        {
            try
            {
                File.AppendAllText(filePath, content + "\n");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
