using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.ExceptionServices;
using CustomExceptions;
using SapirBitSet;

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

            //SolveTheRest();
        }

        public override void Insert(int value, int row, int col)
        {
            if (!InRange(row,col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");

            BitSet possible = GetColumnPossibilities(row,col);
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


            foreach (int possibilite in possible.GetValues())
            {
                if (possibilite == value) continue;
                int posAva;
                posAva = --rowAvailabilityCounter[row][possibilite - 1];
                if (posAva == -1)
                    Console.WriteLine($"HERE ROW {possibilite} : {row},{col}");
                //if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInRow(possibilite, row);
                //if (!GetColumnPossibilities(row, col).Contains(possibilite)) continue;
                posAva = --colAvailabilityCounter[col][possibilite - 1];
                if (posAva == -1)
                    Console.WriteLine($"HERE COL {possibilite} : {row},{col}");
                //if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInCol(possibilite, col);
                //if (!GetColumnPossibilities(row, col).Contains(possibilite)) continue;
                posAva = --gridAvailabilityCounter[GridPos(row, col)][possibilite - 1];
                if (posAva == -1)
                    Console.WriteLine($"HERE GRID {possibilite} : {row},{col}");
                //if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInGrid(possibilite, GridPos(row, col));
            }


            for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
            {
                if (GridPos(rowPos, col) == GridPos(row, col)) continue;
                if (sudoku[rowPos, col] != NONE) continue;
                BitSet p = BitSet.Intersection(rowAvailability[rowPos], gridAvailability[GridPos(rowPos, col)]);
                if (!p.Contains(value)) continue;
                int count = p.Count();
                if (count <= 1) throw new InvalidInsertionException();
                if (count == 2) Console.WriteLine($"INSERT CUZ COUNT {value} in row : {rowPos} count");
                singlePossibilitesCounter[count].Remove((rowPos, col));
                singlePossibilitesCounter[count - 1].Add((rowPos, col));
                int posAva;
                posAva = --rowAvailabilityCounter[rowPos][value-1];
                //if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInRow(value, rowPos);
                if (!BitSet.Intersection(rowAvailability[rowPos], gridAvailability[GridPos(rowPos, col)]).Contains(value))     continue;

                posAva = --gridAvailabilityCounter[GridPos(rowPos, col)][value - 1];
                //if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInGrid(value, GridPos(rowPos, col));
            }

            for (int colPos = 0; colPos < sudoku.GetLength(0); colPos++)
            {
                if (GridPos(row, colPos) == GridPos(row, col)) continue;
                if (sudoku[row,colPos] != NONE) continue;
                BitSet p = BitSet.Intersection(colAvailability[colPos], gridAvailability[GridPos(row, colPos)]);
                if (!p.Contains(value)) continue;
                int count = p.Count();
                if (count <= 1) throw new InvalidInsertionException();
                if (count == 2) Console.WriteLine($"INSERT CUZ COUNT {value} in col : {colPos}");
                singlePossibilitesCounter[count].Remove((row, colPos));
                singlePossibilitesCounter[count - 1].Add((row, colPos));
                int posAva;
                posAva = --colAvailabilityCounter[colPos][value - 1];
                //if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInCol(value, colPos);
                if (!BitSet.Intersection(colAvailability[colPos], gridAvailability[GridPos(row, colPos)]).Contains(value))     continue;

                posAva = --gridAvailabilityCounter[GridPos(row, colPos)][value - 1];
                //if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInGrid(value, GridPos(row, colPos));
            }

            (int initialRow, int initialCol) startInGridPos;
            startInGridPos.initialRow = GridPos(row, col) / (sudoku.GetLength(1) / grid_width) * grid_height;
            startInGridPos.initialCol = GridPos(row, col) * grid_width % sudoku.GetLength(1);
            for (int rowPos = 0; rowPos < grid_height; rowPos++)
            {
                for (int colPos = 0; colPos < grid_width; colPos++)
                {
                    if (startInGridPos.initialRow + rowPos == row && startInGridPos.initialCol + colPos == col) continue;
                    if (!InRange(startInGridPos.initialRow + rowPos, startInGridPos.initialCol + colPos)) continue;
                    if (sudoku[startInGridPos.initialRow + rowPos, startInGridPos.initialCol + colPos] != NONE) continue;
                    BitSet p = new BitSet(full);
                    if (startInGridPos.initialRow + rowPos != row)
                        p = BitSet.Intersection(p, rowAvailability[startInGridPos.initialRow + rowPos]);
                    if (startInGridPos.initialCol + colPos != col)
                        p = BitSet.Intersection(p, colAvailability[startInGridPos.initialCol + colPos]);

                    if (!p.Contains(value)) continue;
                    int count = p.Count();
                    if (count <= 1) throw new InvalidInsertionException();
                    if (count == 2) Console.WriteLine($"INSERT CUZ COUNT {value} in grid : {GridPos(startInGridPos.initialRow + rowPos, startInGridPos.initialCol + colPos)}");
                    singlePossibilitesCounter[count].Remove((startInGridPos.initialRow + rowPos, startInGridPos.initialCol + colPos));
                    singlePossibilitesCounter[count - 1].Add((startInGridPos.initialRow + rowPos, startInGridPos.initialCol + colPos));
                    if (startInGridPos.initialRow + rowPos != row)
                    {
                        int posAva = --rowAvailabilityCounter[startInGridPos.initialRow + rowPos][value - 1];
                        //if (posAva == 0) throw new InvalidInsertionException();
                        if (posAva == 1) PotentialInsertInRow(value, startInGridPos.initialRow + rowPos);
                        if (!BitSet.Intersection(colAvailability[startInGridPos.initialCol + colPos], rowAvailability[startInGridPos.initialRow + rowPos]).Contains(value))     continue;
                    }
                    if (startInGridPos.initialCol + colPos != col)
                    {
                        int posAva = --colAvailabilityCounter[startInGridPos.initialCol + colPos][value - 1];
                        //if (posAva == 0) throw new InvalidInsertionException();
                        if (posAva == 1) PotentialInsertInCol(value, startInGridPos.initialCol + colPos);
                    }

                }

            }
        }

        private void PotentialInsertInRow(int value, int row)
        {
            for (int col = 0; col < sudoku.GetLength(0); col++)
            {
                if (GetColumnPossibilities(row, col).Contains(value))
                {
                    Insert(value, row, col);
                    break;
                }
            }
        }

        private void PotentialInsertInCol(int value, int col)
        {
            for (int row = 0; row < sudoku.GetLength(0); row++)
            {
                if (GetColumnPossibilities(row, col).Contains(value))
                {
                    Insert(value, row, col);
                    break;
                }
            }
        }

        private void PotentialInsertInGrid(int value, int grid)
        {
            int initialRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
            int initialCol = grid * grid_width % sudoku.GetLength(1);

            for (int rowPos = 0; rowPos < grid_height; rowPos++)
            {
                for (int colPos = 0; colPos < grid_width; colPos++)
                {
                    if (GetColumnPossibilities(initialRow + rowPos, initialCol + colPos).Contains(value))
                        Insert(value, initialRow + rowPos, initialCol + colPos);
                }
            }

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
