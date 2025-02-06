using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SapirSudoku.src.Exceptions;

namespace SapirSudoku.src.IO
{
    public abstract class IInputOutput
    {
        public abstract void HandleInput(String str);
        public abstract void HandleOutput(String str);




        /// <summary>
        /// this functions returns the sudoku that is supposed to appear from the string, if it is valid
        /// </summary>
        /// <param name="sudokuString"> The String representing the sudoku</param>
        /// <returns> A sudoku instance that represents the string 'sudokuString'</returns>
        protected Sudoku? TryGetSudoku(String sudokuString)
        {
            try
            {
                // Try to convert the String into a sudoku instance
                return SudokuConvertionsHelper.ConvertStringToSudoku(sudokuString);
            }
            catch (InvalidValueException)
            {
                // Search and print the values that were found as invalid
                PrintSudokuInvalidValues(SudokuErrors.SearchInvalidValues(sudokuString));
            }
            catch (InvalidSudokuSizeException invalidSize)
            {
                HandleOutput(invalidSize.Message);
            }
            return null;
        }



        /// <summary>
        /// Print the collisions that accrued in the sudoku
        /// </summary>
        /// <param name="collisions"></param>
        protected void PrintSudokuCollisions(List<SudokuErrors.Collisions> collisions)
        {
            StringBuilder msg = new StringBuilder();
            foreach (SudokuErrors.Collisions collision in collisions)
            {
                int value = collision.value;

                // the collisions of 'value' in all rows
                foreach (int row in collision.rowCollisions)
                    msg.AppendLine($"Value:{value} appears more then once in row: {row}");

                // the collisions of 'value' in all columns
                foreach (int col in collision.colCollisions)
                    msg.AppendLine($"Value: {value} appears more then once in column: {col}");

                // the collisions of 'value' in all grids
                foreach (int grid in collision.gridCollisions)
                    msg.AppendLine($"Value: {value} appears more then once in grid: {grid}");
            }
            HandleOutput(msg.ToString());
        }

        /// <summary>
        /// Print the invalid values in the sudoku
        /// </summary>
        /// <param name="values"></param>
        protected void PrintSudokuInvalidValues(HashSet<char> invalid_values)
        {
            StringBuilder msg = new StringBuilder();
            foreach (char value in invalid_values)
            {
                msg.AppendLine($"Found Invalid value {value} in sudoku");
            }
            HandleOutput(msg.ToString());
        }

        protected void PrintSudoku(Sudoku answer) 
            => HandleOutput(SudokuConvertionsHelper.ConvertSudokuToString(answer) + "\n");
        

        /// <summary>
        /// Get an integer input from the console
        /// </summary>
        /// <returns>The integer from the input or null if invalid integer </returns>
        /// <exception cref="EndOfInputException"> Thrown when input is null </exception>
        public int? GetInputInt()
        {
            String? input = UI.GetInput();

            if (input is null)
                throw new EndOfInputException();


            if (int.TryParse(input, out int n))
                return n;

            else return null;

        }
    }
}
