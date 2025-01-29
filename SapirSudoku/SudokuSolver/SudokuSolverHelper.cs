// This file contains the helper methods of the SudokuSolver class

using CustomExceptions;
using SapirStruct;

namespace SapirSudoku
{
    public partial class SudokuSolver
    {
        /// <summary>
        /// A function to clear the NextGarunteedAction stack,
        /// and insert all its values into the Sudoku
        /// </summary>
        private void InsertGuranteed()
        {
            while (NextGarunteedAction.Count() != 0)
            {
                (int value, int row, int col) = NextGarunteedAction.Pop();
                if (this.sudoku[row, col] == NONE)
                    Insert(value, row, col);
            }
        }

        /// <summary>
        /// Get the position of a cell with the minimum amount of possible values that can be inserted
        /// </summary>
        /// <returns> 
        /// A tuple that represents a position in the Sudoku(row, col),
        /// where there are the least amount of possible insertions.
        /// </returns>
        private (int, int)? MinimumPossibilitySquare()
        {
            int i;
            // Go through the frequency array until not empty
            for (i = 1; i <= sudoku.GetLength(0); i++)
                if (squarePossibilitesCounter[i].Count() != 0) break;

            return i <= sudoku.GetLength(0) ? squarePossibilitesCounter[i].First() : null;
        }



        /// <summary>
        /// Remove all Insertion, and add all possibilities deletions since the latest guess,
        /// where guess a cell where we inserted without knowing for sure if the value is appropriate.
        /// </summary>
        private void RemoveLatestGuess()
        {
            // if no remainning guesses
            if (previousActions.Count() == 0) return;

            // undo every insertion
            while (previousActions.Peek().insertions.Count() != 0)
            {
                Insertion latestInsertion = previousActions.Peek().insertions.Pop();

                // undo every posibility
                while (latestInsertion.possibilities.Count() != 0)
                {
                    (int valueAc, int rowAc, int colAc) = latestInsertion.possibilities.Pop();

                    // remove possibility
                    AddPossibility(valueAc, rowAc, colAc);
                }

                // remove insertion
                DeInsert(latestInsertion.row, latestInsertion.col);
            }
            // remove latest guess from the stack
            previousActions.Pop();
        }


        /// <summary>
        /// Add possibility for an insertion, for a certain position
        /// </summary>
        /// <param name="value"> The possibility to add</param>
        /// <param name="row"> Position's row number </param>
        /// <param name="col"> Position's col number </param>
        private void AddPossibility(int value, int row, int col)
        {
            // Add possibility for square
            squarePossibilities[row, col].Add(value);
            int count = squarePossibilities[row, col].Count();


            // Add the square one frequency up in the frequency array
            squarePossibilitesCounter[count - 1].Remove((row, col));
            squarePossibilitesCounter[count].Add((row, col));


            // Add current column as a possibility for value in current row
            rowAvailabilityCounter[row, value - 1].Add(col + 1);
            // Add current row as a possibility for value in current col
            colAvailabilityCounter[col, value - 1].Add(row + 1);
            // Add current grid position as a possibility for value in current grid
            gridAvailabilityCounter[GridPositionOf(row, col), value - 1].Add(row % grid_height * grid_width + col % grid_width + 1);
        }


        /// <summary>
        /// Remove a value from a certain position.
        /// </summary>
        /// <param name="row"> A row number </param>
        /// <param name="col"> A column number </param>
        /// <remarks> 
        /// This function only effects the current cell and not his peers.
        /// </remarks>
        private void DeInsert(int row, int col)
        {
            int value = sudoku[row, col];
            if (value == NONE) return;

            //Add value as a possibility for current row, col and grid
            rowAvailability[row].Add(value);
            colAvailability[col].Add(value);
            gridAvailability[GridPositionOf(row, col)].Add(value);

            // add current value as a possibility for current cell
            AddPossibility(value, row, col);

            sudoku[row, col] = NONE;

            count--;
        }



