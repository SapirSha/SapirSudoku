// This file contains main part of the SudokuSolver class

using System.Collections;
using SapirSudoku.src.Exceptions;
using SapirSudoku.src.DataStructures;

namespace SapirSudoku.src.SolveSudoku
{
    /// <summary>
    ///  A class that solve a Sudoku.
    /// </summary>
    public partial class SudokuSolver : Sudoku, IEnumerable<Sudoku>
    {
        /// <summary> How big of a maximum hidden group to check for compared to length </summary>
        private static readonly float GROUP_SEARCH_LOAD = 0.5f;


        /// <summary>
        /// A frequency array:
        /// Every HashSet in the array holds the positions where
        /// the amount of possible insertions is equal to the index
        /// </summary>
        private HashSet<(int row, int col)>[] squarePossibilitesCounter;

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

        /// <summary>
        /// This class is designed to diffrentiate the insertion made.
        /// Each insertion is its own class, with his own possibility removals
        /// </summary>
        private class Insertion
        {
            public int row;
            public int col;

            public Stack<(int value, int row, int col)> possibilities = new Stack<(int value, int row, int col)>();
            public Insertion(int row, int col)
            {
                this.row = row;
                this.col = col;
            }
        }

        /// <summary>
        /// This class is designed to diffrentiate the guesses made.
        /// Each guess is its own class, with his own insertions and possibility removals(in insertions).
        /// </summary>
        private class Guess
        {
            // a stack of insertions with property initialization (to automatically create the stack)
            public Stack<Insertion> insertions = new Stack<Insertion>();
        }

