using System.Text;
using SapirSudoku.src.Exceptions;

namespace SapirSudoku.src.IO
{
    /// <summary>
    /// Base class for the input and handling of recived sudokus
    /// </summary>
    public abstract class InputOutput
    {
        /// <summary>
        /// Handles the input
        /// </summary>
        public abstract void HandleInput(string input);
        /// <summary>
        /// Handls the output
        /// </summary>
        public abstract void HandleOutput(string output);

        /// <summary>
        /// A function that tries to convert a string to a sudoku, 
        /// and if failes outputs an appropriate message
        /// </summary>
        /// <param name="sudokuString"> the potential sudoku </param>
        /// <returns> the sudoku that is represented by the string, or null if failed </returns>
        protected Sudoku? TryGetSudoku(string sudokuString)
        {
            try
            {
                return SudokuConvertionsHelper.ConvertStringToSudoku(sudokuString);
            }
            catch (InvalidValueException)
            {
                // if found invalid values, search for them and print them
                PrintSudokuInvalidValues(SudokuErrors.SearchInvalidValues(sudokuString));
            }
            catch (InvalidSudokuSizeException isse)
            {
                HandleOutput(isse.Message);
            }

            return null;
        }

        /// <summary>
        /// Outputs the collisions that happen in the sudoku
        /// </summary>
        /// <param name="collisions"> A List Of Collisions </param>
        protected void PrintSudokuCollisions(List<SudokuErrors.Collisions> collisions)
        {
            var sb = new StringBuilder();
            foreach (var collision in collisions)
            {
                // the value that has a collision
                int value = collision.value;

                // all the collisions in all rows
                foreach (int row in collision.rowCollisions)
                    sb.AppendLine($"Value: {value} appears more than once in row: {row}");
                // all the collisions in all columns
                foreach (int col in collision.colCollisions)
                    sb.AppendLine($"Value: {value} appears more than once in column: {col}");
                // all the collisions in all grids
                foreach (int grid in collision.gridCollisions)
                    sb.AppendLine($"Value: {value} appears more than once in grid: {grid}");
            }

            HandleOutput(sb.ToString());
        }

        /// <summary>
        /// Outputs the invalid values that appear in the sudoku
        /// </summary>
        /// <param name="invalidValues">A HashSet Containning invalid values that were inserted </param>
        protected void PrintSudokuInvalidValues(HashSet<char> invalidValues)
        {
            var sb = new StringBuilder();
            foreach (char c in invalidValues)
                sb.AppendLine($"Found invalid value '{c}' in sudoku");

            HandleOutput(sb.ToString());
        }

        /// <summary>
        /// Outputs the sudoku
        /// </summary>
        /// <param name="answer">The sudoku to output</param>
        protected void PrintSudoku(Sudoku sudoku)
            => HandleOutput("\n" + SudokuConvertionsHelper.ConvertSudokuToString(sudoku));

        /// <summary>
        /// Get an integer from input
        /// </summary>
        /// <returns> an integer if an int was recieved, null otherwise </returns>
        /// <exception cref="EndOfInputException"> Thrown in case of recieving null </exception>
        protected int? GetInputInt()
        {
            string? input = UI.GetInput();
            if (input is null)
                throw new EndOfInputException();
            return int.TryParse(input, out int n) ? n : null;
        }

        /// <summary>
        /// Converts the Sudoku string to a Sudoku instance,
        /// and calls the solve function
        /// </summary>
        /// <param name="sudokuString"></param>
        protected void ProcessSudoku(string sudokuString)
        {
            Sudoku? sudoku = TryGetSudoku(sudokuString);

            if (sudoku is null)
                return;

            if (!sudoku.IsValid())
            {
                HandleOutput("Sudoku was invalid at input!");
                PrintSudokuCollisions(SudokuErrors.SearchCollisions(sudoku));
                return;
            }

            SolveAndPrintSudoku(sudoku);
        }

        /// <summary>
        /// Solves and prints to the destination
        /// </summary>
        /// <param name="sudoku">The Sudoku that is ment to be solved</param>
        protected abstract void SolveAndPrintSudoku(Sudoku sudoku);
    }
}
