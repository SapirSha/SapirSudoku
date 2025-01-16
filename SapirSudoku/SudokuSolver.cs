using System;
using System.Collections.Generic;
using CustomExceptions;
using SapirBitSet;

namespace SapirSudoku
{
    public class SudokuSolver : Sudoku
    {
        private BitSet[] possibilities; // Amount possible in a single column

        private BitSet[,] row_availability; // Amount possible in all row
        private BitSet[,] col_availability; // Amount possible in all col
        private BitSet[,] grid_availability;// Amount possible in all grid
        private BitSet full; // represents a full bitset


        private SudokuSolver(int length = 9) : base(length){
            this.full = new BitSet(length);
            for (int i = 1; i <= length; i++)
                full.Add(i);


                this.possibilities = new BitSet[length + 1];
            for (int i = 0; i <= length; i++)
                possibilities[i] = new BitSet(length);
            foreach (var value in allowables)
                if (value.Key != NONE)
                    possibilities[length].Add(value.Value);

            this.row_availability = new BitSet[length, length + 1];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j <= length; j++)
                    row_availability[i, j] = new BitSet(length);

                foreach (var value in allowables)
                    if (value.Key != NONE)
                        row_availability[i, length].Add(value.Value);
            }

            this.col_availability = new BitSet[length, length + 1];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j <= length; j++)
                    col_availability[i, j] = new BitSet(length);

                foreach (var value in allowables)
                    if (value.Key != NONE)
                        col_availability[i, length].Add(value.Value);
            }


            this.grid_availability = new BitSet[length, length + 1];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j <= length; j++)
                    grid_availability[i, j] = new BitSet(length);

                foreach (var value in allowables)
                    if (value.Key != NONE)
                        grid_availability[i, length].Add(value.Value);
            }
        }
        public SudokuSolver(int[,] grid) : this(grid.GetLength(0))
        {
            int length = sudoku.GetLength(0);
            if (sudoku.GetLength(1) != length)
                throw new InvalidSudokuException($"Sudoku size must be N*N, instead was {sudoku.GetLength(0)}*{sudoku.GetLength(1)}");

            for (int row = 0; row < sudoku.GetLength(0); row++)
                for (int col = 0; col < sudoku.GetLength(1); col++)
                    if (sudoku[row, col] != NONE)
                        Insert(sudoku[row, col], row, col);

            if (!IsValid())
                throw new InvalidSudokuException($"Invalid Sudoku! Collisions Accrued!");
        }

        private new void Insert(int value, int row, int col)
        {
            
        }

        private new void Remove(int row, int col)
        {

        }
        private int CountPossibilities(int row, int col)
        {
            if (sudoku[row, col] != NONE) return 0;
            if (!InRange(row, col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");
            return BitSet.Subtract(full, BitSet.Union(row_availability[row, 0], col_availability[col, 0], grid_availability[GridPos(row,col), 0])).Count();
        }

        private bool CanInsert(int value, int row, int col)
        {
            if (sudoku[row, col] == NONE) return true;
            return !BitSet.Union(row_availability[row, 0], col_availability[col, 0], grid_availability[GridPos(row, col), 0]).Contains(value);
        }



    }
}