        /// <summary>
        /// A stack of guesses.
        /// With it its possible to go revert back to the latest unsure insertion, in case of solving, or unsolvable.
        /// </summary>
        private Stack<Guess> previousActions;

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
        public SudokuSolver(Sudoku s) : this(s.Board, s.IsGridHorizontal()) { }

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
        protected SudokuSolver(int length = 9, bool horizontal = true) : base(length, horizontal)
        {
            size = sudoku.Length;
            count = 0;

            // Create a BitSet representing a full row/column/grid 
            full = new BitSet(length);
            for (int i = 1; i <= length; i++)
                full.Add(i);


            squarePossibilities = new BitSet[length, length];
            for (int row = 0; row < length; row++)
                for (int col = 0; col < length; col++)
                    // Make the possible values for every cell all values (i.e. full)
                    squarePossibilities[row, col] = new BitSet(full);


            squarePossibilitesCounter = new HashSet<(int, int)>[length + 1];
            for (int i = 0; i <= length; i++)
                squarePossibilitesCounter[i] = new HashSet<(int, int)>(length * length);

            // Add to the last frequency all of the possible positions (since nothing is inserted yet)
            for (int row = 0; row < length; row++)
                for (int col = 0; col < length; col++)
                    squarePossibilitesCounter[length].Add((row, col));


            rowAvailability = new BitSet[length];
            for (int row = 0; row < length; row++)
                // Make the possibilities in every row, every possibility.
                rowAvailability[row] = new BitSet(full);

            // Make the possible columns for every row, for every value, all columns.
            rowAvailabilityCounter = new BitSet[length, length];
            for (int row = 0; row < length; row++)
                for (int value = 1; value < length + 1; value++)
                    rowAvailabilityCounter[row, value - 1] = new BitSet(full);


            // Make the possibilities in every col, every possibility.
            colAvailability = new BitSet[length];
            for (int col = 0; col < length; col++)
                colAvailability[col] = new BitSet(full);

            // Make the possible rows for every column, for every value, all rows.
            colAvailabilityCounter = new BitSet[length, length];
            for (int col = 0; col < length; col++)
                for (int value = 1; value < length + 1; value++)
                    colAvailabilityCounter[col, value - 1] = new BitSet(full);


            // Make the possibilities in every grid, every possibility.
            gridAvailability = new BitSet[length];
            for (int grid = 0; grid < length; grid++)
                gridAvailability[grid] = new BitSet(full);

            // Make the possible positions in grid for every grid, for every values, all positions.
            gridAvailabilityCounter = new BitSet[length, length];
            for (int position = 0; position < length; position++)
                for (int value = 1; value < length + 1; value++)
                    gridAvailabilityCounter[position, value - 1] = new BitSet(full);


            NextGarunteedAction = new Stack<(int value, int row, int col)>(length * 3);

            previousActions = new Stack<Guess>(length * 3);


            // Insert all full rows into the full rows array
            fullRows = new BitSet[grid_height];
            for (int i = 0; i < grid_height; i++)
            {
                fullRows[i] = new BitSet(grid_width);
                for (int j = 0; j < grid_width; j++)
                    fullRows[i].Add(i * grid_width + j + 1);
            }

            // Insert all full cols into the full cols array
            fullCols = new BitSet[grid_width];
            for (int i = 0; i < grid_width; i++)
            {
                fullCols[i] = new BitSet(sudoku.GetLength(0));
                for (int j = 0; j < grid_height; j++)
                    fullCols[i].Add(i + j * grid_width + 1);
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
                throw new InvalidSudokuSizeException($"Sudoku size must be N*N, instead was {arr.GetLength(0)}*{arr.GetLength(1)}");

            // consider the start as a new guess
            previousActions.Push(new Guess());

            // Insert into the current empty Sudoku all values that are in the initialized array
            for (int row = 0; row < sudoku.GetLength(0); row++)
                for (int col = 0; col < sudoku.GetLength(1); col++)
                    if (sudoku[row, col] == NONE && arr[row, col] != NONE)
                        Insert(arr[row, col], row, col);

            // Clear the undo stacks, since we wont come back from here
            previousActions.Clear();
            previousActions.Push(new Guess());

            // Insert the values that their places are guranteed
            InsertGuranteed();
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
                throw new InvalidValueException($"Cannot Insert '{value}' to {row},{col} in sudoku, Invalid Value!");

            if (!InRange(row, col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");

            if (!squarePossibilities[row, col].Contains(value))
                throw new InvalidInsertionException($"Cannot Insert '{value}' to {row},{col} in sudoku, Collisions accrued!");


            // Save current insertion as a part of current guess
            Guess currentGuess = previousActions.Peek();
            currentGuess.insertions.Push(new Insertion(row, col));

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


        public override bool CanInsert(int value, int row, int col)
        {
            if (sudoku[row, col] != NONE || !InRange(row, col)) return false;
            return squarePossibilities[row, col].Contains(value);
        }

        /// <summary>
        /// Checks whether the Sudoku is full/solved
        /// </summary>
        /// <returns> Whether the Sudoku is solved or not </returns>
        public bool IsSolved()
        {
            return count >= size;
        }

        /// <summary>
        /// Fully solves the Sudoku if not yes solved,
        /// and in cases of more then one answer, solves all
        /// </summary>
        /// <returns> IEnumerator with all Sudoku answers as Sudoku instances </returns>
        public IEnumerator<Sudoku> GetEnumerator()
        {
            // Get the position of the square with the minimum possibilities to insert to
            (int, int)? min = MinimumPossibilitySquare();

            // if there are no squares with possibilities its either solved or unsolvable
            if (min is null)
            {
                if (IsSolved()) yield return new Sudoku(this);
                yield break;
            }

            (int row, int col) = (min.Value.Item1, min.Value.Item2);

            // try to insert every possibility, and for each possibility, try to solve the Sudoku
            foreach (int possibility in squarePossibilities[row, col])
            {
                // since its an unsure insertion, consider it as a guess,
                // and push it into a new item in the stack to later revert to
                previousActions.Push(new Guess());

                bool solvable = true;

                try
                {
                    Insert(possibility, row, col);
                    InsertGuranteed(); // insert all the guarenteed actions
                }
                // caught when an insertion is invalid, or the board becomes unsolvable
                catch (InvalidInsertionException)
                {
                    solvable = false;
                    // clear leftover from the stack
                    NextGarunteedAction.Clear();
                }

                if (solvable)
                    foreach (Sudoku s in this)
                        // return all the answers that the possibility got
                        yield return s;

                /* Remove the latest guess, in other words, remove everything since the last insertion in this function
                 * and prepare it to insert a new possibility */
                RemoveLatestGuess();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
