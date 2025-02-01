// This file contains the heuristics part of the SudokuSolver class

using SapirSudoku.src.DataStructures;

namespace SapirSudoku.src.SolveSudoku
{
    public partial class SudokuSolver
    {
        /// <summary>
        /// A function to find a excecute a hidden group in a grid,
        /// given a value and its position that is potentially in a hidden group
        /// </summary>
        /// <param name="value"> the value that is potentially in a hidden group</param>
        /// <param name="row"> the row the value appears at </param>
        /// <param name="col"> the column the number appears at </param>
        /// <param name="hidden_type"> the amount of times value appears at in the grid </param>
        private void HiddenInGrid(int value, int row, int col, int hidden_type)
        {
            /* There are X cells where VALUE appears at in the grid
             * If there are (X - 1) other values who only appear in the same cells as VALUE,
             * HIDDEN VALUES IN GRID */

            int grid = GridPositionOf(row, col);

            // Positions where value appears at in the grid
            BitSet possibilities_in_grid_for_value = gridAvailabilityCounter[grid, value - 1];

            // A set holding all values that appear only in the same cells as VALUE in the grid
            BitSet hidden = new BitSet(sudoku.GetLength(0));

            // look at every possible value in the grid
            foreach (int possibility in gridAvailability[grid])
            {
                // if the cells of possibility in the grid are a subset of the cells of value, add to the hidden set
                if (gridAvailabilityCounter[grid, possibility - 1].IsSubsetOf(possibilities_in_grid_for_value))
                    hidden.Add(possibility);

                // if the cells of possibility in the grid are a superset of the cells of value,
                // possibility might have the cells where the hidden group appears at,
                // that is as long as the amount of cells is less then the Max search group load
                else if (gridAvailabilityCounter[grid, possibility - 1].IsSupersetOf(possibilities_in_grid_for_value))
                {
                    if (gridAvailabilityCounter[grid, possibility - 1].Count() <= sudoku.GetLength(0) * GROUP_SEARCH_LOAD)
                    {
                        HiddenInGrid(possibility, row, col, gridAvailabilityCounter[grid, possibility - 1].Count());
                        return;
                    }
                }
            }

            // If the amount of subsets are less then the amount of cells where value appears, its not a hidden group
            if (hidden.Count() < hidden_type) return;


            // The initial position in the grid
            int initRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
            int initCol = grid * grid_width % sudoku.GetLength(1);

            // go to every cell where value appears at in the grid (and by design where the hidden group appears)
            foreach (int position in possibilities_in_grid_for_value)
            {
                // The row and the column of the cell
                row = initRow + (position - 1) / grid_width;
                col = initCol + (position - 1) % grid_width;

                // Remove all possibilities, that are not in the hidden group, but appear in the cells of the hidden group
                foreach (int possibility_in_old in squarePossibilities[row, col])
                    if (!hidden.Contains(possibility_in_old))
                        RemoveSquarePossibility(possibility_in_old, row, col);
            }
        }


        /// <summary>
        /// A function to find a excecute a hidden group in a row,
        /// given a value and its position that is potentially in a hidden group
        /// </summary>
        /// <param name="value"> the value that is potentially in a hidden group</param>
        /// <param name="row"> the row the value appears at </param>
        /// <param name="col"> the column the number appears at </param>
        /// <param name="hidden_type"> the amount of times value appears at in the row </param>
        private void HiddenInRow(int value, int row, int col, int hidden_type)
        {
            /* There are X cells where VALUE appears at in the row
             * if there are (X - 1) other values who only appear in the same cells as VALUE:
             * HIDDEN VALUES IN ROW */

            // A set of columns where value appears at in the row
            BitSet possibilities_in_row_for_value = rowAvailabilityCounter[row, value - 1];

            // A set holding all values that appear only in the same cells as VALUE in the row
            BitSet hidden = new BitSet(sudoku.GetLength(0));

            // look at every possible value in the row
            foreach (int possibility in rowAvailability[row])
            {
                // if the cells of possibility in the row are a subset of the cells of value, add to the hidden set
                if (rowAvailabilityCounter[row, possibility - 1].IsSubsetOf(possibilities_in_row_for_value))
                    hidden.Add(possibility);

                // if the cells of possibility in the row are a superset of the cells of value,
                // possibility might have the cells where the hidden group appears at,
                // that is as long as the amount of cells is less then the Max search group load
                else if (rowAvailabilityCounter[row, possibility - 1].IsSupersetOf(possibilities_in_row_for_value))
                {
                    if (rowAvailabilityCounter[row, possibility - 1].Count() <= sudoku.GetLength(0) * GROUP_SEARCH_LOAD)
                    {
                        HiddenInRow(possibility, row, col, rowAvailabilityCounter[row, possibility - 1].Count());
                        return;
                    }
                }
            }

            // If the amount of subsets are less then the amount of cells where value appears, its not a hidden group
            if (hidden.Count() < hidden_type) return;

            // go to every cell where value appears at in the row (and by design where the hidden group appears)
            foreach (int position in possibilities_in_row_for_value)
            {
                col = position - 1;

                // Remove all possibilities, that are not in the hidden group, but appear in the cells of the hidden group
                foreach (int possibility_in_old in squarePossibilities[row, col])
                    if (!hidden.Contains(possibility_in_old))
                        RemoveSquarePossibility(possibility_in_old, row, col);
            }
        }


