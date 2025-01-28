using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Intrinsics.X86;
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
        /// Thrown when collisions accrues after insertion,
        /// Thrown when the Sudoku board becomes unsolvable.
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
        private void RemoveSquarePossibility(int value, int row, int col, bool changeRow = true, bool changeCol = true, bool changeGrid = true)
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

                // Count is the number of columns that has the possibility 'value' in the current row
                count = rowAvailabilityCounter[row, value - 1].Count();
                switch (count){
                    // if count is 0: the last position where value appears was removed, and value can no longer be placed in a certain row
                    case 0: throw new InvalidInsertionException();
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
                    case 0: throw new InvalidInsertionException();
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

                switch (count){
                    // if count is 0: the last position where value appears at was removed, and value can no longer be placed in a certain grid
                    case 0: throw new InvalidInsertionException();
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
                if (GridPositionOf(row,colPos) == GridPositionOf(row,col)) continue;
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

            int grid = GridPositionOf(row,col);

            // Positions where value appears at in the grid
            BitSet possibilities_in_grid_for_value = gridAvailabilityCounter[grid, value - 1];

            // A set holding all values that appear only in the same cells as VALUE in the grid
            BitSet hidden = new BitSet(sudoku.GetLength(0));

            // look at every possible value in the grid
            foreach (int possibility in gridAvailability[grid])
            {
                // if the cells of possibility in the grid are a subset of the cells of value, add to the hidden set
                if (gridAvailabilityCounter[grid, possibility - 1].IsSubSetOf(possibilities_in_grid_for_value))
                    hidden.Add(possibility);

                // if the cells of possibility in the grid are a superset of the cells of value,
                // possibility might have the cells where the hidden group appears at,
                // that is as long as the amount of cells is less then the Max search group load
                else if (gridAvailabilityCounter[grid, possibility - 1].IsSuperSetOf(possibilities_in_grid_for_value))
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
                if (rowAvailabilityCounter[row, possibility - 1].IsSubSetOf(possibilities_in_row_for_value))
                    hidden.Add(possibility);

                // if the cells of possibility in the row are a superset of the cells of value,
                // possibility might have the cells where the hidden group appears at,
                // that is as long as the amount of cells is less then the Max search group load
                else if (rowAvailabilityCounter[row, possibility - 1].IsSuperSetOf(possibilities_in_row_for_value))
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
                if (colAvailabilityCounter[col, possibility - 1].IsSubSetOf(possibilities_in_col_for_value))
                    hidden.Add(possibility);

                // if the cells of possibility in the column are a superset of the cells of value,
                // possibility might have the cells where the hidden group appears at,
                // that is as long as the amount of cells is less then the Max search group load
                else if (colAvailabilityCounter[col, possibility - 1].IsSuperSetOf(possibilities_in_col_for_value))
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
                if (possitions.IsSubSetOf(possibleRow))
                    return true;
            return false;
        }

        /// <summary>
        /// Checks whether there is a Column Pointing group in a grid
        /// </summary>
        /// <param name="value"> the potential value of the pointing group </param>
        /// <param name="grid"> the grid where the group may appear</param>
        /// <returns> Whether there is a Row Pointing group in a grid </returns>
        public bool IsColPointingGroup(int value, int grid)
        {
            // a set of the cells where value appears at in the grid
            BitSet possitions = gridAvailabilityCounter[grid, value - 1];
            if (possitions.IsEmpty()) return false;

            // if the cells positions are a subset of any full column, its a pointing group
            foreach (BitSet possibleRow in fullCols)
                if (possitions.IsSubSetOf(possibleRow))
                    return true;

            return false;
        }
        /// <summary>
        /// A function to search and excecute a pointing group.
        /// </summary>
        /// <param name="value"> The value that is potentially the pointing group </param>
        /// <param name="grid"> the grid where the group may appear</param>
        public void PointingGroup(int value, int grid)
        {
            if (IsRowPointingGroup(value, grid))
            {
                // The initial row of the grid
                int initRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
                // The row where value appears at (and by design where the pointing griup is)
                int row = initRow + (gridAvailabilityCounter[grid, value - 1].GetSmallest() - 1) / grid_width;

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
                int col = initCol + (gridAvailabilityCounter[grid, value - 1].GetSmallest() - 1) % grid_width;

                // Remove all possibilities for value in all other grids, besides the grid of the pointing group
                for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
                    if (GridPositionOf(rowPos, col) != grid)
                        RemoveSquarePossibility(value, rowPos, col);
            }
        }

        /// <summary>
        /// A function to search and excecute an XWing for a row,
        /// where value appears only twice in the row.
        /// </summary>
        /// <param name="value"> The potential value in the XWing </param>
        /// <param name="row"> The row position of the value </param>
        /// <param name="col"> The column position of the value</param>
        /// <remarks> VALUE appears only twice in the row! </remarks>
        public void RowXWing(int value, int row, int col)
        {
            // First column of appearance
            int col1 = rowAvailabilityCounter[row, value - 1].GetSmallest() - 1;
            // Second column of appearance
            int col2 = rowAvailabilityCounter[row, value - 1].GetLargest() - 1;

            int row2 = -1;

            // go through every appearance of value in the two columns he appears
            foreach (int rowPos in colAvailabilityCounter[col1, value - 1])
            {
                if (rowPos - 1 != row)
                {
                    // if a certain, difference row, has the exact same two columns appearance: its the second row of the XWing
                    if (rowAvailabilityCounter[rowPos - 1, value - 1].Equals(rowAvailabilityCounter[row, value - 1]))
                    {
                        row2 = rowPos - 1;
                        break;
                    }
                }
            }
            // If no other row found, no XWing
            if (row2 == -1) return;

            // Remove all other positions that are not in the XWing (not in the same rows)
            for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
            {
                if (rowPos != row && rowPos != row2)
                {
                    RemoveSquarePossibility(value, rowPos, col1);
                    RemoveSquarePossibility(value, rowPos, col2);
                }
            }
        }

        /// <summary>
        /// A function to search and excecute an XWing for a column,
        /// where value appears only twice in the column.
        /// </summary>
        /// <param name="value"> The potential value in the XWing </param>
        /// <param name="row"> The row position of the value </param>
        /// <param name="col"> The column position of the value</param>
        /// <remarks> VALUE appears only twice in the column! </remarks>
        public void ColXWing(int value, int row, int col)
        {
            // First row of appearance
            int row1 = colAvailabilityCounter[col, value - 1].GetSmallest() - 1;
            // Second row of appearance
            int row2 = colAvailabilityCounter[col, value - 1].GetLargest() - 1;

            int col2 = -1;

            // go through every appearance of value in the two rows he appears
            foreach (int colPos in rowAvailabilityCounter[row1, value - 1])
            {
                if (colPos - 1 != col)
                {
                    // if a certain, difference column, has the exact same two rows appearance: its the second column of the XWing
                    if (colAvailabilityCounter[colPos - 1, value - 1].Equals(colAvailabilityCounter[col, value - 1]))
                    {
                        col2 = colPos - 1;
                        break;
                    }
                }
            }

            // If no other column found, no XWing
            if (col2 == -1) return;

            // Remove all other positions that are not in the XWing (not in the same column)
            for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
            {
                if (colPos == col || colPos == col2) continue;

                RemoveSquarePossibility(value, row1, colPos);
                RemoveSquarePossibility(value, row2, colPos);
            }

        }

        public override bool CanInsert(int value, int row, int col)
        {
            if (sudoku[row, col] == NONE || !InRange(row,col)) return true;
            return !BitSet.Union(rowAvailability[row], colAvailability[col], gridAvailability[GridPositionOf(row, col)]).Contains(value);
        }

        /// <summary>
        /// Checks whether the Sudoku is full/solved
        /// </summary>
        /// <returns> Whether the Sudoku is solved or not </returns>
        public bool IsSolved()
        {
            return size <= count;
        }

        /// <summary>
        /// Fully solves the Sudoku if not yes solved,
        /// and in cases of more then one answer, solves all
        /// </summary>
        /// <returns> IEnumerator with all Sudoku answers as Sudoku instances </returns>
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
