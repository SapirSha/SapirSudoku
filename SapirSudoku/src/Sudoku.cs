using SapirSudoku.src.Exceptions;
using SapirSudoku.src.Utilities;
using SapirSudoku.src.SolveSudoku;
using SapirSudoku.src.DataStructures.BitSet;

namespace SapirSudoku.src
{
    /// <summary>
    /// A class that represents a sudoku.
    /// </summary>
    public class Sudoku
    {
        protected static readonly int MAX_SUDOKU_LENGTH = 25;
        protected static readonly int MIN_SUDOKU_LENGTH = 1;


        /// <summary> Represents the value that is not considered a clue in the Sudoku </summary>
        protected static readonly int NONE = 0;
        /// <summary> Holds the allowed values in the Sudoku </summary>
        protected Dictionary<int, int> allowables = new Dictionary<int, int> { { NONE, 0 } };

        /// <summary> Represents the Sudoku itself and it's inserted values </summary>
        protected int[,] sudoku;

        /// <summary> A clone of the 2d array that represents the Sudoku </summary>
        public int[,] Array { get { return (int[,])sudoku.Clone(); } }


        protected int grid_width;
        public int GridWidth { get { return grid_width; } }


        protected int grid_height;
        public int GridHeight { get { return grid_height; } }

        /// <returns> Whther the grids in the sudoku are lying down (horizontally =) </returns>
        public bool IsGridHorizontal() { return grid_width >= grid_height; }

        /// <returns> Whther the grids in the sudoku are standing up (vertically) </returns>
        public bool IsGridVertical() { return grid_height >= grid_width; }

        /// <summary>
        /// A Constructor to create a new Sudoku instance, with the same fields of another Sudoku instance.
        /// </summary>
        /// <param name="sudoku"> The Sudoku to copy </param>
        public Sudoku(Sudoku sudoku)
        {
            // Clone 2d array with values
            this.sudoku = sudoku.Array;
            grid_height = sudoku.grid_height;
            grid_width = sudoku.grid_width;
        }
        /// <summary>
        /// A constructor to create a new Sudoku instance, with no clues in it.
        /// </summary>
        /// <param name="length"> The length/type of the sudoku </param>
        /// <param name="horizontal">
        /// In cases where the grid height and grid length are different,
        /// This value indicates whether the grids are laying down(horizontal), or standing up(vertical).
        /// </param>
        /// <exception cref="InvalidSudokuException"> 
        /// Thrown if the length of the sudoku is invalid ,
        /// Either because it is out of range, or because it is a prime number
        /// </exception>
        protected Sudoku(int length = 9, bool horizontal = true)
        {
            if (length < MIN_SUDOKU_LENGTH)
                throw new InvalidSudokuException("Minimum Sudoku Length is 1");

            if (length > MAX_SUDOKU_LENGTH)
                throw new InvalidSudokuException("Maximum Sudoku Length is 25");

            // Add the allowed numbers to the allowed numbers hashset
            for (int i = 1; i <= length; i++)
                allowables.Add(i, i);

            // Get the two closest divisibles to the length
            (int smaller, int bigger) = MathUtilities.ClosestMultiplications(length);


            if (smaller == 1)
                if (length != 1)
                    throw new InvalidSudokuException("Cannot Create a Sudoku with a prime number as length");

            if (horizontal)
            {
                // the grids are lying down
                grid_height = smaller;
                grid_width = bigger;
            }
            else
            {
                // the grids are standing up
                grid_height = bigger;
                grid_width = smaller;
            }

            sudoku = new int[length, length];
        }

        /// <summary>
        ///A constructor to create a new Sudoku instance, with initialized values.
        /// </summary>
        /// <param name="sudoku"> The 2d array that represents the sudoku </param>
        /// <param name="horizontal"> 
        /// In cases where the grid height and grid length are different,
        /// This value indicates whether the grids are laying down, or standing up.
        /// </param>
        /// <exception cref="InvalidSudokuException"> 
        /// Thrown when a sudoku length is invalid:
        /// The Length is too big, The Length is too small, The size of the representing array is not symmetrical
        /// </exception>
        /// <exception cref="InvalidInsertionException"> Thrown at Insert function: The value could not be inserted </exception>
        public Sudoku(int[,] sudoku, bool horizontal = true) : this(sudoku.GetLength(0), horizontal)
        {
            int length = sudoku.GetLength(0);
            // the 2d array is not symmetrical 
            if (sudoku.GetLength(1) != length)
                throw new InvalidSudokuException($"Sudoku size must be N*N, instead was {sudoku.GetLength(0)}*{sudoku.GetLength(1)}");

            // Insert all the values in the params 2d array to the class 2d array
            for (int row = 0; row < sudoku.GetLength(0); row++)
                for (int col = 0; col < sudoku.GetLength(1); col++)
                    if (sudoku[row, col] != NONE)
                        Insert(sudoku[row, col], row, col);
        }

        /// <summary>
        /// Checks whether a certain row values, representing a row and a column,
        /// are inside the 2d array which represents the sudoku.
        /// </summary>
        /// <param name="row"> A row number </param>
        /// <param name="col"> A column number </param>
        /// <returns> Whether the row and column are inside the 2d array. </returns>
        public bool InRange(int row, int col)
        {
            if (row < 0 || col < 0 || row >= sudoku.GetLength(0) || col >= sudoku.GetLength(1))
                return false;
            return true;
        }
        /// <summary>
        /// A function to get the grid number, which a certain row and column appear at
        /// </summary>
        /// <param name="row"> A row number </param>
        /// <param name="col"> A col number </param>
        /// <returns> The grid number which the position [row,col] appears at. </returns>
        protected int GridPositionOf(int row, int col)
        {
            return row / grid_height * (sudoku.GetLength(1) / grid_width) + col / grid_width;
        }

