using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SapirSudoku.src.DataStructures;

namespace SapirSudoku.src.IO
{
    public static class SudokuErrors
    {
        public class Collisions
        {
            public int value;
            public bool has_collisions;
            public HashSet<int> rowCollisions = new HashSet<int>();
            public HashSet<int> colCollisions = new HashSet<int>();
            public HashSet<int> gridCollisions = new HashSet<int>();
        }


        public static List<Collisions> SearchCollisions(Sudoku s)
        {
            int[,] sudoku = s.Board;
            Collisions[] collisions = new Collisions[sudoku.GetLength(0) + 1];
            for(int i = 0; i < collisions.Length; i++)
            {
                collisions[i] = new Collisions();
                collisions[i].has_collisions = false;
            }

            BitSet[] rows = new BitSet[sudoku.GetLength(0)];
            for (int i = 0; i < rows.Length; i++) rows[i] = new BitSet(sudoku.GetLength(0));

            BitSet[] cols = new BitSet[sudoku.GetLength(0)];
            for (int i = 0; i < cols.Length; i++) cols[i] = new BitSet(sudoku.GetLength(0));

            BitSet[] grids = new BitSet[sudoku.GetLength(0)];
            for (int i = 0; i < grids.Length; i++) grids[i] = new BitSet(sudoku.GetLength(0));


            for (int row = 0; row < sudoku.GetLength(0); row++)
            {
                for (int col = 0; col < sudoku.GetLength(1); col++)
                {
                    int cur = sudoku[row, col];

                    if (cur == Sudoku.NONE) continue;

                    int gridPosition = s.GridPositionOf(row, col);

                    if (rows[row].Contains(cur) || cols[col].Contains(cur) || grids[gridPosition].Contains(cur))
                    {
                        collisions[cur].value = cur;
                        collisions[cur].has_collisions = true;
                        if (rows[row].Contains(cur))
                        {
                            collisions[cur].rowCollisions.Add(row);
                        }
                        if (cols[col].Contains(cur))
                        {
                            collisions[cur].colCollisions.Add(col);
                        }
                        if (grids[gridPosition].Contains(cur))
                        {
                            Console.WriteLine("HERE");
                            collisions[cur].gridCollisions.Add(gridPosition);
                        }
                    }
                        rows[row].Add(cur);
                        cols[col].Add(cur);
                        grids[gridPosition].Add(cur);
                }
            }
            return collisions.ArrayToList();
        }

        private static List<Collisions> ArrayToList(this Collisions[] collisions)
        {
            List<Collisions> list = new List<Collisions>();
            foreach (Collisions collision in collisions)
                if (collision.has_collisions)
                    list.Add(collision);
            return list;
        }

        public static HashSet<char> SearchInvalidValues(String sudokuString)
        {
            char min_value = (char)(Sudoku.NONE + '0');
            char max_value = (char)(Math.Sqrt(sudokuString.Length) + '0');

            HashSet<char> result = new HashSet<char>();

            foreach (char value in sudokuString)
                if (value < min_value || value > max_value)
                    result.Add(value);

            return result;
        }

        
    }
}
