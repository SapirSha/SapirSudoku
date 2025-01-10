using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CustomExceptions;

namespace SapirSudoku
{
    public class Sudoku
    {
        static private HashSet<int> ALLOWABLES = new HashSet<int>();
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

        private int[][] sudoku;
        private int grid_width;
        private int grid_height;


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


        }

        public void Insert(int value, int row, int col)
        {
            if (ALLOWABLES.Contains(value))
                sudoku[row][col] = value;
            else
                throw new InvalidValueException($"Cannot Insert '{value}' to sudoku");

        }

        public void PrintLine()
        {
            for (int row = 0; row < sudoku.Length; row++)
            {
                if (row % grid_height == 0 && row != 0)
                    Console.WriteLine();
                for (int col = 0; col < sudoku[0].Length; col++)
                {
                    if (col % grid_width == 0 && col != 0)
                        Console.Write(" ");
                    Console.Write(sudoku[row][col]);
                }
                Console.WriteLine();
            }
            
        }


    }
}