        /// <summary>
        /// Remove a possiblity from a cell.
        /// </summary>
        /// <param name="value"> The possibility to remove </param>
        /// <param name="row"> A row number</param>
        /// <param name="col"> A column number </param>
        /// <param name="changeRow"> whether to change the row also </param>
        /// <param name="changeCol"> whether to change the column also </param>
        /// <param name="changeGrid"> whther to change the grid also </param>
        /// <exception cref="InvalidInsertionException">
        /// Thrown when collisions accrued, or when collisions become unavoidable because of the insertion.
        /// </exception>
        private void RemoveSquarePossibility(int value, int row, int col, bool changeRow = true, bool changeCol = true, bool changeGrid = true)
        {
            if (!squarePossibilities[row, col].Contains(value)) return;

            // save this possibility removal as a part of the current insertion
            Insertion currentInsertion = previousActions.Peek().insertions.Peek();
            currentInsertion.possibilities.Push((value, row, col));

            squarePossibilities[row, col].Remove(value);

            int count = squarePossibilities[row, col].Count();
            switch (count)
            {
                // if no remainning possibilities, Sudoku is unsolvable (or current path is)
                case 0: throw new InvalidInsertionException($"Cell R{row}C{col} has no possibilities due to insertion!");
                // if only one possiblity is remainning, its a neccassery insertion
                case 1: PotentialInsertInSquare(row, col); break;
            }

            // Lower the current position by one in the frequency array
            squarePossibilitesCounter[count + 1].Remove((row, col));
            squarePossibilitesCounter[count].Add((row, col));

            if (changeRow)
            {
                // Remove current column from the possible columns in the current row for the current value
                rowAvailabilityCounter[row, value - 1].Remove(col + 1);

                // Count is the number of columns that has the possibility 'value' in the current row
                count = rowAvailabilityCounter[row, value - 1].Count();
                switch (count)
                {
                    // if count is 0: the last position where value appears was removed, and value can no longer be placed in a certain row
                    case 0: throw new InvalidInsertionException($"Can no longer insert value: {value} in row {row}");
                    // if count is 1: there is only one column for value to be inserted in, in this column
                    case 1: PotentialInsertInRow(value, row); break;
                    // if count is 2: potential XWing (appears only twice in row)
                    case 2: RowXWing(value, row, col); break;
                    default:
                        // if count is less the Maximum search load: search for hidden groups in the row
                        if (count <= sudoku.GetLength(0) * GROUP_SEARCH_LOAD) HiddenInRow(value, row, col, count);
                        break;
                }
            }

            if (changeCol)
            {
                // Remove current row from the possible rows in the current column for the current value
                colAvailabilityCounter[col, value - 1].Remove(row + 1);
                // Count is the number of rows that has the possibility 'value' in the current column
                count = colAvailabilityCounter[col, value - 1].Count();
                switch (count)
                {
                    // if count is 0: the last position where value appears was removed, and value can no longer be placed in a certain column
                    case 0: throw new InvalidInsertionException($"Can no longer insert value: {value} in column {col}");
                    // if count is 1: there is only one row for value to be inserted in, in this column
                    case 1: PotentialInsertInCol(value, col); break;
                    // if count is 2: potential XWing ( appears only twice in column column )
                    case 2: ColXWing(value, row, col); break;
                    default:
                        // if count is less the Maximum search load: search for hidden groups in the column
                        if (count <= sudoku.GetLength(0) * GROUP_SEARCH_LOAD)
                            HiddenInCol(value, row, col, count);

                        break;
                }
            }

            if (changeGrid)
            {
                // Remove current position from the possible positions in the current grid for the current value
                gridAvailabilityCounter[GridPositionOf(row, col), value - 1].Remove(row % grid_height * grid_width + col % grid_width + 1);

                // Count is the number of rows that has the possibility 'value' in the current column
                count = gridAvailabilityCounter[GridPositionOf(row, col), value - 1].Count();

                switch (count)
                {
                    // if count is 0: the last position where value appears at was removed, and value can no longer be placed in a certain grid
                    case 0: throw new InvalidInsertionException($"Can no longer insert value: {value} in grid {GridPositionOf(row, col)}");
                    // if count is 1: there is only one position for value to be inserted in, in this gird.
                    case 1: PotentialInsertInGrid(value, GridPositionOf(row, col)); break;
                    default:
                        // if max is less then the grid width, or less the the grid height: search for pointing groups
                        if (count <= grid_width || count <= grid_height)
                            PointingGroup(value, GridPositionOf(row, col));
                        // if count is less the Maximum search load: search for hidden groups in the grid
                        if (count <= sudoku.GetLength(0) * GROUP_SEARCH_LOAD)
                            HiddenInGrid(value, row, col, count);
                        break;
                }
            }
        }

