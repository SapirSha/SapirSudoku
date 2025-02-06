using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapirSudoku.src.IO
{
    public class FileIO : IInputOutput
    {
        public String? pathFile;
        public override void HandleInput(String pathFile)
        {
            this.pathFile = pathFile;

            IEnumerable<String>? linesList = GetLines(pathFile);
            int counter = 1;
            if (linesList is null) Console.WriteLine("INVALID FILE PATH!");
            else
            {
                foreach (String line in linesList)
                {
                    if (line.Equals("") || !TypeOfInput.IsSudoku(line)) continue;
                    if (linesList.Count() > 1) HandleOutput(" - - - - - - - - - - SUDOKU: " + (counter++) + ":");
                    HandleOutput("\n");
                    HandleSudoku(line);
                    HandleOutput("\n");
                }
                if (counter == 1 && linesList.Count() > 1)
                    HandleOutput("No Sudoku Found in File!");
            }
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

                count_answers++;

                // Time only the first solve
                if (count_answers == 1)
                {
                    watch.Stop();
                    PrintWithOutFile($" - - - - - Took {watch.ElapsedMilliseconds}ms to get to first answer\n\n");
                }


                if (count_answers >= max_answers)
                {
                    PrintWithOutFile($"Currently viewing at most {max_answers} answers, check for how many more?");
                    int? input = GetInputInt();

                    if (input is null || input <= 0)
                    {
                        PrintWithOutFile(" - - - - - Stopping");
                        break;
                    }
                    else
                        max_answers += (int)input;
                }
            }


            if (count_answers == 0)
                HandleOutput("Unsolvable Suudoku!");
        }



        public static IEnumerable<String>? GetLines(String pathFile)
        {
            try
            {
                String[] Lines = File.ReadAllLines(@pathFile);
                return Lines;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override void HandleOutput(string str)
        {
            Console.WriteLine(str);
            if (this.pathFile is null || !PrintToFile(this.pathFile, str))
                Console.WriteLine(" - - - - - Failed To Print To File!");
        }
        public void PrintWithOutFile(string str)
        {
            Console.WriteLine(str);
        }



        private static bool PrintToFile(String pathFile, String content)
        {
            try
            {
                File.AppendAllText(pathFile, content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
