using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;

public static class Program {

    public static void Print(this int[][] sudoku)
    {
        int count = 0;
        int count2 = 1;
        foreach (int[] row in sudoku)
        {
            foreach (int col in row)
            {
                if (count++ % 3 == 0)
                    Console.Write(" ");
                Console.Write(col);
            }
            if (count2++ % 3 == 0)
                Console.WriteLine();
            Console.WriteLine();
        }
    }


    public static bool CanInsert(this int[][] sudoku, int value, int row, int col)
    {
        int length = sudoku.Length;
        int sqrlength = (int)Math.Sqrt(length);

        for (int i = 0; i < length; i ++)
            if (sudoku[row][i] == value) return false;

        for (int i = 0; i < length; i++)
            if (sudoku[i][col] == value) return false;

        int square_start_row = (row / sqrlength) * sqrlength;
        int square_start_col = (col / sqrlength) * sqrlength;

        for (int i = 0; i < sqrlength; i++)
            for (int j = 0; j < sqrlength; j++)
                if (sudoku[square_start_row + i][square_start_col + j] == value)
                    return false;

        return true;
    }

    public static bool Solve(this int[][] sudoku)
    {
        int Length = sudoku.Length;
        for (int i = 0; i < Length; i++)
        {
            for (int j = 0; j <Length; j++)
            {
                if (sudoku[i][j] == 0)
                {
                    for (int v = 1; v <= Length; v++)
                    {
                        if (sudoku.CanInsert(v, i, j))
                        {
                            sudoku[i][j] = v;
                            sudoku.Solve();
                            sudoku[i][j] = 0;
                        }
                    }
                    return false;
                }
            }
        }
        sudoku.Print();
        throw new Exception();
        return true;
    }

    public static void Main(string[] args)
    {
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

        int[][] sudoku ={
            new int[]{8,0,0,0,0,0,0,0,0 },
            new int[]{0,0,3,6,0,0,0,0,0 },
            new int[]{0,7,0,0,9,0,2,0,0 },
            new int[]{0,5,0,0,0,7,0,0,0 },
            new int[]{0,0,0,0,4,5,7,0,0 },
            new int[]{0,0,0,1,0,0,0,3,0 },
            new int[]{0,0,1,0,0,0,0,6,8 },
            new int[]{0,0,8,5,0,0,0,1,0 },
            new int[]{0,9,0,0,0,0,4,0,0 },
        };
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
    }
}