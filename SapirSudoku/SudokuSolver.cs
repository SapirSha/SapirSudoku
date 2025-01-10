using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using CustomExceptions;
namespace SapirSudoku
{
    public class SudokuSolver : Sudoku
    {
        private HashSet<int>[][] avaliables;
        public SudokuSolver(int length = 9) : base(length)
        {
            avaliables = new HashSet<int>[sudoku.Length][];
            for (int row = 0; row < sudoku.Length; row++)
            {
                avaliables[row] = new HashSet<int>[sudoku[row].Length];
                for (int col = 0; col < sudoku[row].Length; col++)
                {
                    avaliables[row][col] = new HashSet<int>(ALLOWABLES.Count);
                    foreach (int i in ALLOWABLES)
                        if (i != NONE)
                            avaliables[row][col].Add(i);
                }
            }
        }
        public SudokuSolver(int[][] sudoku) : this(sudoku.Length) {
            if (sudoku.Length != sudoku[0].Length)
                throw new InvalidSudokuException("Cannot create a sudoku with different height and width");

            for (int row = 0; row < sudoku.Length; row++)
                for (int col = 0; col < sudoku[0].Length; col++)
                    if (sudoku[row][col] != NONE)
                        Insert(sudoku[row][col], row, col);
        }

        public SudokuSolver(Sudoku sudoku) : this(sudoku.CloneSudoku()) { }

        public new void Insert(int value, int row, int col)
        {
            if (!avaliables[row][col].Contains(value))
                throw new InvalidInsertionException($"Cannot insert {value} at {row},{col} due to collisions");

            base.Insert(value, row, col);

            for (int rowPos = 0; rowPos < sudoku.Length; rowPos++)
                avaliables[rowPos][col].Remove(value);

            for (int colPos = 0; colPos < sudoku.Length; colPos++)
                avaliables[row][colPos].Remove(value);

            int gridPos = (col / grid_width) + ((row / grid_height) * 3);
            int gridX = gridPos % (sudoku[0].Length / grid_width) * grid_width;
            int gridY = gridPos - (gridX / 3);

            for (int y = 0; y < grid_height; y++)
                for (int x = 0; x < grid_width; x++)
                    avaliables[gridY + y][gridX + x].Remove(value);

        }



    }
}
