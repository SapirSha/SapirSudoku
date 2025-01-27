using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using CustomExceptions;
using SapirStruct;
using SapirSudoku;

namespace SapirSudoku
{
    /// <summary>
    ///  A class that solve a Sudoku.
    /// </summary>
    public class SudokuSolver : Sudoku, IEnumerable<Sudoku>
    {
        /// <summary> How big of a maximum hidden group to check for compared to length </summary>
        private static readonly float GROUP_SEARCH_LOAD = 0.5f;


        /// <summary>
        /// A frequency array:
        /// Every HashSet in the array holds the positions where
        /// the amount of possible insertions is equal to the index
        /// </summary>
        private HashSet<(int row,int col)>[] squarePossibilitesCounter;

        /// <summary>
        /// Each cell of the 2d array is a BitSet Representing the possible insertions
        /// in the representing Sudoku
        /// </summary>
        private BitSet[,] squarePossibilities;


        /// <summary>
        /// Holds the possible values that can be inserted in each row.
        /// </summary>
        private BitSet[] rowAvailability;

        /// <summary>
        /// A 2d Array where:
        /// The rows in this array represents the row its 'assigned' to in the Sudoku,
        /// The columns in this array repesents certain values,
        /// The values in this array are sets that hold the columns where the value appears at, in the same row.
        /// </summary>
        private BitSet[,] rowAvailabilityCounter;


        /// <summary>
        /// Holds the possible values that can be inserted in each column.
        /// </summary>
        private BitSet[] colAvailability;

        /// <summary>
        /// A 2d Array where:
        /// The rows in this array represents the column its 'assigned' to in the Sudoku,
        /// The columns in this array repesents certain values,
        /// The values in this array are sets that hold the rows where the value appears at, in the same column.
        /// </summary>
        private BitSet[,] colAvailabilityCounter;


        /// <summary>
        /// Holds the possible values that can be inserted in each grid.
        /// </summary>
        private BitSet[] gridAvailability;

        /// <summary>
        /// A 2d Array where:
        /// The rows in this array represents the grid its 'assigned' to in the Sudoku,
        /// The columns in this array repesents certain values,
        /// The values in this array are sets that hold the position
        /// from the initial position in the grid where the value appears at, in the same grid.
        /// </summary>
        private BitSet[,] gridAvailabilityCounter;

        /// <summary>
        /// a full BitSet in the Sudoku.
        /// In other words: a full column, a full row or a full grid.
        /// </summary>
        private BitSet full;

        /// <summary>
        /// A Stack that holds the positions and thier values, where an insertion is a neccessity.
        /// </summary>
        private Stack<(int value, int row, int col)> NextGarunteedAction;

        private Stack<Stack<(int row, int col)>> PrevInsertion;
        // FIRST IN PREV INSERTION FOR GUESSING
        // SECOND IN PREV INSERTION FOR INSERTIONS

        private Stack<Stack<Stack<(int value, int row, int col)>>> PrevAction;
        // FIRST IN PREV ACTION FOR GUESSING
        // SECOND IN PREV ACTION FOR INSERTIONS
        // THIRD FOR PREV ACTION FOR POSSIBILITY

        /// <summary>
        /// An array of Bitsets, each representing a full different row
        /// </summary>
        private BitSet[] fullRows;

        /// <summary>
        /// An array of Bitsets, each representing a full different column
        /// </summary>
        private BitSet[] fullCols;

        /// <summary>
        /// The maximum amount of values that can be inserted in the Sudoku
        /// </summary>
        private int size;

        /// <summary>
        /// The amount of values that have been inserted into the Sudokuu
        /// </summary>
        private int count;

        /// <summary>
        /// A constructor that creates a SudokuSolver instance using a Sudoku instance
        /// </summary>
        /// <param name="s"> The base Sudoku to create the instance of </param>
        public SudokuSolver(Sudoku s) : this(s.Array, s.IsGridHorizontal()) { }

