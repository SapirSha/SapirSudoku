using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Transactions;
using CustomExceptions;

namespace SapirSudoku
{
    public class Sudoku
    {
        static protected HashSet<int> ALLOWABLES = new HashSet<int>();
        static protected int NONE = 0;
        static Sudoku()
        {
            ALLOWABLES.Add(0);
            ALLOWABLES.Add(1);
            ALLOWABLES.Add(2);
            ALLOWABLES.Add(3);
            ALLOWABLES.Add(4);
            ALLOWABLES.Add(5);
            ALLOWABLES.Add(6);
            ALLOWABLES.Add(7);
            ALLOWABLES.Add(8);
            ALLOWABLES.Add(9);
        }

        protected int[][] sudoku;
        protected int grid_width;
        protected int grid_height;


        public Sudoku(int length = 9)
        {

            try { 
                (int smaller, int bigger) = MathUtils.ColsestDivisibles(length);
                if (smaller == 0)
                    throw new InvalidSudokuException("Cannot create a sudoku with no length");

                grid_height = smaller;
                grid_width = bigger;
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
            if (sudoku.Length != sudoku[0].Length)
                throw new InvalidSudokuException("Cannot create a sudoku with different height and width");

            for (int row = 0; row < sudoku.Length; row++)
                for (int col = 0; col < sudoku[0].Length; col++)
                    Insert(sudoku[row][col], row, col);

            (bool isValid, int? posRow, int? posCol) = this.IsValid();
            if (!isValid)
                throw new InvalidSudokuException($"Invalid Sudoku! rules collisions accrued at row: {posRow + 1}, col: {posCol + 1}");
        }

        public void Insert(int value, int row, int col)
        {
            if (ALLOWABLES.Contains(value))
                sudoku[row][col] = value;
            else
                throw new InvalidValueException($"Cannot Insert '{value}' to sudoku");
        }

        public (bool,int?,int?) IsValid()
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

                    int gridPos = (col / grid_width) + ((row / grid_height) * 3);
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
            for (int row = 0; row < sudoku.Length; row++)
            {
                if (row % grid_height == 0 && row != 0)
                    Console.WriteLine();
                for (int col = 0; col < sudoku[row].Length; col++)
                {
                    if (col % grid_width == 0 && col != 0)
                        Console.Write(" ");
                    Console.Write(sudoku[row][col]);
                }
                Console.WriteLine();
            }
        }

        public int[][] CloneSudoku()
        {
            int[][] sudoku = new int[this.sudoku.Length][];
            for (int row = 0;row < sudoku.Length; row++)
            {
                sudoku[row] = new int[sudoku[row].Length];
                for (int col = 0; col < sudoku[row].Length; col++)
                {
                    sudoku[row][col] = this.sudoku[row][col];
                }
            }
            return sudoku;
        }
    }
}