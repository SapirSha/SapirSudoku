using SapirSudoku.src;
using SapirSudoku.src.DataStructures;
using SapirSudoku.src.Exceptions;
using SapirSudoku.src.IO;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace MAIN
{
    public static class Program
    {
        static int MAX = 1;
        static int i = 0;
        public static void Print(this int[,] sudoku)
        {
            String msg = "";
            for (int i =0; i < sudoku.GetLength(0); i++)
            {
                if (i != 0 && i % 3 == 0)
                    msg += "\n";
                for( int j = 0; j < sudoku.GetLength(1); j++)
                {
                    if (j!= 0 && j%3 ==0)
                        msg += " ";
                    msg += sudoku[i, j];
                }
                msg += "\n";

            }
            Console.WriteLine(msg);
        }


        public static bool CanInsert(this int[,] sudoku, int value, int row, int col)
        {
            int length = sudoku.GetLength(0);
            int sqrlength = (int)Math.Sqrt(length);

            for (int i = 0; i < length; i++)
                if (sudoku[row,i] == value) return false;

            for (int i = 0; i < length; i++)
                if (sudoku[i,col] == value) return false;

            int square_start_row = (row / sqrlength) * sqrlength;
            int square_start_col = (col / sqrlength) * sqrlength;

            for (int i = 0; i < sqrlength; i++)
                for (int j = 0; j < sqrlength; j++)
                    if (sudoku[square_start_row + i, square_start_col + j] == value)
                        return false;

            return true;
        }

        public static bool Solve(this int[,] sudoku)
        {
            int Length = sudoku.GetLength(0);
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (sudoku[i,j] == 0)
                    {
                        for (int v = 1; v <= Length; v++)
                        {
                            if (sudoku.CanInsert(v, i, j))
                            {
                                sudoku[i,j] = v;
                                sudoku.Solve();
                                sudoku[i,j] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
            i++;
            Console.WriteLine("\n" + i + ":");
            sudoku.Print();
            if (i >= MAX)
                throw new Exception();
            return true;
        }

        public static int[,] StringToGrid(String str, int length)
        {
            int[,] grid = new int[length, length];
            int index = 0;
            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                {
                    if (str[index] == '.')
                        grid[i, j] = 0;
                    else
                        grid[i, j] = str[index] - '0';
                    index++;
                }
            return grid;
        }

        public static void SudokuPrintAsA2DArray(this Sudoku s)
        {
            int[,] board = s.Board;
            int gridWidth = s.GridWidth;
            int gridHieght = s.GridHeight;
            for (int row = 0; row < board.GetLength(0); row++)
            {
                if (row % gridHieght == 0 && row != 0)
                    Console.Write("\n");

                Console.Write("{");
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    if (col % gridWidth == 0 && col != 0)
                        Console.Write(" ");
                    Console.Write($"{board[row, col], -2}");
                    Console.Write(",");

                }
                Console.WriteLine("},");

            }
        }

        public static void Main(string[] args)
        {
            //var watch = Stopwatch.StartNew();
            /*
            int[][] sudoku = new int[][]{
                new int[]{5,3,0,0,7,0,0,0,0 },
                new int[]{6,0,0,1,9,5,0,0,0 },
                new int[]{0,9,8,0,0,0,0,6,0 },
                new int[]{8,0,0,0,6,0,0,0,3 },
                new int[]{4,0,0,8,0,3,0,0,1 },
                new int[]{7,0,0,0,2,0,0,0,6 },
                new int[]{0,6,0,0,0,0,2,8,0 },
                new int[]{0,0,0,4,1,9,0,0,5 },
                new int[]{0,0,0,0,8,0,0,7,9 },
            };
            */

            int[,] grid ={


            };
            var watch = Stopwatch.StartNew();

            UI.StartUI();

            watch.Stop();
            Console.WriteLine($"The Execution time of the program is: {watch.ElapsedMilliseconds}ms");

        }
    }
}