        /// <summary>
        /// A constructor that creates an empty SudokuSolver instance.
        /// </summary>
        /// <param name="length"> The length of the Sudoku </param>
        /// <param name="horizontal">
        /// In cases where the grid height and grid length are different,
        /// This value indicates whether the grids are laying down(horizontal), or standing up(vertical).
        /// </param>
        /// <exception cref="InvalidSudokuException"> 
        /// Thrown in base:
        /// Thrown if the length of the sudoku is invalid ,
        /// Either because it is out of range, or because it is a prime number.
        /// </exception>
        protected SudokuSolver(int length = 9, bool horizontal = true) : base(length, horizontal){
            size = sudoku.Length;
            count = 0;

            // Create a BitSet representing a full row/column/grid 
            this.full = new BitSet(length);
            for (int i = 1; i <= length; i++)
                full.Add(i);


            squarePossibilities = new BitSet[length, length];
            for (int row = 0; row < length; row++)
                for (int col = 0; col < length; col++)
                    // Make the possible values for every cell all values (i.e. full)
                    squarePossibilities[row, col] = new BitSet(full);
            

            this.squarePossibilitesCounter = new HashSet<(int, int)>[length + 1];
            for (int i = 0; i <= length; i++)
                squarePossibilitesCounter[i] = new HashSet<(int, int)> (length * length);

            // Add to the last frequency all of the possible positions (since nothing is inserted yet)
            for (int row = 0; row < length; row++)
                for (int col = 0; col < length; col++)
                    squarePossibilitesCounter[length].Add((row,col));


            this.rowAvailability = new BitSet[length];
            for (int row = 0; row < length; row++)
                // Make the possibilities in every row, every possibility.
                rowAvailability[row] = new BitSet(full);

            // Make the possible columns for every row, for every value, all columns.
            this.rowAvailabilityCounter = new BitSet[length,length];
            for (int row = 0; row < length; row++)
                for (int value = 1; value < length + 1; value++)
                    rowAvailabilityCounter[row, value - 1] = new BitSet(full);


            // Make the possibilities in every col, every possibility.
            this.colAvailability = new BitSet[length];
            for (int col = 0; col < length; col++)
                colAvailability[col] = new BitSet(full);

            // Make the possible rows for every column, for every value, all rows.
            this.colAvailabilityCounter = new BitSet[length,length];
            for (int col = 0; col < length; col++)
                for (int value = 1; value < length + 1; value++)
                    colAvailabilityCounter[col, value - 1] = new BitSet(full);


            // Make the possibilities in every grid, every possibility.
            this.gridAvailability = new BitSet[length];
            for (int grid = 0; grid < length; grid++)
                gridAvailability[grid] = new BitSet(full);

            // Make the possible positions in grid for every grid, for every values, all positions.
            this.gridAvailabilityCounter = new BitSet[length, length];
            for (int position = 0; position < length; position++)
                for (int value = 1; value < length + 1; value++)
                    gridAvailabilityCounter[position, value - 1] = new BitSet(full);


            this.NextGarunteedAction = new Stack<(int value, int row, int col)>(length * 3);

            this.PrevAction = new Stack<Stack<Stack<(int value, int row, int col)> > >(length);

            this.PrevInsertion = new Stack<Stack<(int row, int col)>>(length);


            // Insert all full rows into the full rows array
            this.fullRows = new BitSet[grid_height];
            for (int i = 0; i < grid_height; i++) {
                fullRows[i] = new BitSet(grid_width);
                for (int j = 0; j < grid_width; j++)
                    fullRows[i].Add(i * grid_width + j + 1);
            }

            // Insert all full cols into the full cols array
            this.fullCols = new BitSet[grid_width];
            for (int i = 0; i < grid_width; i++) {
                fullCols[i] = new BitSet(sudoku.GetLength(0));
                for (int j = 0; j < grid_height; j++)
                    fullCols[i].Add(i + j * grid_height + 1);
            }
        }
        /// <summary>
        /// A constructor that creates a SudokuSolver instance with initialized values.
        /// </summary>
        /// <param name="grid"> The 2d array that the SudokuSolver would represent </param>
        /// <param name="horizontal">
        /// In cases where the grid height and grid length are different,
        /// This value indicates whether the grids are laying down(horizontal), or standing up(vertical).
        /// </param>
        /// <exception cref="InvalidSudokuException"> 
        /// Thrown if the 2d Array grid is not symmetrical,
        /// Also thrown in base:
        /// Thrown if the length of the sudoku is invalid ,
        /// Either because it is out of range, or because it is a prime number.
        /// </exception>
        public SudokuSolver(int[,] arr, bool horizontal = true) : this(arr.GetLength(0), horizontal)
        {
            int length = arr.GetLength(0);
            // if width is not equal to height: not symmetrical. thus, invalid length.
            if (arr.GetLength(1) != length)
                throw new InvalidSudokuException($"Sudoku size must be N*N, instead was {arr.GetLength(0)}*{arr.GetLength(1)}");

            PrevAction.Push(new Stack<Stack<(int value, int row, int col)>>(length));
            PrevInsertion.Push(new Stack<(int row, int col)>(length));

            // Insert into the current empty Sudoku all values that are in the initialized array
            for (int row = 0; row < sudoku.GetLength(0); row++)
                for (int col = 0; col < sudoku.GetLength(1); col++)
                    if (this.sudoku[row, col] == NONE && arr[row, col] != NONE)
                        Insert(arr[row, col], row, col);

            // Clear the undo stacks, since we wont come back from here
            PrevAction.Clear();
            PrevAction.Push(new Stack<Stack<(int value, int row, int col)>>(length));

            PrevInsertion.Clear();
            PrevInsertion.Push(new Stack<(int row, int col)>(length));

            // Insert the values that their places are guranteed
            InsertGuranteed();
        }

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
            if (PrevInsertion.Count() == 0 || PrevAction.Count() == 0) return;