        /// <summary>
        /// A function to find a excecute a hidden group in a column,
        /// given a value and its position that is potentially in a hidden group
        /// </summary>
        /// <param name="value"> the value that is potentially in a hidden group</param>
        /// <param name="row"> the row the value appears at </param>
        /// <param name="col"> the column the number appears at </param>
        /// <param name="hidden_type"> the amount of times value appears at in the column </param>
        private void HiddenInCol(int value, int row, int col, int hidden_type)
        {
            /* There are X cells where VALUE appeares at in the column
             * If there are (X - 1) other values who only appear in the same cells as VALUE:
             * HIDDEN VALUES IN COLUMN */

            // A set of rows where value appears at in the column
            BitSet possibilities_in_col_for_value = colAvailabilityCounter[col, value - 1];

            // A set holding all values that appear only in the same cells as VALUE in the column
            BitSet hidden = new BitSet(sudoku.GetLength(0));

            // look at every possible value in the column
            foreach (int possibility in colAvailability[col])
            {
                // if the cells of possibility in the column are a subset of the cells of value, add to the hidden set
                if (colAvailabilityCounter[col, possibility - 1].IsSubsetOf(possibilities_in_col_for_value))
                    hidden.Add(possibility);

                // if the cells of possibility in the column are a superset of the cells of value,
                // possibility might have the cells where the hidden group appears at,
                // that is as long as the amount of cells is less then the Max search group load
                else if (colAvailabilityCounter[col, possibility - 1].IsSupersetOf(possibilities_in_col_for_value))
                {
                    if (colAvailabilityCounter[col, possibility - 1].Count() <= sudoku.GetLength(0) * GROUP_SEARCH_LOAD)
                    {
                        HiddenInCol(possibility, row, col, colAvailabilityCounter[col, possibility - 1].Count());
                        return;
                    }
                }
            }

            // If the amount of subsets are less then the amount of cells where value appears, its not a hidden group
            if (hidden.Count() < hidden_type) return;

            // go to every cell where value appears at in the column (and by design where the hidden group appears)
            foreach (int position in possibilities_in_col_for_value)
            {
                row = position - 1;

                // Remove all possibilities, that are not in the hidden group, but appear in the cells of the hidden group
                foreach (int possibility_in_old in squarePossibilities[row, col])
                    if (!hidden.Contains(possibility_in_old))
                        RemoveSquarePossibility(possibility_in_old, row, col);
            }
        }


        /// <summary>
        /// Checks whether there is a Row Pointing group in a grid
        /// </summary>
        /// <param name="value"> the potential value of the pointing group </param>
        /// <param name="grid"> the grid where the group may appear</param>
        /// <returns> Whether there is a Row Pointing group in a grid </returns>
        private bool IsRowPointingGroup(int value, int grid)
        {
            // a set of the cells where value appears at in the grid
            BitSet possitions = gridAvailabilityCounter[grid, value - 1];
            if (possitions.IsEmpty()) return false;

            // if the cells positions are a subset of any full row, its a pointing group
            foreach (BitSet possibleRow in fullRows)
                if (possitions.IsSubsetOf(possibleRow))
                    return true;
            return false;
        }


        /// <summary>
        /// Checks whether there is a Column Pointing group in a grid
        /// </summary>
        /// <param name="value"> the potential value of the pointing group </param>
        /// <param name="grid"> the grid where the group may appear</param>
        /// <returns> Whether there is a Row Pointing group in a grid </returns>
        private bool IsColPointingGroup(int value, int grid)
        {
            // a set of the cells where value appears at in the grid
            BitSet possitions = gridAvailabilityCounter[grid, value - 1];
            if (possitions.IsEmpty()) return false;

            // if the cells positions are a subset of any full column, its a pointing group
            foreach (BitSet possibleRow in fullCols)
                if (possitions.IsSubsetOf(possibleRow))
                    return true;

            return false;
        }
        /// <summary>
        /// A function to search and excecute a pointing group.
        /// </summary>
        /// <param name="value"> The value that is potentially the pointing group </param>
        /// <param name="grid"> the grid where the group may appear</param>
        private void PointingGroup(int value, int grid)
        {
            if (IsRowPointingGroup(value, grid))
            {
                // The initial row of the grid
                int initRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
                // The row where value appears at (and by design where the pointing griup is)
                int row = initRow + (gridAvailabilityCounter[grid, value - 1].Smallest - 1) / grid_width;

                // Remove all possibilities for value in all other grids, besides the grid of the pointing group
                for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
                    if (GridPositionOf(row, colPos) != grid)
                        RemoveSquarePossibility(value, row, colPos);
            }
            else if (IsColPointingGroup(value, grid))
            {
                // The initial column of the grid
                int initCol = grid * grid_width % sudoku.GetLength(1);
                // The column where value appears at (and by design where the pointing griup is)
                int col = initCol + (gridAvailabilityCounter[grid, value - 1].Smallest - 1) % grid_width;

                // Remove all possibilities for value in all other grids, besides the grid of the pointing group
                for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
                    if (GridPositionOf(rowPos, col) != grid)
                        RemoveSquarePossibility(value, rowPos, col);
            }
        }

    }
}
