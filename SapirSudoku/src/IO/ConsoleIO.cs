using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SapirSudoku.src.Exceptions;

namespace SapirSudoku.src.IO
{
    /// <summary>
    /// The class is made to handle the sudokus from the console
    /// </summary>
    public class ConsoleIO : IInputOutput
    {

        public override void HandleInput(string str)
        {
            HandleSudoku(str);
        }


        private void HandleSudoku(String sudokuString)
        {
            Sudoku? sudoku = TryGetSudoku(sudokuString);

            // sudoku is null when TryGetSudoku failed
            if (sudoku is null) return;

            if (!sudoku.IsValid())
            {
                HandleOutput("SUDOKU Was invalid at input!");
                // Find and print the collisions that made the sudoku invalid
                PrintSudokuCollisions(SudokuErrors.SearchCollisions(sudoku));
                return;
            }

            int count_answers = 0;
            int max_answers = 3;

            Stopwatch watch = Stopwatch.StartNew();

            foreach (Sudoku answer in sudoku.Answers)
            {
                PrintSudoku(answer);
                HandleOutput("\n");

                count_answers++;

                // Time only the first solve
                if (count_answers == 1)
                {
                    watch.Stop();
                    HandleOutput($" - - - - - Took {watch.ElapsedMilliseconds}ms to get to first answer\n\n");
                }


                if (count_answers >= max_answers)
                {
                    HandleOutput($"Currently viewing at most {max_answers} answers, check for how many more?");
                    int? input = GetInputInt();

                    if (input is null || input <= 0)
                    {
                        HandleOutput(" - - - - - Stopping");
                        break;
                    }
                    else
                        max_answers += (int)input;
                }
            }


            if (count_answers == 0)
                HandleOutput("Unsolvable Suudoku!");
        }



        public override void HandleOutput(string str) 
            => Console.WriteLine(str);

    }
}
