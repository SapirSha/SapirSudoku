using System;
using System.Collections.Generic;
using SapirSudoku;
using SapirStruct;
using System.Diagnostics;
using System.Collections;
using System.Numerics;
using System.ComponentModel.DataAnnotations;

namespace MAIN
{
    public static class Program
    {
        static int MAX = 10;
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
                /*
                new int[]{0,0,0,0,0,0,0,0,0 },
                new int[]{0,0,0,0,0,0,0,0,0 },
                new int[]{0,0,0,0,0,0,0,0,0 },
                new int[]{0,0,0,0,0,0,0,0,0 }, 
                new int[]{0,0,0,0,0,0,0,0,0 },
                new int[]{0,0,0,0,0,0,0,0,0 },
                new int[]{0,0,0,0,0,0,0,0,0 },
                new int[]{0,0,0,0,0,0,0,0,0 },
                new int[]{0,0,0,0,0,0,0,0,0 },
                */

                /*
                {0,0,0,0,3,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                */

                /*
                {8,0,0,0,0,0,0,0,0 },
                {0,0,3,6,0,0,0,0,0 },
                {0,7,0,0,9,0,2,0,0 },
                {0,5,0,0,0,7,0,0,0 },
                {0,0,0,0,4,5,7,0,0 },
                {0,0,0,1,0,0,0,3,0 },
                {0,0,1,0,0,0,0,6,8 },
                {0,0,8,5,0,0,0,1,0 },
                {0,9,0,0,0,0,4,0,0 },
                */
                /*
                {0,0,0,1,0,0},
                {0,0,0,5,0,6},
                {2,0,0,0,5,0},
                {0,5,0,0,0,2},
                {6,0,3,0,0,0},
                {0,0,0,6,1,3},
                */
                
                {5,3,0, 0,7,0, 0,0,0 },
                {6,0,0, 1,9,5, 0,0,0 },
                {0,9,8, 0,0,0, 0,6,0 },
                
                {8,0,0, 0,6,0, 0,0,3 },
                {4,0,0, 8,0,3, 0,0,1 },
                {7,0,0, 0,2,0, 0,0,6 },

                {0,6,0, 0,0,0, 2,8,0 },
                {0,0,0, 4,1,9, 0,0,5 },
                {0,0,0, 0,8,0, 0,7,9 }
                
