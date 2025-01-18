using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.ExceptionServices;
using CustomExceptions;
using SapirBitSet;
using SapirSudoku;

namespace SapirSudoku
{
    public class SudokuSolver : Sudoku
    {
        private HashSet<(int row,int col)>[] singlePossibilitesCounter;

        private BitSet[] rowAvailability; // Amount possible in all row
        private int[][] rowAvailabilityCounter;
        private BitSet[] colAvailability; // Amount possible in all col
        private int[][] colAvailabilityCounter;
        private BitSet[] gridAvailability;// Amount possible in all grid
        private int[][] gridAvailabilityCounter;

        private BitSet full; // represents a full bitset

        private Stack<(int value, int row, int col)> NextGarunteedAction;
        private Stack<(int value, int row, int col)> PrevAction;
        
        private SudokuSolver(int length = 9) : base(length){
            this.full = new BitSet(length);
            for (int i = 1; i <= length; i++)
                full.Add(i);

            this.singlePossibilitesCounter = new HashSet<(int, int)>[length + 1];
            for (int i = 0; i <= length; i++)
                singlePossibilitesCounter[i] = new HashSet<(int, int)> (length * length);
            for (int row = 0; row < length; row++)
                for (int col = 0; col < length; col++)
                    singlePossibilitesCounter[length].Add((row,col));

            this.rowAvailability = new BitSet[length];
            for (int i = 0; i < length; i++)
                rowAvailability[i] = new BitSet(full);

            this.rowAvailabilityCounter = new int[length][];
            for (int i = 0; i < length; i++)
            {
                rowAvailabilityCounter[i] = new int[length];
                for (int j = 0; j < length; j++)
                    rowAvailabilityCounter[i][j] = length;
            }



            this.colAvailability = new BitSet[length];
            for (int i = 0; i < length; i++)
                colAvailability[i] = new BitSet(full);

            this.colAvailabilityCounter = new int[length][];
            for (int i = 0; i < length; i++)
            {
                colAvailabilityCounter[i] = new int[length];
                for (int j = 0; j < length; j++)
                    colAvailabilityCounter[i][j] = length;
            }

            this.gridAvailability = new BitSet[length];
            for (int i = 0; i < length; i++)
                gridAvailability[i] = new BitSet(full);

            this.gridAvailabilityCounter = new int[length][];
            for (int i = 0; i < length; i++)
            {
                gridAvailabilityCounter[i] = new int[length];
                for (int j = 0; j < length; j++)
                    gridAvailabilityCounter[i][j] = length;
            }

            this.NextGarunteedAction = new Stack<(int value, int row, int col)>(length * 3);
            this.PrevAction = new Stack<(int value, int row, int col)>(length * length);
        }
        public SudokuSolver(int[,] grid) : this(grid.GetLength(0))
        {
            int length = sudoku.GetLength(0);
            if (sudoku.GetLength(1) != length)
                throw new InvalidSudokuException($"Sudoku size must be N*N, instead was {sudoku.GetLength(0)}*{sudoku.GetLength(1)}");

            for (int row = 0; row < sudoku.GetLength(0); row++)
                for (int col = 0; col < sudoku.GetLength(1); col++)
                    if (sudoku[row, col] == NONE && grid[row,col] != NONE)
                        Insert(grid[row, col], row, col);




            Console.WriteLine("Row");

            foreach (var i in rowAvailabilityCounter)
            {
                for (int j = 0; j < length; j++)
                {
                    Console.Write(j+1 + ": " + i[j] + " \t ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Col");

            foreach (var i in colAvailabilityCounter)
            {
                for (int j = 0; j < length; j++)
                {
                    Console.Write(j + 1 + ": " + i[j] + " \t ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("GRID");
            foreach (var i in gridAvailabilityCounter)
            {
                for (int j = 0; j < length; j++)
                {
                    Console.Write(j + 1 + ": " + i[j] + " \t ");
                }
                Console.WriteLine();
            }

            Console.WriteLine();

            Console.WriteLine("0 : ");
            foreach ((int,int) i in singlePossibilitesCounter[0])
                    Console.Write(i.Item1 + " " + i.Item2 + "\t");
            Console.WriteLine();
            Console.WriteLine("1 : ");
            foreach ((int, int) i in singlePossibilitesCounter[1])
                Console.Write(i.Item1 + " " + i.Item2 + "\t");
            Console.WriteLine();
            Console.WriteLine("2 : ");
            foreach ((int, int) i in singlePossibilitesCounter[2])
                Console.Write(i.Item1 + " " + i.Item2 + "\t");
            Console.WriteLine();

            {

            }
            
            while (NextGarunteedAction.Count() != 0)
            {
                (int value, int row, int col) = NextGarunteedAction.Pop();
                if (sudoku[row,col] == NONE)
                    Insert(value, row, col);
            }
            
        }

        public override void Insert(int value, int row, int col)
        {
            if (!InRange(row,col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");

            UpdateSquareInsert(value, row, col);
            UpdateRowInsert(value, row, col);
            UpdateColInsert(value, row, col);
            UpdateGridInsert(value, row, col);
        }

        private void UpdateSquareInsert(int value, int row, int col)
        {
            BitSet possible = GetColumnPossibilities(row, col);
            if (!possible.Contains(value))
                throw new InvalidInsertionException($"Cannot Insert '{value}' to {row},{col} in sudoku");

            rowAvailabilityCounter[row][value - 1] = 0;
            colAvailabilityCounter[col][value - 1] = 0;
            gridAvailabilityCounter[GridPos(row, col)][value - 1] = 0;

            rowAvailability[row].Remove(value);
            colAvailability[col].Remove(value);
            gridAvailability[GridPos(row, col)].Remove(value);

            singlePossibilitesCounter[possible.Count()].Remove((row, col));
            singlePossibilitesCounter[0].Add((row, col));
            sudoku[row, col] = value;

            possible.Remove(value);
            foreach (int possibilite in possible.GetValues())
            {
                int posAva;
                posAva = --rowAvailabilityCounter[row][possibilite - 1];
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInRow(possibilite, row);
                posAva = --colAvailabilityCounter[col][possibilite - 1];
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInCol(possibilite, col);
                posAva = --gridAvailabilityCounter[GridPos(row, col)][possibilite - 1];
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInGrid(possibilite, GridPos(row, col));
            }
        }

        private void UpdateRowInsert(int value, int row, int col)
        {
            for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
            {
                if (sudoku[rowPos, col] != NONE) continue;
                BitSet p = BitSet.Intersection(rowAvailability[rowPos], gridAvailability[GridPos(rowPos, col)]);
                if (!p.Contains(value)) continue;

                int count = p.Count();
                if (count <= 1) throw new InvalidInsertionException();
                if (count == 2) Console.WriteLine($"INSERT CUZ COUNT {value} in row : {rowPos} count");
                
                singlePossibilitesCounter[count].Remove((rowPos, col));
                singlePossibilitesCounter[count - 1].Add((rowPos, col));

                int posAva = --rowAvailabilityCounter[rowPos][value - 1];
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInRow(value, rowPos);

                posAva = --gridAvailabilityCounter[GridPos(rowPos, col)][value - 1];
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInGrid(value, GridPos(rowPos, col));
            }
        }

        private void UpdateColInsert(int value, int row, int col)
        {

            for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
            {
                if (sudoku[row, colPos] != NONE) continue;
                BitSet p = BitSet.Intersection(colAvailability[colPos], gridAvailability[GridPos(row, colPos)]);
                if (!p.Contains(value)) continue;

                int count = p.Count();
                if (count <= 1) throw new InvalidInsertionException();
                if (count == 2) Console.WriteLine($"INSERT CUZ COUNT {value} in col : {colPos}");

                singlePossibilitesCounter[count].Remove((row, colPos));
                singlePossibilitesCounter[count - 1].Add((row, colPos));

                int posAva = --colAvailabilityCounter[colPos][value - 1];
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) GridPos(PotentialInsertInCol(value, colPos), colPos);

                posAva = --gridAvailabilityCounter[GridPos(row, colPos)][value - 1];
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInGrid(value, GridPos(row, colPos));
            }
        }

        private void UpdateGridInsert(int value, int row, int col)
        {

            int initRow = GridPos(row, col) / (sudoku.GetLength(1) / grid_width) * grid_height;
            int initCol = GridPos(row, col) * grid_width % sudoku.GetLength(1);
            for (int rowPos = 0; rowPos < grid_height; rowPos++)
            {
                for (int colPos = 0; colPos < grid_width; colPos++)
                {
                    if (initRow + rowPos == row && initCol + colPos == col) continue;
                    if (sudoku[initRow + rowPos, initCol + colPos] != NONE) continue;

                    BitSet p = new BitSet(full);
                    if (initRow + rowPos != row)
                        p = BitSet.Intersection(p, rowAvailability[initRow + rowPos]);
                    if (initCol + colPos != col)
                        p = BitSet.Intersection(p, colAvailability[initCol + colPos]);

                    if (!p.Contains(value)) continue;
                    int count = p.Count();
                    if (count <= 1) throw new InvalidInsertionException();
                    if (count == 2) Console.WriteLine($"INSERT CUZ COUNT {value} in grid : {GridPos(initRow + rowPos, initCol + colPos)}");

                    singlePossibilitesCounter[count].Remove((initRow + row, initCol + col));
                    singlePossibilitesCounter[count - 1].Add((initRow + rowPos, initCol + colPos));

                    if (initRow + rowPos != row)
                    {
                        int posAva = --rowAvailabilityCounter[initRow + rowPos][value - 1];
                        if (posAva == 0) throw new InvalidInsertionException();
                        if (posAva == 1) PotentialInsertInRow(value, initRow + rowPos);
                    }
                    if (initCol + colPos != col)
                    {
                        int posAva = --colAvailabilityCounter[initCol + colPos][value - 1];
                        if (posAva == 0) throw new InvalidInsertionException();
                        if (posAva == 1) PotentialInsertInCol(value, initCol + colPos);
                    }
                }

            }
        }

        private int PotentialInsertInRow(int value, int row)
        {
            Console.WriteLine($"NEW INSERT {value} in row {row}");
            int col;
            for (col = 0; col < sudoku.GetLength(0); col++)
            {
                if (GetColumnPossibilities(row, col).Contains(value))
                {
                    NextGarunteedAction.Push((value,row,col));
                    return col;
                }
            }
            return -1;
        }

        private int PotentialInsertInCol(int value, int col)
        {
            Console.WriteLine($"NEW INSERT {value} in col {col}");
            int row;
            for (row = 0; row < sudoku.GetLength(0); row++)
            {
                if (GetColumnPossibilities(row, col).Contains(value))
                {
                    NextGarunteedAction.Push((value, row, col));
                    return row;
                }
            }
            return -1;
        }

        private int PotentialInsertInGrid(int value, int grid)
        {
            Console.WriteLine($"NEW INSERT {value} in grid {grid}");

            int initialRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
            int initialCol = grid * grid_width % sudoku.GetLength(1);
            for (int rowPos = 0; rowPos < grid_height; rowPos++)
            {
                for (int colPos = 0; colPos < grid_width; colPos++)
                {
                    if (GetColumnPossibilities(initialRow + rowPos, initialCol + colPos).Contains(value))
                    {
                        NextGarunteedAction.Push((value, initialRow + rowPos, initialCol + colPos));
                        return GridPos(initialRow + rowPos, initialCol + colPos);
                    }
                }
            }
            return -1;
        }


        public override void Remove(int row, int col)
        {

        }
        private BitSet GetColumnPossibilities(int row, int col)
        {
            if (!InRange(row,col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");
            if (sudoku[row, col] != NONE) return new BitSet(0);
            if (!InRange(row, col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");
            return BitSet.Intersection(rowAvailability[row], colAvailability[col], gridAvailability[GridPos(row, col)]);
        }

        public override bool CanInsert(int value, int row, int col)
        {
            if (sudoku[row, col] == NONE || !InRange(row,col)) return true;
            return !BitSet.Union(rowAvailability[row], colAvailability[col], gridAvailability[GridPos(row, col)]).Contains(value);
        }

        private int RowColIndex(int row, int col)
        {
            return row * sudoku.GetLength(0) + col + 1;
        }



    }
}