            while (PrevInsertion.Peek().Count() != 0 && PrevAction.Peek().Count() != 0)
            {
                while (PrevAction.Peek().Peek().Count() != 0)
                {
                    (int valueAc, int rowAc, int colAc) = PrevAction.Peek().Peek().Pop();
                    AddPossibility(valueAc, rowAc, colAc);
                }
                (int rowIn, int colIn) = PrevInsertion.Peek().Pop();
                DeInsert(rowIn, colIn);

                PrevAction.Peek().Pop();
            }
            PrevAction.Pop();
            PrevInsertion.Pop();
            
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
        /// Insert a value into the Sudoku, at a certain position.
        /// </summary>
        /// <param name="value"> The value to be inserted</param>
        /// <param name="row"> A row number</param>
        /// <param name="col"> A column number</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when trying to insert outside of the Sudoku boundaries 
        /// </exception>
        /// <exception cref="InvalidInsertionException">
        /// Thrown when trying to insert an invalid value,
        /// Thrown when collisions accrues after insertion.
        /// </exception>
        public override void Insert(int value, int row, int col)
        {
            if (!allowables.ContainsKey(value))
                throw new InvalidInsertionException($"Cannot Insert '{value}' to {row},{col} in sudoku, Invalid Value!");

            if (!InRange(row,col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");
            
            if (!squarePossibilities[row,col].Contains(value))
                throw new InvalidInsertionException($"Cannot Insert '{value}' to {row},{col} in sudoku");

            // Save this insertion
            PrevInsertion.Peek().Push((row, col));
            // Separate insertions
            PrevAction.Peek().Push(new Stack<(int value, int row, int col)>());

            // Update peers with the same current col (row changes)
            UpdateRowInsert(value, row, col);

            // Update peers with the same current col (row changes)
            UpdateColInsert(value, row, col);

            // Update peers with the same current col (row changes)
            UpdateGridInsert(value, row, col);

            //Update the current cell
            UpdateSquareInsert(value, row, col);

            count++;
        }
        
        /// <summary>
        /// Remove a possiblity from a cell
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
        public void RemoveSquarePossibility(int value, int row, int col, bool changeRow = true, bool changeCol = true, bool changeGrid = true)
        {
            if (!squarePossibilities[row, col].Contains(value)) return;

            int count;

            //Save this possibility removal
            PrevAction.Peek().Peek().Push((value, row, col));
            
            squarePossibilities[row, col].Remove(value);

            count = squarePossibilities[row, col].Count();
            switch (count) {
                // if no remainning possibilities, Sudoku is unsolvable (or current path is)
                case 0: throw new InvalidInsertionException();
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

                // Let count be the number of columns that has the possibility value in the current row
                count = rowAvailabilityCounter[row, value - 1].Count();
                switch (count){
                    // if count is 0: the last position was removed, and value cannot be placed in the row
                    case 0: throw new InvalidInsertionException();
                    // if count is 1: there is only one column for value to be inserted in, for this row
                    case 1: PotentialInsertInRow(value, row); break;
                    // if count is 2: potential XWing (only two in the row)
                    case 2: RowXWing(value, row, col); break;
                    default:
                        // if count is less the Maximum search load: search for hidden groups in the row
                        if (count <= sudoku.GetLength(0) * GROUP_SEARCH_LOAD) HiddenInRow(value, row, col, count);
                        break;
                }

            }

            if (changeCol)
            {
                colAvailabilityCounter[col, value - 1].Remove(row + 1);
                count = colAvailabilityCounter[col, value - 1].Count();
                if (count == 0) throw new InvalidInsertionException();
                else if (count == 1) PotentialInsertInCol(value, col);
                else
                {
                    if (count == 2) ColXWing(value, row, col);
                    if (count <= sudoku.GetLength(0) * GROUP_SEARCH_LOAD) HiddenInCol(value, row, col, count);
                }
            }

            if (changeGrid)
            {
                gridAvailabilityCounter[GridPositionOf(row, col), value - 1].Remove(row % grid_height * grid_width + col % grid_width + 1);
                count = gridAvailabilityCounter[GridPositionOf(row, col), value - 1].Count();
                if (count == 0) throw new InvalidInsertionException();
                else if (count == 1) PotentialInsertInGrid(value, GridPositionOf(row, col));
                else
                {
                    if (count <= grid_width || count <= grid_height) PointingPair(value, GridPositionOf(row, col));
                    if (count <= sudoku.GetLength(0) * GROUP_SEARCH_LOAD) HiddenInGrid(value, row, col, count);
                }
            }
        }

        private void UpdateSquareInsert(int value, int row, int col)
        {
            BitSet possible = squarePossibilities[row, col];

            foreach (int possibilite in possible)
                if (possibilite != value)
                    RemoveSquarePossibility(possibilite, row, col);

            squarePossibilitesCounter[possible.Count()].Remove((row, col));
            squarePossibilitesCounter[0].Add((row, col));

            rowAvailability[row].Remove(value);
            colAvailability[col].Remove(value);
            gridAvailability[GridPositionOf(row, col)].Remove(value);

            rowAvailabilityCounter[row, value - 1].ClearAll();
            colAvailabilityCounter[col, value - 1].ClearAll();
            gridAvailabilityCounter[GridPositionOf(row, col), value - 1].ClearAll();

            squarePossibilities[row, col] = new BitSet(0);

            sudoku[row, col] = value;
        }

        private void UpdateRowInsert(int value, int row, int col)
        {
            for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
            {
                if (GridPositionOf(rowPos, col) == GridPositionOf(row, col)) continue;
                if (sudoku[rowPos, col] != NONE) continue;

                RemoveSquarePossibility(value, rowPos, col, true, false, true);
            }
        }

        private void UpdateColInsert(int value, int row, int col)
        {
            for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
            {
                if (GridPositionOf(row,colPos) == GridPositionOf(row,col)) continue;
                if (sudoku[row, colPos] != NONE) continue;

                RemoveSquarePossibility(value, row, colPos, false, true, true);
            }
        }

        private void UpdateGridInsert(int value, int row, int col)
        {
            int initRow = GridPositionOf(row, col) / (sudoku.GetLength(1) / grid_width) * grid_height;
            int initCol = GridPositionOf(row, col) * grid_width % sudoku.GetLength(1);
            for (int rowPos = 0; rowPos < grid_height; rowPos++)
            {
                for (int colPos = 0; colPos < grid_width; colPos++)
                {
                    if (initRow + rowPos == row && initCol + colPos == col) continue;
                    if (sudoku[initRow + rowPos, initCol + colPos] != NONE) continue;

                    RemoveSquarePossibility(value, initRow + rowPos, initCol + colPos, true, true, false);
                }
            }
        }

        private void PotentialInsertInRow(int value, int row)
        {
            NextGarunteedAction.Push((value, row, (rowAvailabilityCounter[row, value - 1].GetSmallest() - 1)));
        }

        private void PotentialInsertInCol(int value, int col)
        {
            NextGarunteedAction.Push((value, colAvailabilityCounter[col, value - 1].GetSmallest() - 1, col));
        }

        private void PotentialInsertInGrid(int value, int grid)
        {
            int rowInPos = (gridAvailabilityCounter[grid, value - 1].GetSmallest() - 1) / grid_width;
            int colInPos = (gridAvailabilityCounter[grid, value - 1].GetSmallest() - 1) % grid_width;

            int initRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
            int initCol = grid * grid_width % sudoku.GetLength(1);

            NextGarunteedAction.Push((value, initRow + rowInPos, initCol + colInPos));
        }

        public void PotentialInsertInSquare(int row, int col)
        {
            BitSet possibilities = squarePossibilities[row, col];
            NextGarunteedAction.Push((possibilities.GetSmallest(), row, col));
        }


        private void HiddenInGrid(int value, int row, int col, int hidden_type)
        {
            // THERE ARE X SQUARES WHERE V APPEARES IN THE GRID
            // IF THERE ARE X - 1 OTHER VALUES WHO ONLY APPEAR IN THE SAME GRIDS
            // HIDDEN VALUES IN GRID
            int grid = GridPositionOf(row,col);

            // COLUMNS WHERE VALUE APPEARS AT
            BitSet possibilities_in_grid_for_value = gridAvailabilityCounter[grid, value - 1];
            BitSet possibilities_in_grid_other_values = gridAvailability[grid];

            BitSet Hidden = new BitSet(sudoku.GetLength(0));
            foreach (int possibility in possibilities_in_grid_other_values)
            {
                if (gridAvailabilityCounter[grid, possibility - 1].IsEmpty()) continue;
                else if (gridAvailabilityCounter[grid, possibility - 1].IsSubSetOf(possibilities_in_grid_for_value))
                    Hidden.Add(possibility);
                else if (gridAvailabilityCounter[grid, possibility - 1].IsSuperSetOf(possibilities_in_grid_for_value))
                {
                    if (gridAvailabilityCounter[grid, possibility - 1].Count() <= sudoku.GetLength(0) * GROUP_SEARCH_LOAD)
                    {
                        HiddenInGrid(possibility, row, col, gridAvailabilityCounter[grid, possibility - 1].Count());
                        return;
                    }
                }
            }

            if (Hidden.Count() >= hidden_type)
            {
                int initRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
                int initCol = grid * grid_width % sudoku.GetLength(1);
                foreach (int position in possibilities_in_grid_for_value)
                {
                    row = initRow + (position - 1) / grid_width;
                    col = initCol + (position - 1) % grid_width;
                    foreach (int possibility_in_old in squarePossibilities[row, col])
                    {
                        if (Hidden.Contains(possibility_in_old)) continue;
                        RemoveSquarePossibility(possibility_in_old, row, col);
                    }

                }
            }
        }
        
        private void HiddenInRow(int value, int row, int col, int hidden_type)
        {
            // THERE ARE X SQUARES WHERE V APPEARES IN THE ROW
            // IF THERE ARE X - 1 OTHER VALUES WHO ONLY APPEAR IN THE SAME ROW
            // HIDDEN VALUES IN ROW
            BitSet possibilities_in_row_for_value = rowAvailabilityCounter[row, value - 1];
            BitSet possibilities_in_row_other_values = rowAvailability[row];

            BitSet Hidden = new BitSet(sudoku.GetLength(0));
            foreach (int possibility in possibilities_in_row_other_values)
            {
                if (rowAvailabilityCounter[row, possibility - 1].IsEmpty()) continue;
                if (rowAvailabilityCounter[row, possibility - 1].IsSubSetOf(possibilities_in_row_for_value))
                    Hidden.Add(possibility);
                else if (rowAvailabilityCounter[row, possibility - 1].IsSuperSetOf(possibilities_in_row_for_value))
                {
                    if (rowAvailabilityCounter[row, possibility - 1].Count() <= sudoku.GetLength(0) * GROUP_SEARCH_LOAD)
                    {
                        HiddenInRow(possibility, row, col, rowAvailabilityCounter[row, possibility - 1].Count());
                        return;
                    }
                }
            }
            
            if (Hidden.Count() >= hidden_type)
            {
                foreach (int position in possibilities_in_row_for_value)
                {

                    col = position - 1;
                    foreach (int possibility_in_old in squarePossibilities[row, col])
                    {
                        if (Hidden.Contains(possibility_in_old)) continue;
                        RemoveSquarePossibility(possibility_in_old, row, col);
                    }
                }
            }
        }

        private void HiddenInCol(int value, int row, int col, int hidden_type)
        {
            // THERE ARE X SQUARES WHERE V APPEARES IN THE COL
            // IF THERE ARE X - 1 OTHER VALUES WHO ONLY APPEAR IN THE SAME COL
            // HIDDEN VALUES IN COL
            BitSet possibilities_in_col_for_value = colAvailabilityCounter[col, value - 1];
            BitSet possibilities_in_col_other_values = colAvailability[col];
            BitSet Hidden = new BitSet(sudoku.GetLength(0));
            foreach (int possibility in possibilities_in_col_other_values)
            {
                if (colAvailabilityCounter[col, possibility - 1].IsEmpty()) continue;
                if (colAvailabilityCounter[col, possibility - 1].IsSubSetOf(possibilities_in_col_for_value))
                    Hidden.Add(possibility);
                else if (colAvailabilityCounter[col, possibility - 1].IsSuperSetOf(possibilities_in_col_for_value))
                {
                    if (colAvailabilityCounter[col, possibility - 1].Count() <= sudoku.GetLength(0) * GROUP_SEARCH_LOAD)
                    {
                        HiddenInCol(possibility, row, col, colAvailabilityCounter[col, possibility - 1].Count());
                        return;
                    }
                }
            }
            row = possibilities_in_col_for_value.GetSmallest() - 1;


            if (Hidden.Count() >= hidden_type)
            {
                foreach (int position in possibilities_in_col_for_value)
                {
                    row = position - 1;
                    foreach(int possibility_in_old in squarePossibilities[row, col])
                    {
                        if (Hidden.Contains(possibility_in_old)) continue;
                        RemoveSquarePossibility(possibility_in_old, row, col);
                    }
                }
            }
        }

        public bool IsRowPointingGroup(int value, int grid)
        {
            BitSet possitions = gridAvailabilityCounter[grid, value - 1];
            if (possitions.IsEmpty()) return false;

            foreach (BitSet possibleRow in fullRows)
                if (possitions.IsSubSetOf(possibleRow))
                    return true;
            return false;
        }

        public bool IsColPointingGroup(int value, int grid)
        {
            BitSet possitions = gridAvailabilityCounter[grid, value - 1];
            if (possitions.IsEmpty()) return false;

            foreach (BitSet possibleRow in fullCols)
                if (possitions.IsSubSetOf(possibleRow))
                    return true;
            return false;
        }

        public void PointingPair(int value, int grid)
        {
            if (IsRowPointingGroup(value, grid))
            {
                int initRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
                int row = initRow + (gridAvailabilityCounter[grid, value - 1].GetSmallest() - 1) / grid_width;
                for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
                {
                    if (GridPositionOf(row, colPos) == grid) continue;
                    RemoveSquarePossibility(value, row, colPos);
                }
            }
            else if (IsColPointingGroup(value, grid))
            {
                int initCol = grid * grid_width % sudoku.GetLength(1);
                int col = initCol + (gridAvailabilityCounter[grid, value - 1].GetSmallest() - 1) % grid_width;
                for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
                {
                    if (GridPositionOf(rowPos, col) == grid) continue;
                    RemoveSquarePossibility(value, rowPos, col);
                }
            }
        }

        int i = 0;
        public void RowXWing(int value, int row, int col)
        {
            if (rowAvailabilityCounter[row, value - 1].Count() == 2)
            {
                int col1 = rowAvailabilityCounter[row, value - 1].GetSmallest() - 1;
                int col2 = rowAvailabilityCounter[row, value - 1].GetLargest() - 1;
                int row2 = -1;
                foreach (int rowPos in colAvailabilityCounter[col1, value - 1])
                {
                    if (rowPos - 1 == row) continue;
                    if (rowAvailabilityCounter[rowPos - 1, value - 1].Equals(rowAvailabilityCounter[row, value - 1]))
                    {
                        row2 = rowPos - 1;
                        break;
                    }
                }

                if (row2 != -1)
                {
                    for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
                    {
                        if (rowPos == row || rowPos == row2) continue;

                        RemoveSquarePossibility(value, rowPos, col1);
                        RemoveSquarePossibility(value, rowPos, col2);
                    }
                }
            }
        }

        public void ColXWing(int value, int row, int col)
        {
            if (colAvailabilityCounter[col, value - 1].Count() == 2)
            {
                int row1 = colAvailabilityCounter[col, value - 1].GetSmallest() - 1;
                int row2 = colAvailabilityCounter[col, value - 1].GetLargest() - 1;
                int col2 = -1;
                foreach (int colPos in rowAvailabilityCounter[row1, value - 1])
                {
                    if (colPos - 1 == col) continue;
                    if (colAvailabilityCounter[colPos - 1, value - 1].Equals(colAvailabilityCounter[col, value - 1]))
                    {
                        col2 = colPos - 1;
                        break;
                    }
                }
                if (col2 != -1)
                {
                    for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
                    {
                        if (colPos == col || colPos == col2) continue;

                        RemoveSquarePossibility(value, row1, colPos);
                        RemoveSquarePossibility(value, row2, colPos);
                    }
                }
            }

        }

        public override bool CanInsert(int value, int row, int col)
        {
            if (sudoku[row, col] == NONE || !InRange(row,col)) return true;
            return !BitSet.Union(rowAvailability[row], colAvailability[col], gridAvailability[GridPositionOf(row, col)]).Contains(value);
        }

        public bool IsSolved()
        {
            return size <= count;
        }

        public void printPoss()
        {
            for(int i = 0; i < squarePossibilities.GetLength(0); i++)
            {
                Console.Write($"{i}:");

                for (int j = 0; j < squarePossibilities.GetLength(1); j++)
                {
                    String msg = "";
                    Console.Write(j+":");
                    foreach (int v in squarePossibilities[i, j])
                        msg += v;
                    Console.Write($"{msg, -10}");
                }
                Console.WriteLine();
            }
        }

        public IEnumerator<Sudoku> GetEnumerator()
        {
            (int, int)? min = MinimumPossibilitySquare();
            if (min == null)
            {
                if (IsSolved()) yield return new Sudoku(this);
                yield break;
            }

            (int row, int col) = (min.Value.Item1, min.Value.Item2);

            foreach (int possibility in squarePossibilities[row, col])
            {
                PrevAction.Push(new Stack<Stack<(int value, int row, int col)>>(sudoku.GetLength(0)));
                PrevInsertion.Push(new Stack<(int row, int col)>(sudoku.GetLength(0)));

                bool flag = true;

                try {
                    Insert(possibility, row, col);
                    InsertGuranteed();
                }
                catch (InvalidInsertionException)
                {
                    flag = false;
                    NextGarunteedAction.Clear();
                }

                if (flag)
                    foreach (Sudoku s in this)
                        yield return s;

                RemoveLatestGuess();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