                /* UNBEATABLE
                {2,0,0,9,0,0,0,0,0 },
                {0,0,0,0,0,0,0,6,0 },
                {0,0,0,0,0,1,0,0,0 },
                {5,0,2,6,0,0,4,0,7 },
                {0,0,0,0,0,4,1,0,0 },
                {0,0,0,0,9,8,0,2,3 },
                {0,0,0,0,0,3,0,8,0 },
                {0,0,5,0,1,0,0,0,0 },
                {0,0,7,0,0,0,0,0,0 },
                */
                /*???
                {0, 0, 0, 0, 9, 8, 0, 0, 0},
                {0, 3, 0, 0, 0, 2, 0, 9, 0},
                {0, 0, 9, 0, 0, 0, 0, 0, 8},
                {0, 9, 0, 0, 0, 0, 0, 7, 0},
                {0, 0, 0, 3, 0, 0, 0, 0, 9},
                {0, 0, 0, 0, 0, 9, 8, 0, 0},
                {8, 0, 0, 0, 0, 0, 0, 3, 0},
                {0, 0, 0, 9, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 9, 0, 0},
                */
                /*
    new int[]{5, 0, 0, 0, 0, 0, 0, 0, 0},
    new int[]{0, 3, 0, 0, 0, 0, 0, 0, 0},
    new int[]{0, 0, 0, 0, 9, 0, 0, 0, 0},
    new int[]{0, 9, 8, 0, 0, 0, 0, 0, 0},
    new int[]{0, 0, 0, 0, 0, 0, 0, 0, 0},
    new int[]{0, 0, 0, 0, 0, 0, 0, 3, 0},
    new int[]{0, 0, 0, 0, 0, 0, 0, 0, 9},
    new int[]{0, 0, 0, 0, 0, 0, 0, 0, 0},
    new int[]{0, 0, 0, 0, 0, 0, 0, 0, 5}
                */
                /*
                new int[]     {8, 0, 0, 0, 0, 0, 0, 0, 0},
   new int[] {0, 0, 3, 6, 0, 0, 0, 0, 0},
   new int[] {0, 7, 0, 0, 9, 0, 2, 0, 0},
   new int[] {0, 5, 0, 0, 0, 7, 0, 0, 0},
  new int[]  {0, 0, 0, 0, 4, 5, 7, 0, 0},
  new int[]  {0, 0, 0, 1, 0, 0, 0, 3, 0},
  new int[]  {0, 0, 1, 0, 0, 0, 0, 6, 8},
  new int[]  {0, 0, 8, 5, 0, 0, 0, 1, 0},
  new int[]  {0, 9, 0, 0, 0, 0, 4, 0, 0}
                */
            };
            var watch = Stopwatch.StartNew();
            //Solve(grid);
            /*
            Sudoku sudoku = new Sudoku(grid);
            sudoku.PrintLine();
            SudokuSolver solver = new SudokuSolver(sudoku);
            int i = 0;
            IEnumerable<Sudoku> answers = solver.Solve();
            foreach (Sudoku s in answers)
            {
                if (i >= 1) break;
                Console.WriteLine($"\n{++i}:");
                s.PrintLine();
                if (!(s.IsValid().valid && IsSameBase(sudoku, s)))
                    throw new Exception("ALALAL");
            }
            if (i == 0)
                Console.WriteLine("Unbeatable!");
            /*
            try
            {
                //Solve(grid);
            }
            catch (Exception ex)
            {

            }
            */
            /*
            MAX = 1;
            Sudoku sudoku = new Sudoku(grid);
            SudokuSolver solver = new SudokuSolver(sudoku);
            solver.PrintLine();
            int ans = 1;
            foreach (Sudoku answer in solver.Solve())
            {
                Console.WriteLine("\n" + ans++ +": ");
                answer.PrintLine();
                if (ans >= MAX) break;
            }
            if (ans == 1)
                Console.WriteLine("UNBEATABLE");
            Console.WriteLine();
            watch.Stop();
            Console.WriteLine($"CLASS: {watch.ElapsedMilliseconds}ms");
            
            var watch2 = Stopwatch.StartNew();
            try { Solve(grid); }
            catch (Exception) { Console.WriteLine("STOP"); }
            watch2.Stop();
            Console.WriteLine($"FUNC: {watch2.ElapsedMilliseconds}ms");
            */
            /*
        }

        catch (Exception e)
        {
            Console.WriteLine("EXCEPTION --- ");
            Console.WriteLine(e.Message);
            Console.WriteLine(e.GetType());
            Console.WriteLine(e.StackTrace);
            Console.WriteLine("EXCEPTION --- ");
        }



        //sudoku.Print();


        /*
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        sudoku.Print();
        try
        { sudoku.Solve(); }
        catch (Exception ex)
        {
            Console.WriteLine("FINISH");
        }
        stopwatch.Stop();
        TimeSpan ts = stopwatch.Elapsed;

        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");

            new int[] { 5, 3, 0, 0, 7, 0, 0, 0, 0 },
                new int[] { 6, 0, 0, 1, 9, 5, 0, 0, 0 },
                new int[] { 0, 9, 8, 0, 0, 0, 0, 6, 0 },
                new int[] { 8, 0, 0, 0, 6, 0, 0, 0, 3 },
                new int[] { 4, 0, 0, 8, 0, 3, 0, 0, 1 },
                new int[] { 7, 0, 0, 0, 2, 0, 0, 0, 6 },
                new int[] { 0, 6, 0, 0, 0, 0, 2, 8, 0 },
                new int[] { 0, 0, 0, 4, 1, 9, 0, 0, 5 },
                new int[] { 0, 0, 0, 0, 8, 0, 0, 7, 9 },



            /*= ""
                + "030605000"
                + "600090002"
                + "070100006"
                + "090000000"
                + "810050069" 
                + "000000080" 
                + "400003020" 
                + "900020005" 
                + "000908030";*/

            //  Solve(grid) = 32ms
            // Solve grid 1000 solves in 7421
            // Sudoku.Solve in 4211
            // 469/491/483 .solve for empty 100
            //476/496/496 solve() for empty
            // no print empty solve() 230/222/230
            // no print empty .solve 235/237/231

            // Print the execution time in milliseconds 
            // by using the property elapsed milliseconds 

            /*
            Heap<int> hip = new Heap<int>(10, (x,y) => y > x);

            int[] arr = new int[] { 10, 9, 5, 4, 7, 1, 2, 3,8,8,8 };
            foreach (int i in arr)
                hip.Push(i);

            for (int i = 0; i < hip.values.Length; i++)
            {
                Console.WriteLine(hip.values[i]);
            }
            */
            //Sudoku s = new Sudoku(grid);
            //Console.WriteLine(s);
            SudokuSolver solver = new SudokuSolver(grid);

            Console.WriteLine(solver);
            Console.WriteLine();
            //try { Solve(grid); } catch (Exception e) {  }




            watch.Stop();
            Console.WriteLine($"The Execution time of the program is: {watch.ElapsedMilliseconds}ms");

        }
    }
}