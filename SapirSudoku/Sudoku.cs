using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using CustomExceptions;
using SapirBitSet;
using SapirMath;

namespace SapirSudoku
{
    public class Sudoku
    {
        static protected int NONE = 0;
        static protected Dictionary<int, int> allowables = new Dictionary<int, int>{ {NONE, 0} };

        protected int[,] sudoku;
        public int[,] SudokuGrid { get { return CloneGrid(); } }
        protected int grid_width;
        protected int grid_height;

        public Sudoku(Sudoku sudoku)
        {
            this.sudoku = sudoku.CloneGrid();
            this.grid_height = sudoku.grid_height;
            this.grid_width = sudoku.grid_width;
        }
        public Sudoku(int length = 9)
        {
            if (length < 2)
                throw new InvalidSudokuException("Minimum Sudoku Length is 2");

            for (int i = 1; i <= length; i++)
                allowables.Add(i,i);

            try {
                (int smaller, int bigger) = MathUtils.ColsestDivisibles(length);

                grid_height = smaller; // usually
                grid_width = bigger;   // usually
            }
            catch (PrimeNumberException)
            {
                throw new InvalidSudokuException("Cannot create a sudoku with a prime number as length");
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidSudokuException("Cannot create a sudoku with a negative length");
            }


            sudoku = new int[length, length];
        }

        public Sudoku(int[,] sudoku) : this(sudoku.GetLength(0))
        {
            int length = sudoku.GetLength(0);
            if (sudoku.GetLength(1) != length)
                throw new InvalidSudokuException("Sudoku size must be N*N");


            for (int row = 0; row < sudoku.GetLength(0); row++)
                for (int col = 0; col < sudoku.GetLength(1); col++)
                    Insert(sudoku[row,col], row, col);
            
            if (!IsValid())
                throw new InvalidSudokuException($"Invalid Sudoku! Collisions Accrued!");
        }
        
        public void Insert(int value, int row, int col)
        {
            if (value == NONE) return;
            if (allowables.ContainsKey(value) && CanInsert(value, row, col))
                sudoku[row,col] = value;
            else
                throw new InvalidValueException($"Cannot Insert '{value}' to sudoku");
        }
        
        public bool IsValid()
        {
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
                    if (cur == NONE)
                        continue;

                    if (rows[row].Contains(cur) || cols[col].Contains(cur))
                        return false;
                    else
                    {
                        rows[row].Add(cur);
                        cols[col].Add(cur);

                    }

                    int gridPos =  (row / grid_height) * (sudoku.GetLength(0) / grid_width) + col / grid_width;
                    if (grids[gridPos].Contains(cur))
                        return false;
                    else grids[gridPos].Add(cur);
                }
            }
            return true;
        }
        
        public bool CanInsert(int value, int row, int col)
        {
            if (value == NONE || sudoku[row,col] != NONE) return false;

            for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
                if (sudoku[rowPos, col] == value)
                    return false;

            for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
                if (sudoku[row, colPos] == value)
                    return false;

            int[] x = new int[] { -1,-1,-1,1,1,1,0,0,0};
            int[] y = new int[] { -1,1,0,-1,1,0,-1,1,0};

            for (int index = 0; index < sudoku.GetLength(0); index++)
                if ((row + y[index] < sudoku.GetLength(0)) && (col + x[index] < sudoku.GetLength(0)) 
                    && (row + y[index] >= 0) && (col + x[index] >= 0))
                if (sudoku[row + y[index], col + x[index]] == value)
                    return false;

            return true;
        }
        
        public override String ToString()
        {
            String msg = "";
            for (int row = 0; row < sudoku.GetLength(0); row++)
            {
                if (row != 0 && row % grid_height == 0)
                    msg += "\n";
                for (int col = 0; col < sudoku.GetLength(1); col++)
                {
                        if (col != 0 && col % grid_width == 0)
                            msg += " ";
                        msg += sudoku[row, col];
                }
                msg += "\n";
            }

            return msg;
        }
        
        public int[,] CloneGrid()
        {
            int[,] newSudoku = new int[sudoku.GetLength(0),sudoku.GetLength(1)];
            Array.Copy(sudoku, newSudoku, sudoku.Length);
            return newSudoku;
        }
    }
}