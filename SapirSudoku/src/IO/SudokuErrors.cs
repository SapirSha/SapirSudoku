using SapirSudoku.src.DataStructures;

namespace SapirSudoku.src.IO
{
    /// <summary>
    /// A Helper class that looks for specific error and gives more specific messages
    /// </summary>
    public static class SudokuErrors
    {
        /// <summary>
        /// a class that holds all the collisions that appear for a certain value
        /// </summary>
        public class Collisions
        {
            public int value;
            public bool has_collisions;
            // the rows where collision accrued for the value
            public HashSet<int> rowCollisions = new HashSet<int>();
            // the columns where collision accrued for the value
            public HashSet<int> colCollisions = new HashSet<int>();
            // the grids where collision accrued for the value
            public HashSet<int> gridCollisions = new HashSet<int>();
        }

        /// <summary>
        /// Search for collisions in a sudoku
        /// </summary>
        /// <param name="s"> The sudoku instance with potentiall collisions </param>
        /// <returns></returns>
        public static List<Collisions> SearchCollisions(Sudoku s)
        {
            int[,] sudoku = s.Board;
            // every index in the array represent a value
            Collisions[] collisions = new Collisions[sudoku.GetLength(0) + 1];

            for(int i = 0; i < collisions.Length; i++)
            {
                collisions[i] = new Collisions();
                collisions[i].has_collisions = false;
            }

            // Create a set to remember all the values that appear in the row
            // every index in the rows array is the index of the row
            BitSet[] rows = new BitSet[sudoku.GetLength(0)];
            for (int i = 0; i < rows.Length; i++) 
                rows[i] = new BitSet(sudoku.GetLength(0));

            // Create a set to remember all the values that appear in the column
            // every index in the cols array is the index of the column
            BitSet[] cols = new BitSet[sudoku.GetLength(0)];
            for (int i = 0; i < cols.Length; i++) 
                cols[i] = new BitSet(sudoku.GetLength(0));

            // Create a set to remember all the values that appear in the grid
            // every index in the grids array is the index of the grid
            BitSet[] grids = new BitSet[sudoku.GetLength(0)];
            for (int i = 0; i < grids.Length; i++) 
                grids[i] = new BitSet(sudoku.GetLength(0));


            for (int row = 0; row < sudoku.GetLength(0); row++)
            {
                for (int col = 0; col < sudoku.GetLength(1); col++)
                {
                    int cur = sudoku[row, col];
                    if (cur == Sudoku.NONE) continue;

                    int gridPosition = s.GridPositionOf(row, col);

                    // if the value was already in the row or column or grid a collision accrued
                    if (rows[row].Contains(cur) || cols[col].Contains(cur) || grids[gridPosition].Contains(cur))
                    {
                        collisions[cur].value = cur;
                        collisions[cur].has_collisions = true;
                        // add to the appropriate collision type

                        if (rows[row].Contains(cur))
                            collisions[cur].rowCollisions.Add(row);

                        if (cols[col].Contains(cur))
                            collisions[cur].colCollisions.Add(col);

                        if (grids[gridPosition].Contains(cur))
                            collisions[cur].gridCollisions.Add(gridPosition);
                    }

                    // Add the value to the set
                    rows[row].Add(cur);
                    cols[col].Add(cur);
                    grids[gridPosition].Add(cur);
                }
            }

            return collisions.ArrayToList();
        }

        /// <summary>
        /// A function to specifically change the array of collisions in the "SearchCollision" function.
        /// it keeps only the collision that accrued (i.e those with 'has_collisions' set to true)
        /// </summary>
        /// <param name="collisions">The array to turn into list</param>
        /// <returns>A list from the array with items that have has_collisions set to true</returns>
        private static List<Collisions> ArrayToList(this Collisions[] collisions)
        {
            List<Collisions> list = new List<Collisions>();
            foreach (Collisions collision in collisions)
                if (collision.has_collisions)
                    list.Add(collision);
            return list;
        }

        /// <summary>
        /// Search for invalid values in the string that represents a sudoku
        /// </summary>
        /// <param name="sudokuString"></param>
        /// <returns>A hashset with invalid values, that are not allowed in the sudoku</returns>
        public static HashSet<char> SearchInvalidValues(String sudokuString)
        {
            char min_value = (char)(ValueConvertor.ConvertToChar(Sudoku.NONE));
            char max_value = (char)(ValueConvertor.ConvertToChar((int)Math.Sqrt(sudokuString.Length)));

            HashSet<char> result = new HashSet<char>();

            foreach (char value in sudokuString)
                if (value < min_value || value > max_value)
                    result.Add(value);

            return result;
        }

        
    }
}