        /// <summary>
        /// Helper function for the "insert" function:
        /// updates the current grid after an insertion accrued
        /// </summary>
        /// <param name="value"> The value wanted to be inserted </param>
        /// <param name="row"> A row number </param>
        /// <param name="col"> A column number </param>
        private void UpdateSquareInsert(int value, int row, int col)
        {
            BitSet possible = squarePossibilities[row, col];

            // Remove all possibilities from the current cell ( since it is going to be filled )
            foreach (int possibility in possible)
                if (possibility != value)
                    RemoveSquarePossibility(possibility, row, col);

            // Put this cell's position in 0 at the frequency array ( no available posiibilities )
            squarePossibilitesCounter[possible.Count()].Remove((row, col));
            squarePossibilitesCounter[0].Add((row, col));

            // Remove the current value from the possible values in the current row, column and grid.
            rowAvailability[row].Remove(value);
            colAvailability[col].Remove(value);
            gridAvailability[GridPositionOf(row, col)].Remove(value);

            // Make the columns, where value can appear at in the current row, none.
            rowAvailabilityCounter[row, value - 1].ClearAll();
            // Make the rows, where value can appear at in the current column, none.
            colAvailabilityCounter[col, value - 1].ClearAll();
            // Make the positions, where value can appear at in the current grid, none.
            gridAvailabilityCounter[GridPositionOf(row, col), value - 1].ClearAll();

            // Make the current cell possibility, no possibility.
            squarePossibilities[row, col] = new BitSet(0);

            sudoku[row, col] = value;
        }


        /// <summary>
        /// Helper function for the "insert" function:
        /// Update the rows effected after an insert accrued.
        /// </summary>
        /// <param name="value"> the value that is inserted </param>
        /// <param name="row"> the row number the value will be inserted </param>
        /// <param name="col"> the column number the number will be inserted </param>
        private void UpdateRowInsert(int value, int row, int col)
        {
            // look at all rows in the Sudoku with the same column as the inserted position
            for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
            {
                // if the looked at position is in the same grid as the inserted position dont do anything
                // changes in the grid will happend in 'UpdateGridInsert' function
                ///< see cref = "UpdateGridInsert" /> 
                if (GridPositionOf(rowPos, col) == GridPositionOf(row, col)) continue;

                if (sudoku[rowPos, col] != NONE) continue;

                // Remove value as a possibility from current cell.
                // but without changing the column, since the column is the same as the inserted position, and will be cleared later
                RemoveSquarePossibility(value, rowPos, col, true, false, true);
            }
        }


        /// <summary>
        /// Helper function for the "insert" function:
        /// Update the columns effected after an insert accrued.
        /// </summary>
        /// <param name="value"> the value that is inserted </param>
        /// <param name="row"> the row number the value will be inserted </param>
        /// <param name="col"> the column number the number will be inserted </param>
        private void UpdateColInsert(int value, int row, int col)
        {
            // look at all columns in the Sudoku with the same row as the inserted position
            for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
            {
                // if the looked at position is in the same grid as the inserted position dont do anything
                // changes in the grid will happend in 'UpdateGridInsert' function
                ///< see cref = "UpdateGridInsert" /> 
                if (GridPositionOf(row, colPos) == GridPositionOf(row, col)) continue;
                if (sudoku[row, colPos] != NONE) continue;

                // Remove value as a possibility from current cell.
                // but without changing the row, since the row is the same as the inserted position, and will be cleared later.
                RemoveSquarePossibility(value, row, colPos, false, true, true);
            }
        }


