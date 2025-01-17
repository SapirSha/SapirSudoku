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
        private BitSet[] singlePossibilitesCounter;

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


            this.singlePossibilitesCounter = new BitSet[length + 1];
            for (int i = 0; i <= length; i++)
                singlePossibilitesCounter[i] = new BitSet(length * length);
            for (int i = 1; i <= length * length; i++)
                singlePossibilitesCounter[length].Add(i);

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



            //SolveTheRest();
        }

        public override void Insert(int value, int row, int col)
        {
            if (!InRange(row,col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");

            BitSet possible = GetColumnPossibilities(row,col);
            if (!possible.Contains(value)) 
                throw new InvalidInsertionException($"Cannot Insert '{value}' to {row},{col} in sudoku");

            singlePossibilitesCounter[possible.Count()].Remove(value);
            singlePossibilitesCounter[0].Add(value);


            foreach (int possibilite in possible.GetValues())
            {
                if (possibilite == value) continue;
                rowAvailabilityCounter[row][possibilite - 1]--;
                colAvailabilityCounter[col][possibilite - 1]--;
                gridAvailabilityCounter[GridPos(row, col)][possibilite - 1]--;
            }
            Console.WriteLine(GridPos(row,col));
            rowAvailabilityCounter[row][value - 1]= 0;
            colAvailabilityCounter[col][value - 1]= 0;
            gridAvailabilityCounter[GridPos(row,col)][value - 1] = 0;


            for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
            {
                if (GridPos(rowPos, col) == GridPos(row, col)) continue;
                BitSet p = GetColumnPossibilities(rowPos, col);
                if (!p.Contains(value)) continue;
                int count = p.Count();
                if (count <= 1) throw new InvalidInsertionException();
                singlePossibilitesCounter[count].Remove(RowColIndex(rowPos,col));
                singlePossibilitesCounter[count-1].Add(RowColIndex(rowPos, col));
                int posAva;
                posAva = rowAvailabilityCounter[rowPos][value-1]--;
                if (posAva == 0) throw new InvalidInsertionException();
                posAva = gridAvailabilityCounter[GridPos(rowPos, col)][value - 1]--;
                if (posAva == 0) throw new InvalidInsertionException();
            }

            for (int colPos = 0; colPos < sudoku.GetLength(0); colPos++)
            {
                if (GridPos(row, colPos) == GridPos(row, col)) continue;
                BitSet p = GetColumnPossibilities(row, colPos);
                if (!p.Contains(value)) continue;
                int count = p.Count();
                if (count <= 1) throw new InvalidInsertionException();
                singlePossibilitesCounter[count].Remove(RowColIndex(row, colPos));
                singlePossibilitesCounter[count - 1].Add(RowColIndex(row, colPos));
                int posAva;
                posAva = colAvailabilityCounter[colPos][value - 1]--;
                if (posAva == 0) throw new InvalidInsertionException();
                posAva = gridAvailabilityCounter[GridPos(row, colPos)][value - 1]--;
                if (posAva == 0) throw new InvalidInsertionException();
            }

            (int initialRow, int initalCol) startInGridPos;
            startInGridPos.initialRow = GridPos(row, col) / (sudoku.GetLength(1) / grid_width) * grid_height;
            startInGridPos.initalCol = GridPos(row, col) * grid_width % sudoku.GetLength(1);
            for (int rowPos = 0; rowPos < grid_height; rowPos++)
            {
                for (int colPos = 0; colPos < grid_width; colPos++)
                {
                    if (startInGridPos.initialRow + rowPos == row && startInGridPos.initalCol + colPos == col) continue;
                    if (!InRange(startInGridPos.initialRow + rowPos, startInGridPos.initalCol + colPos))
                        continue;
                    BitSet p = GetColumnPossibilities(startInGridPos.initialRow + rowPos, startInGridPos.initalCol + colPos);
                    if (!p.Contains(value)) continue;
                    int count = p.Count();
                    if (count <= 1) throw new InvalidInsertionException();
                    singlePossibilitesCounter[count].Remove(RowColIndex(startInGridPos.initialRow + rowPos, startInGridPos.initalCol + colPos));
                    singlePossibilitesCounter[count - 1].Add(RowColIndex(startInGridPos.initialRow + rowPos, startInGridPos.initalCol + colPos));
                    int posAva = 1; //

                    if (startInGridPos.initialRow + rowPos != row)
                    {
                        posAva = rowAvailabilityCounter[startInGridPos.initialRow + rowPos][value - 1]--;
                        if (posAva == 0) throw new InvalidInsertionException();
                    }
                    if (startInGridPos.initalCol + colPos != col)
                    {
                        posAva = colAvailabilityCounter[startInGridPos.initalCol + colPos][value - 1]--;
                        if (posAva == 0) throw new InvalidInsertionException();
                    }

                }

            }
            rowAvailability[row].Remove(value);
            colAvailability[col].Remove(value);
            gridAvailability[GridPos(row, col)].Remove(value);
            sudoku[row, col] = value;
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
