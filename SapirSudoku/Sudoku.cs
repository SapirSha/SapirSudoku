using System;
using System.Collections.Generic;
using CustomExceptions;
using SapirMath;

namespace SapirSudoku
{
    public class Sudoku
    {
        static protected int NONE = 0;
        static protected HashSet<int> allowables = new HashSet<int>{NONE};

        protected int[][] sudoku;
        public int[][] SudokuGrid { get { return CloneGrid(); } }
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
            for (int i = 1; i <= length; i++)
                allowables.Add(i);

            try { 
                (int smaller, int bigger) = MathUtils.ColsestDivisibles(length);
                if (smaller == 0)
                    throw new InvalidSudokuException("Cannot create a sudoku with no length");

                grid_height = smaller; // usually
                grid_width = bigger;   // usually
            }
            catch (PrimeNumberException pne)
            {
                throw new InvalidSudokuException("Cannot create a sudoku with a prime number as length");
            }
            catch (ArgumentOutOfRangeException aoore)
            {
                throw new InvalidSudokuException("Cannot create a sudoku with a negative length");
            }


            sudoku = new int[length][];
            for (int row = 0; row < length; row++)
                sudoku[row] = new int[length];
        }

        public Sudoku(int[][] sudoku) : this(sudoku.Length)
        {
            int length = sudoku.Length;
            foreach (int[]row in sudoku)
                if (row.Length != length)
                    throw new InvalidSudokuException("Sudoku size must be N*N");

            for (int row = 0; row < sudoku.Length; row++)
                for (int col = 0; col < sudoku[0].Length; col++)
                    Insert(sudoku[row][col], row, col);

            (bool isValid, int? posRow, int? posCol) = this.IsValid();
            if (!isValid)
                throw new InvalidSudokuException($"Invalid Sudoku! rules collisions accrued at row: {posRow + 1}, col: {posCol + 1}");
        }

        public void Insert(int value, int row, int col)
        {
            if (allowables.Contains(value))
                sudoku[row][col] = value;
            else
                throw new InvalidValueException($"Cannot Insert '{value}' to sudoku");
        }

        public (bool valid,int? row,int? col) IsValid()
        {
            HashSet<int>[] rowsSet = new HashSet<int>[sudoku.Length];
            HashSet<int>[] colsSet = new HashSet<int>[sudoku[0].Length]; ;
            HashSet<int>[] gridsSet = new HashSet<int>[(sudoku.Length / grid_height) * (sudoku[0].Length / grid_width)]; ;

            for (int row = 0; row < sudoku.Length; row++)
            {
                for (int col = 0; col < sudoku[row].Length; col++)
                {

                    int cur = sudoku[row][col];
                    if (cur == NONE) continue;

                    if (rowsSet[row] == null)
                        rowsSet[row] = new HashSet<int>(sudoku.Length * 2);
                    if (colsSet[col] == null)
                        colsSet[col] = new HashSet<int>(sudoku[0].Length * 2);

                    int gridPos = (col / grid_width) + ((row / grid_height) * (sudoku[0].Length / grid_width));
                    if (gridsSet[gridPos] == null)
                        gridsSet[gridPos] = new HashSet<int>(sudoku.Length * 2);

                    if (rowsSet[row].Contains(cur) || colsSet[col].Contains(cur) || gridsSet[gridPos].Contains(cur))
                        return (false, row,col);
                    else
                    {
                        rowsSet[row].Add(cur);
                        colsSet[col].Add(cur);
                        gridsSet[gridPos].Add(cur);
                    }
                }
            }
            return (true,null,null);
        }

        public void PrintLine()
        {
            String msg = "";
            for (int row = 0; row < sudoku.Length; row++)
            {
                if (row % grid_height == 0 && row != 0)
                    msg += "\n";
                for (int col = 0; col < sudoku[row].Length; col++)
                {
                    if (col % grid_width == 0 && col != 0)
                        msg += " ";
                    msg += sudoku[row][col];
                }
                msg += "\n";
            }
            Console.WriteLine(msg);
        }

        public int[][] CloneGrid()
        {
            
            int[][] grid = (int[][])this.sudoku.Clone();
            for (int i = 0; i < sudoku.Length; i++)
                grid[i] = (int[])sudoku[i].Clone();
            return grid;
        }
    }
}