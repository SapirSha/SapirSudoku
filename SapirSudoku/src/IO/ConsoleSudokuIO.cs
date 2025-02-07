using System.Diagnostics;

namespace SapirSudoku.src.IO
{
    /// <summary>
    /// The class is made to handle the sudokus from the console
    /// </summary>
    public class ConsoleSudokuIO : InputOutput
    {
        /// <summary>
        /// Handles the input, which is supposed to represent a Sudoku
        /// </summary>
        /// <param name="str"> the possible sudoku</param>
        public override void HandleInput(string str)
            => ProcessSudoku(str);

        /// <summary>
        /// Handles the outputs, prints to console
        /// </summary>
        /// <param name="str"> the content to print </param>
        public override void HandleOutput(string str) 
            => Console.WriteLine(str);

        // This function is being called from ProccessSudoku function in InputOutput class
        protected override void SolveAndPrintSudoku(Sudoku sudoku)
        {
            int countAnswers = 0;
            int maxAnswers = 3;

            Stopwatch watch = Stopwatch.StartNew();

            foreach (Sudoku answer in sudoku.Answers)
            {
                PrintSudoku(answer);
                countAnswers++;

                // Print the time it took to get to the first answer
                if (countAnswers == 1)
                {
                    watch.Stop();
                    HandleOutput($" - - - - - Took {watch.ElapsedMilliseconds}ms to get the first answer\n");
                }

                if (countAnswers >= maxAnswers)
                {
                    HandleOutput($"Currently viewing at most {maxAnswers} answers, check for how many more?");
                    int? amount = GetInputInt();
                    
                    if (amount is null || amount <= 0)
                    {
                        if (amount is null) HandleOutput(" - - - INVALID INPUT!");
                        HandleOutput(" - - - - - Stopping");
                        break;
                    }

                    maxAnswers += amount.Value;
                }
            }

            if (countAnswers == 0)
                HandleOutput("Unsolvable Sudoku!");
        }
    }
}
