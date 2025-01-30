using SapirSudoku.src;
using SapirSudoku.src.DataStructures;
using System.Diagnostics;
using System.Globalization;

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
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 }, 
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                //*/

                /*
                {0,0,0,3,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,4,0,0,0 }, //
                {0,0,0,0,0,2,0,0,0 }, //
                {0,0,0,0,0,1,0,0,0 }, //
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                //*/

                ///*
                {8,0,0,0,0,0,0,0,0 },
                {0,0,3,6,0,0,0,0,0 },
                {0,7,0,0,9,0,2,0,0 },
                {0,5,0,0,0,7,0,0,0 },
                {0,0,0,0,4,5,7,0,0 },
                {0,0,0,1,0,0,0,3,0 },
                {0,0,1,0,0,0,0,6,8 },
                {0,0,8,5,0,0,0,1,0 },
                {0,9,0,0,0,0,4,0,0 },
                //*/

                /*
                {0,0,0,0,6,0,0,0,0 },
                {0,0,0,0,4,2,7,3,6 },
                {0,0,6,7,3,0,0,4,0 },
                {0,9,4,0,0,0,0,6,8 },
                {0,0,0,0,9,6,4,0,7 },
                {6,0,7,0,5,0,9,2,3 },
                {1,0,0,0,0,0,0,8,5 },
                {0,6,0,0,8,0,2,7,1 },
                {0,0,5,0,1,0,0,9,4 }
                //*/
                /*
                {0,0,0,0,0,0},
                {0,0,0,5,0,6},
                {2,0,0,0,5,0},
                {0,5,0,0,0,2},
                {6,0,3,0,0,0},
                {0,0,0,6,1,3},
                //*/
                /*
                {5,3,0, 0,7,0, 0,0,0 },
                {6,0,0, 1,9,5, 0,0,0 },
                {0,9,8, 0,0,0, 0,6,0 },
                
                {8,0,0, 0,6,0, 0,0,3 },
                {4,0,0, 8,0,3, 0,0,1 },
                {7,0,0, 0,2,0, 0,0,6 },

                {0,6,0, 0,0,0, 2,8,0 },
                {0,0,0, 4,1,9, 0,0,5 },
                {0,0,0, 0,8,0, 0,7,9 }
                //*/
                /* ww
                {0,0,0, 0,0,5, 0,8,0 },
                {0,0,0, 6,0,1, 0,4,3 },
                {0,0,0, 0,0,0, 0,0,0 },

                {0,1,0, 5,0,0, 0,0,0 },
                {0,0,0, 1,0,6, 0,0,0 },
                {0,0,0, 0,0,0, 0,0,5 },

                {5,3,0, 0,0,0, 0,6,1 },
                {0,0,0, 0,0,0, 0,0,0 },
                {0,0,0, 0,0,0, 0,0,0 }
                //*/
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
                //*/
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
                //*/
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
{8, 0, 0, 0, 0, 0, 0, 0, 0},
{0, 0, 3, 6, 0, 0, 0, 0, 0},
{0, 7, 0, 0, 9, 0, 2, 0, 0},
{0, 5, 0, 0, 0, 7, 0, 0, 0},
{0, 0, 0, 0, 4, 5, 7, 0, 0},
{0, 0, 0, 1, 0, 0, 0, 3, 0},
{0, 0, 1, 0, 0, 0, 0, 6, 8},
{0, 0, 8, 5, 0, 0, 0, 1, 0},
{0, 9, 0, 0, 0, 0, 4, 0, 0}
                //*/
                /*
    {5,2,8,6,0,0,0,4,9 },
    {1,3,6,4,9,0,0,2,5 },
    {7,9,4,2,0,5,6,3,0 },
    {0,0,0,1,0,0,2,0,0 },
    {0,0,7,8,2,6,3,0,0 },
    {0,0,2,5,0,9,0,6,0 },
    {2,4,0,3,0,0,9,7,6 },
    {8,0,9,7,0,2,4,1,3 },
    {0,7,0,9,0,4,5,8,2 }
                //*/
                /*
                {3,7,0,4,0,8,1,0,0 },
                {0,0,0,9,0,3,7,0,4 },
                {9,4,0,1,0,0,0,8,3 },
                {4,2,0,0,0,0,0,0,5 },
                {0,0,0,5,0,4,0,0,0 },
                {8,0,0,0,0,0,0,4,6 },
                {0,1,0,0,4,9,0,0,0 },
                {5,0,9,6,0,0,4,0,0 },
                {0,0,4,2,0,0,9,3,1 }
                //*/
                /*
                {9,0,3,4,0,0,6,7,0 },
                {7,1,2,6,5,8,9,4,3 },
                {0,6,4,7,3,9,0,0,2 },
                {0,3,0,0,4,7,5,0,9 },
                {0,0,0,0,9,0,3,0,0 },
                {0,0,9,3,0,0,0,8,0 },
                {4,0,0,0,6,3,0,9,0 },
                {0,9,0,2,7,0,4,3,6 },
                {3,0,6,9,8,4,7,0,1 }
                //*/
                /*
                {0,0,8,0,0,7,0,0,0 },
                {0,4,2,0,0,5,0,0,0 },
                {0,0,0,0,0,0,0,0,0 },
                {0,0,3,0,0,6,8,0,1 },
                {0,0,0,0,0,0,0,0,6 },
                {9,0,0,0,0,0,0,0,0 },
                {0,8,0,1,3,0,4,7,0 },
                {0,0,0,0,9,0,0,0,0 },
                {0,1,0,0,0,0,0,0,0 }
                //*/
                /*
                {0,3,0,0,0,0,0,1,0 },
                {0,0,8,0,9,0,0,0,0 },
                {4,0,0,6,0,8,0,0,0 },
                {0,0,0,5,7,6,9,4,0 },
                {0,0,0,9,8,3,5,2,0 },
                {0,0,0,1,2,4,0,0,0 },
                {2,7,6,0,0,5,1,9,0 },
                {0,0,0,7,0,9,0,0,0 },
                {0,9,5,0,0,0,4,7,0 }
                //*/
                /*
                {0,3,0,0,0,0,0,1,0 },
                {0,0,8,0,9,0,0,0,0 },
                {4,0,0,6,0,8,0,0,0 },
                {0,0,0,0,7,6,9,4,0 },
                {0,0,0,0,0,0,5,2,0 },
                {0,0,0,1,2,4,0,0,0 },
                {2,0,6,0,0,0,1,9,0 },
                {0,0,0,7,0,0,0,0,0 },
                {0,9,5,0,0,0,4,7,0 }
                //*/
                /*
                 {0,0,2,0,8,5,0,0,4 },
                 {0,0,0,0,3,0,0,6,0 },
                 {0,0,4,2,1,0,0,3,0 },
                 {0,0,0,0,0,0,0,5,2 },
                 {0,0,0,0,0,0,3,1,0 },
                 {9,0,0,0,0,0,0,0,0 },
                 {8,0,0,0,0,6,0,0,0 },
                 {2,5,0,4,0,0,0,0,8 },
                 {0,0,0,0,0,1,6,0,0 }
                //*/
                /*
                {0,0,3,8,0,0,5,1,0 },
                {0,0,8,7,0,0,9,3,0 },
                {1,0,0,3,0,5,7,2,8 },
                {0,0,0,2,0,0,8,4,9 },
                {8,0,1,9,0,6,2,5,7 },
                {0,0,0,5,0,0,1,6,3 },
                {9,6,4,1,2,7,3,8,5 },
                {3,8,2,6,5,9,4,7,1 },
                {0,1,0,4,0,0,6,9,2 }
                //*/
                /*
            {9,0,8,7,3,5,1,0,0 },
            {0,1,0,9,8,0,0,3,0 },
            {0,0,0,0,2,0,0,9,8 },
            {8,0,5,4,6,9,3,1,0 },
            {0,9,0,0,7,0,0,0,0 },
            {0,4,3,2,5,0,9,0,0 },
            {2,5,0,0,9,0,0,0,1 },
            {0,8,9,5,1,2,0,6,3 },
            {0,0,1,8,4,7,0,0,9 }
                //*/
                /*
                {4,0,0,0,0,0,9,3,8 },
                {0,3,2,0,9,4,1,0,0 },
                {0,9,5,3,0,0,2,4,0 },
                {3,7,0,6,0,9,0,0,4 },
                {5,2,9,0,0,1,6,7,3 },
                {6,0,4,7,0,3,0,9,0 },
                {9,5,7,0,0,8,3,0,0 },
                {0,0,3,9,0,0,4,0,0 },
                {2,4,0,0,3,0,7,0,9 }
                */
                
            };
            var watch = Stopwatch.StartNew();
            //Solve(grid);
            /*

812 753 649
943 682 175
675 491 283

154 237 896
369 845 721
287 169 534

521 974 368
438 526 917
796 318 452

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
            
            ///*
            int[,] g = StringToGrid("0000:=000000000?70050;01:00@90<8900800700004600=60:=080000070002=00030890>?500012;01@:000008007>00001000@0000900000<>?0740000000006@900000>0100002;0600=800<00500070002000000000<0900>?5;4020=0@0020=0@0<0907000>?500400=6000<803<0000002001@:0000=68000?0004020", 16);
            
            Sudoku s = new Sudoku(g, true);

            Console.WriteLine(s);

            int MAX = 10;
            int c = 0;


            foreach (Sudoku answer in s.Answers)
            {
                if (++c <= MAX)
                    Console.WriteLine(
                        "\n - - - - - - - - \n" +
                        answer +
                        "\n - - - - - - - - \n");
                else break;
            }
            

            watch.Stop();
            Console.WriteLine($"The Execution time of the program is: {watch.ElapsedMilliseconds}ms");
            
            //*/

            ISet<int> e = new BitSet(10);
            e.Add(5);
            e.Add(7);
            e.Add(8);
            e.Add(25);

            HashSet<int> set = new HashSet<int> { 5, 7, 8 };
            BitSet e2 = new BitSet(set);

            int[] a = new int[] { 1, 2, 3,4,5,6 };
            e.CopyTo(a, 1);
            foreach(int i in a)
                Console.WriteLine("WASD " + i);

            Console.WriteLine(e.IsSupersetOf(e2));
            Console.WriteLine(e.IsSupersetOf(set));


            Console.WriteLine(e);



        }
    }
}