        /// <summary>
        /// Remove the number in position [row, col] from the sudoku.
        /// </summary>
        /// <param name="row"> A row number </param>
        /// <param name="col"> A column number </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when row or col are out of the 2d array which represents the Sudoku.
        /// </exception>
        public virtual void Remove(int row, int col)
        {
            if (!InRange(row, col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");

            sudoku[row, col] = NONE;
        }
        /// <summary>
        /// A function to insert a number into the Sudoku.
        /// The function will insert 'value' into position [row,col].
        /// </summary>
        /// <param name="value"> The number to insert </param>
        /// <param name="row"> A row number </param>
        /// <param name="col"> A col number</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the position isnt inside the 2d array which represents the Sudoku
        /// </exception>
        /// <exception cref="InvalidInsertionException">
        /// Thrown when the Insertion failed: the value is not allowed to be inserted
        /// </exception>
        public virtual void Insert(int value, int row, int col)
        {
            if (!InRange(row, col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");

            if (value == NONE) { Remove(row, col); return; }

            if (allowables.ContainsKey(value))
                sudoku[row, col] = value;
            else
                throw new InvalidInsertionException($"Cannot Insert '{value}' to {row},{col} in sudoku");
        }

        /// <summary>
        /// Checks whether the sudoku is valid 
        /// (no collisions between values accrued, including grid, rows and columns collisions)
        /// </summary>
        /// <returns> If the Sudoku represented has no collisions </returns>
        public bool IsValid()
        {
            // remember value got in every row
            BitSet[] rows = new BitSet[sudoku.GetLength(0)];
            for (int i = 0; i < rows.Length; i++) rows[i] = new BitSet(sudoku.GetLength(0));

            // remember value got in every column
            BitSet[] cols = new BitSet[sudoku.GetLength(0)];
            for (int i = 0; i < cols.Length; i++) cols[i] = new BitSet(sudoku.GetLength(0));

            // remember value got in every grid
            BitSet[] grids = new BitSet[sudoku.GetLength(0)];
            for (int i = 0; i < grids.Length; i++) grids[i] = new BitSet(sudoku.GetLength(0));


            for (int row = 0; row < sudoku.GetLength(0); row++)
            {
                for (int col = 0; col < sudoku.GetLength(1); col++)
                {
                    int cur = sudoku[row, col];

                    if (cur == NONE) continue;

                    int gridPosition = GridPositionOf(row, col);

                    if (rows[row].Contains(cur) || cols[col].Contains(cur) || grids[gridPosition].Contains(cur))
                        // if the looked at value already is inside the set, a collision accrued
                        return false;
                    else
                    {
                        // else add the value to the sets
                        rows[row].Add(cur);
                        cols[col].Add(cur);
                        grids[gridPosition].Add(cur);
                    }

                }
            }
            // if no collisions accrued return true
            return true;
        }

        /// <summary>
        /// Checks if inserting a value into a position would result in any collisions.
        /// </summary>
        /// <param name="value"> The number to look at </param>
        /// <param name="row"> A row number </param>
        /// <param name="col"> A col number</param>
        /// <returns> Whether there would be no collisions after inserting 'value' into position [row,col] </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when row or col are outside the boundaries of the Sudoku
        /// </exception>
        public virtual bool CanInsert(int value, int row, int col)
        {
            if (!InRange(row, col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");

            // if cell already occupied: return false
            if (sudoku[row, col] != NONE) return false;

            // if value appears in the same col: return false (row changes)
            for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
                if (sudoku[rowPos, col] == value)
                    return false;

            // if value appears in the same row: return false (col changes)
            for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
                if (sudoku[row, colPos] == value)
                    return false;

            // first row in grid
            int initRow = GridPositionOf(row, col) / (sudoku.GetLength(1) / grid_width) * grid_height;
            // first column in grid
            int initCol = GridPositionOf(row, col) * grid_width % sudoku.GetLength(1);

            // if value appears in the same grid: return false
            for (int rowPos = 0; rowPos < grid_height; rowPos++)
                for (int colPos = 0; colPos < grid_width; colPos++)
                    if (sudoku[initRow + rowPos, initCol + colPos] == value)
                        return false;

            // if the value did not appear in the same row, column or grid: return true
            return true;
        }

        /// <summary>
        /// Converts the Sudoku represented to an appropriate String
        /// </summary>
        /// <returns> A string representing the Sudoku </returns>
        public override string ToString()
        {
            string msg = "";
            for (int row = 0; row < sudoku.GetLength(0); row++)
            {
                if (row != 0 && row % grid_height == 0)
                    msg += "\n\n";

                for (int col = 0; col < sudoku.GetLength(1); col++)
                {
                    if (col != 0 && col % grid_width == 0)
                        msg += "   ";
                    msg += $"{sudoku[row, col],-3}";
                }

                msg += "\n";
            }

            return msg;
        }

        /// <summary>
        /// All solutions for the current Sudoku that is represented.
        /// </summary>
        public IEnumerable<Sudoku> Answers
        {
            get
            {
                SudokuSolver solver;
                // try to create a solver instance with this instance
                try { solver = new SudokuSolver(this); }
                // if Invalid Insertion accrued: no solutions for the current Sudoku
                catch (InvalidInsertionException) { yield break; }

                // else return the Sudoku represented by each answer
                foreach (Sudoku answer in solver)
                    yield return answer;
            }
        }
    }
}