        /// <summary>
        /// Helper function for the "insert" function:
        /// Update the grid effected after an insert accrued.
        /// </summary>
        /// <param name="value"> the value that is inserted </param>
        /// <param name="row"> the row number the value will be inserted </param>
        /// <param name="col"> the column number the number will be inserted </param>
        private void UpdateGridInsert(int value, int row, int col)
        {
            // initial cell position of the grid
            int initRow = GridPositionOf(row, col) / (sudoku.GetLength(1) / grid_width) * grid_height;
            int initCol = GridPositionOf(row, col) * grid_width % sudoku.GetLength(1);

            // look at all the cells in the grid where the insertion is happenning
            for (int rowPos = 0; rowPos < grid_height; rowPos++)
            {
                for (int colPos = 0; colPos < grid_width; colPos++)
                {
                    // dont do anything with the current cell
                    // changes to the current cell will happen in "UpdateSquareInsert"
                    ///< see cref = "UpdateSquareInsert" /> 
                    if (initRow + rowPos == row && initCol + colPos == col) continue;

                    if (sudoku[initRow + rowPos, initCol + colPos] != NONE) continue;

                    // Remove value as a possibility from current cell.
                    // but without changing the grid, since the grid is the same as the inserted position, and will be cleared later.
                    RemoveSquarePossibility(value, initRow + rowPos, initCol + colPos, true, true, false);
                }
            }
        }

        /// <summary>
        /// Gets the column where value appears only once at and push the position's values into the NextGarunteedAction stack.
        /// </summary>
        /// <param name="value"> the value that appears once </param>
        /// <param name="row"> the row where the value appears at once </param>
        /// <remarks> Value has to appear only once in the row! </remarks>
        private void PotentialInsertInRow(int value, int row)
        {
            // Get the smallest (in this case only) value in the set (which represents a column number)
            int col = (rowAvailabilityCounter[row, value - 1].GetSmallest() - 1);

            NextGarunteedAction.Push((value, row, col));
        }

        /// <summary>
        /// Gets the row where value appears only once at and push the position's values into the NextGarunteedAction stack.
        /// </summary>
        /// <param name="value"> the value that appears once </param>
        /// <param name="col"> the column where the value appears only once at </param>
        /// <remarks> Value has to appear only once in the column! </remarks>
        private void PotentialInsertInCol(int value, int col)
        {
            // Get the smallest (in this case only) value in the set (which represents a row number)
            int row = colAvailabilityCounter[col, value - 1].GetSmallest() - 1;

            NextGarunteedAction.Push((value, row, col));
        }


        /// <summary>
        /// Gets the grid where value appears only once at and push the position's values into the NextGarunteedAction stack.
        /// </summary>
        /// <param name="value"> the value that appears once </param>
        /// <param name="grid"> the grid where value appears only once at </param>
        private void PotentialInsertInGrid(int value, int grid)
        {
            // Get the smallest (in this case only) value in the set (which represents a position in the grid)
            int rowInPos = (gridAvailabilityCounter[grid, value - 1].GetSmallest() - 1) / grid_width;
            int colInPos = (gridAvailabilityCounter[grid, value - 1].GetSmallest() - 1) % grid_width;

            // the position of the initial cell in the grid
            int initRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
            int initCol = grid * grid_width % sudoku.GetLength(1);

            NextGarunteedAction.Push((value, initRow + rowInPos, initCol + colInPos));
        }
        /// <summary>
        /// Gets the only possible value in the position, and push the position's value into the NextGarunteedAction stack
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="grid"> The position has to have only one possibility! </param>
        private void PotentialInsertInSquare(int row, int col)
        {
            int value = squarePossibilities[row, col].GetSmallest();
            NextGarunteedAction.Push((value, row, col));
        }


    }
}
