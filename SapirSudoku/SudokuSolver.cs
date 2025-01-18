using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.ExceptionServices;
using CustomExceptions;
using SapirStruct;
using SapirSudoku;

namespace SapirSudoku
{
    public class SudokuSolver : Sudoku
    {
        private HashSet<(int row,int col)>[] singlePossibilitesCounter;

        private BitSet[] rowAvailability; // Amount possible in all row
        private BitSet[][] rowAvailabilityCounter;
        private BitSet[] colAvailability; // Amount possible in all col
        private BitSet[][] colAvailabilityCounter;
        private BitSet[] gridAvailability;// Amount possible in all grid
        private BitSet[][] gridAvailabilityCounter;

        private BitSet full; // represents a full bitset in range 1 to N

        private Stack<(int value, int row, int col)> NextGarunteedAction;
        private Stack<(int value, int row, int col)> PrevAction;

        int size;
        int count;

        private SudokuSolver(Sudoku s) : this(s.CloneGrid()) { }
        
        private SudokuSolver(int length = 9) : base(length){
            this.full = new BitSet(length);
            for (int i = 1; i <= length; i++)
                full.Add(i);

            size = sudoku.Length;
            count = 0;

            this.singlePossibilitesCounter = new HashSet<(int, int)>[length + 1];
            for (int i = 0; i <= length; i++)
                singlePossibilitesCounter[i] = new HashSet<(int, int)> (length * length);
            for (int row = 0; row < length; row++)
                for (int col = 0; col < length; col++)
                    singlePossibilitesCounter[length].Add((row,col));

            this.rowAvailability = new BitSet[length];
            for (int i = 0; i < length; i++)
                rowAvailability[i] = new BitSet(full);

            this.rowAvailabilityCounter = new BitSet[length][];
            for (int i = 0; i < length; i++)
            {
                rowAvailabilityCounter[i] = new BitSet[length];
                for (int j = 0; j < length; j++)
                    rowAvailabilityCounter[i][j] = new BitSet(full);
            }

            this.colAvailability = new BitSet[length];
            for (int i = 0; i < length; i++)
                colAvailability[i] = new BitSet(full);

            this.colAvailabilityCounter = new BitSet[length][];
            for (int i = 0; i < length; i++)
            {
                colAvailabilityCounter[i] = new BitSet[length];
                for (int j = 0; j < length; j++)
                    colAvailabilityCounter[i][j] = new BitSet(full);
            }

            this.gridAvailability = new BitSet[length];
            for (int i = 0; i < length; i++)
                gridAvailability[i] = new BitSet(full);

            this.gridAvailabilityCounter = new BitSet[length][];
            for (int i = 0; i < length; i++)
            {
                gridAvailabilityCounter[i] = new BitSet[length];
                for (int j = 0; j < length; j++)
                    gridAvailabilityCounter[i][j] = new BitSet(full);
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
                    Console.Write(j  +1 + ": " + i[j].Count() + " \t ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Col");

            foreach (var i in colAvailabilityCounter)
            {
                for (int j = 0; j < length; j++)
                {
                    Console.Write(j + 1 + ": " + i[j].Count() + " \t ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("GRID");
            foreach (var i in gridAvailabilityCounter)
            {
                for (int j = 0; j < length; j++)
                {
                    Console.Write(j + 1 + ": " + i[j].Count() + " \t ");
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


            Console.WriteLine("RIGHT HERE");
            Console.WriteLine(GetSquarePossibilities(4,4));
            Console.WriteLine(singlePossibilitesCounter[1].Count());
            Console.WriteLine(singlePossibilitesCounter[2].Count());
            Console.WriteLine(singlePossibilitesCounter[3].Count());
            Console.WriteLine("Left: " + NextGarunteedAction.Count());

            
            while (NextGarunteedAction.Count() != 0)
            {
                (int value, int row, int col) = NextGarunteedAction.Pop();
                if (GetSquarePossibilities(row,col).Contains(value))
                    Insert(value, row, col);
            }
            if (IsSolved()) Console.WriteLine("ONE SOLUTION -------------------------------");

            Console.WriteLine("Row");

            foreach (var i in rowAvailabilityCounter)
            {
                for (int j = 0; j < length; j++)
                {
                    Console.Write(j + 1 + ": " + i[j].Count() + " \t ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Col");

            foreach (var i in colAvailabilityCounter)
            {
                for (int j = 0; j < length; j++)
                {
                    Console.Write(j + 1 + ": " + i[j].Count() + " \t ");
                }
                Console.WriteLine();
            }

            Console.WriteLine("GRID");
            foreach (var i in gridAvailabilityCounter)
            {
                for (int j = 0; j < length; j++)
                {
                    Console.Write(j + 1 + ": " + i[j].Count() + " \t ");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        public override void Insert(int value, int row, int col)
        {
            if (!InRange(row,col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");
            
            if (!GetSquarePossibilities(row, col).Contains(value))
                throw new InvalidInsertionException($"Cannot Insert '{value}' to {row},{col} in sudoku");

            UpdateRowInsert(value, row, col);
            UpdateColInsert(value, row, col);
            UpdateGridInsert(value, row, col);
            UpdateSquareInsert(value, row, col);
            count++;
        }

        private void UpdateSquareInsert(int value, int row, int col)
        {
            BitSet possible = GetSquarePossibilities(row, col);

            rowAvailabilityCounter[row][value - 1].ClearAll();
            colAvailabilityCounter[col][value - 1].ClearAll();
            gridAvailabilityCounter[GridPos(row, col)][value - 1].ClearAll();

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
                rowAvailabilityCounter[row][possibilite - 1].Remove(col + 1);
                posAva = rowAvailabilityCounter[row][possibilite - 1].Count();
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInRow(possibilite, row);

                colAvailabilityCounter[col][possibilite - 1].Remove(row + 1);
                posAva = colAvailabilityCounter[col][possibilite - 1].Count();
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInCol(possibilite, col);

                gridAvailabilityCounter[GridPos(row, col)][possibilite - 1].Remove(row % grid_height * grid_width + col % grid_width + 1);
                posAva = gridAvailabilityCounter[GridPos(row, col)][possibilite - 1].Count();
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInGrid(possibilite, GridPos(row, col));
            }
        }

        private void UpdateRowInsert(int value, int row, int col)
        {
            for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
            {
                if (rowPos == row) continue;
                if (sudoku[rowPos, col] != NONE) continue;
                BitSet p = GetSquarePossibilities(rowPos, col);
                if (!p.Contains(value)) continue;

                int count = p.Count();
                if (count <= 1) throw new InvalidInsertionException();
                if (count == 2) PotentialInsertInSquare(rowPos, col);
                
                singlePossibilitesCounter[count].Remove((rowPos, col));
                singlePossibilitesCounter[count - 1].Add((rowPos, col));

                rowAvailabilityCounter[rowPos][value - 1].Remove(col + 1);
                int posAva = rowAvailabilityCounter[rowPos][value - 1].Count();
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInRow(value, rowPos);

                gridAvailabilityCounter[GridPos(rowPos, col)][value - 1].Remove(rowPos % grid_height * grid_width + col % grid_width + 1);
                posAva = gridAvailabilityCounter[GridPos(rowPos, col)][value - 1].Count();
                
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInGrid(value, GridPos(rowPos, col));
            }
        }

        private void UpdateColInsert(int value, int row, int col)
        {

            for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
            {
                if (colPos == col) continue;
                if (sudoku[row, colPos] != NONE) continue;
                BitSet p = GetSquarePossibilities(row, colPos);
                if (!p.Contains(value)) continue;

                int count = p.Count();
                if (count <= 1) throw new InvalidInsertionException();
                if (count == 2) PotentialInsertInSquare(row, colPos);


                singlePossibilitesCounter[count].Remove((row, colPos));
                singlePossibilitesCounter[count - 1].Add((row, colPos));

                colAvailabilityCounter[colPos][value - 1].Remove(row + 1);
                int posAva = colAvailabilityCounter[colPos][value - 1].Count();
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInCol(value, colPos);

                gridAvailabilityCounter[GridPos(row, colPos)][value - 1].Remove(row % grid_height * grid_width + col % grid_width + 1);
                posAva = gridAvailabilityCounter[GridPos(row, colPos)][value - 1].Count();
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

                    BitSet p = GetSquarePossibilities(initRow + rowPos, initCol + colPos);
                    if (!p.Contains(value)) continue;
                    int count = p.Count();
                    if (count <= 1) throw new InvalidInsertionException();
                    if (count == 2) PotentialInsertInSquare(initRow + rowPos, initCol + colPos);

                    singlePossibilitesCounter[count].Remove((initRow + row, initCol + col));
                    singlePossibilitesCounter[count - 1].Add((initRow + rowPos, initCol + colPos));

                    if (initRow + rowPos != row)
                    {
                        rowAvailabilityCounter[initRow + rowPos][value - 1].Remove(initCol + colPos + 1);
                        int posAva = rowAvailabilityCounter[initRow + rowPos][value - 1].Count();
                        if (posAva == 0) throw new InvalidInsertionException();
                        if (posAva == 1) PotentialInsertInRow(value, initRow + rowPos);
                    }
                    if (initCol + colPos != col)
                    {
                        colAvailabilityCounter[initCol + colPos][value - 1].Remove(initRow + rowPos + 1);
                        int posAva = colAvailabilityCounter[initCol + colPos][value - 1].Count();
                        if (posAva == 0) throw new InvalidInsertionException();
                        if (posAva == 1) PotentialInsertInCol(value, initCol + colPos);
                    }
                }
            }
        }

        private void PotentialInsertInRow(int value, int row)
        {
            Console.WriteLine($"NEW INSERTION {value} in row: {row}");
            NextGarunteedAction.Push((value, row, (rowAvailabilityCounter[row][value - 1].GetSmallest() - 1)));
        }

        private void PotentialInsertInCol(int value, int col)
        {
            Console.WriteLine($"NEW INSERTION {value} in col: {col}");
            NextGarunteedAction.Push((value, colAvailabilityCounter[col][value - 1].GetSmallest() - 1, col));
        }

        private void PotentialInsertInGrid(int value, int grid)
        {
            Console.WriteLine($"NEW INSERTION {value} in grid: {grid}");
            int rowInPos = (gridAvailabilityCounter[grid][value - 1].GetSmallest() - 1) / grid_width;
            int colInPos = (gridAvailabilityCounter[grid][value - 1].GetSmallest() - 1) % grid_width;

            int initRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
            int initCol = grid * grid_width % sudoku.GetLength(1);

            NextGarunteedAction.Push((value, initRow + rowInPos, initCol + colInPos));
        }

        public void PotentialInsertInSquare(int row, int col)
        {
            BitSet possibilities = GetSquarePossibilities(row, col);
            Console.WriteLine("INSERT IN SQUARE ------------------------------------------");
            Console.WriteLine((possibilities.GetSmallest() + 1) + " " + row + " " +  col);
            NextGarunteedAction.Push((possibilities.GetSmallest() + 1, row, col));
        }


        public override void Remove(int row, int col)
        {

        }
        private BitSet GetSquarePossibilities(int row, int col)
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

        public bool IsSolved()
        {
            return size == count;
        }



    }
}
