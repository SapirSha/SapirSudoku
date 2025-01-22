﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using CustomExceptions;
using SapirStruct;
using SapirSudoku;

namespace SapirSudoku
{
    public class SudokuSolver : Sudoku
    {
        float LLOAD = 0.3f;
        public HashSet<(int row,int col)>[] squarePossibilitesCounter;

        public BitSet[,] squarePossibilities;

        public BitSet[] rowAvailability; // Amount possible in all row
        public BitSet[][] rowAvailabilityCounter;
        public BitSet[] colAvailability; // Amount possible in all col
        public BitSet[][] colAvailabilityCounter;
        public BitSet[] gridAvailability;// Amount possible in all grid
        public BitSet[][] gridAvailabilityCounter;

        public BitSet full; // represents a full bitset in range 1 to N

        public Stack<(int value, int row, int col)> NextGarunteedAction;
        public Stack<Stack<(int row, int col)>> PrevAction;

        int size;
        int count;

        public SudokuSolver(Sudoku s) : this(s.CloneGrid()) { }
        
        private SudokuSolver(int length = 9) : base(length){
            this.full = new BitSet(length);
            for (int i = 1; i <= length; i++)
                full.Add(i);

            squarePossibilities = new BitSet[length,length];
            for (int row = 0; row < length; row++)
            {
                for (int col = 0; col < length; col++)
                {
                    squarePossibilities[row, col] = new BitSet(full);
                }
            }


        size = sudoku.Length;
            count = 0;

            this.squarePossibilitesCounter = new HashSet<(int, int)>[length + 1];
            for (int i = 0; i <= length; i++)
                squarePossibilitesCounter[i] = new HashSet<(int, int)> (length * length);
            for (int row = 0; row < length; row++)
                for (int col = 0; col < length; col++)
                    squarePossibilitesCounter[length].Add((row,col));

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
            this.PrevAction = new Stack<Stack<(int row, int col)> >(length);
        }
        public SudokuSolver(int[,] grid) : this(grid.GetLength(0))
        {
            int length = grid.GetLength(0);
            if (grid.GetLength(1) != length)
                throw new InvalidSudokuException($"Sudoku size must be N*N, instead was {grid.GetLength(0)}*{grid.GetLength(1)}");

            PrevAction.Push(new Stack<(int row, int col)>(length));
            for (int row = 0; row < sudoku.GetLength(0); row++)
                for (int col = 0; col < sudoku.GetLength(1); col++)
                    if (sudoku[row, col] == NONE && grid[row, col] != NONE)
                        Insert(grid[row, col], row, col);





            PrevAction.Clear();
            PrevAction.Push(new Stack<(int row, int col)>(length));
            
            while (NextGarunteedAction.Count() != 0)
            {
                (int value, int row, int col) = NextGarunteedAction.Pop();
                if (GetRealSquarePossibilities(row, col).Contains(value))
                    Insert(value, row, col);
            }
            if (IsSolved()) return;
            
            foreach (var i in PrevAction.Peek())
                Remove(i.row, i.col);
            PrevAction.Pop();

            /*
            PrevAction.Push(new Stack<(int row, int col)>());
            (int y, int x) = squarePossibilitesCounter[2].First();
            Console.WriteLine(x +" + " +y);
            Insert(squarePossibilities[y, x].GetSmallest(), y, x);

            while (NextGarunteedAction.Count() != 0)
            {
                (int value, int row, int col) = NextGarunteedAction.Pop();
                if (GetSquarePossibilities(row, col).Contains(value))
                    Insert(value, row, col);
            }
            /*
            foreach (var i in PrevAction.Peek())
                Remove(i.row, i.col);
            PrevAction.Pop();

            PrevAction.Push(new Stack<(int row, int col)>());
            (y, x) = squarePossibilitesCounter[3].Last();
            Console.WriteLine(x + " + " + y);
            Insert(squarePossibilities[y, x].GetSmallest(), y, x);

            while (NextGarunteedAction.Count() != 0)
            {
                (int value, int row, int col) = NextGarunteedAction.Pop();
                if (GetSquarePossibilities(row, col).Contains(value))
                    Insert(value, row, col);
            }
            /*
            foreach (var i in PrevAction.Peek())
                Remove(i.row, i.col);
            */
            /*
            foreach (var i in PrevAction)
            {
                Console.WriteLine(i);
                foreach (var j in i)
                    Console.WriteLine(j);
            }
            */

        }

        public override void Insert(int value, int row, int col)
        {
            if (!InRange(row,col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");
            
            if (!GetRealSquarePossibilities(row, col).Contains(value))
                throw new InvalidInsertionException($"Cannot Insert '{value}' to {row},{col} in sudoku");

            UpdateRowInsert(value, row, col);
            UpdateColInsert(value, row, col);
            UpdateGridInsert(value, row, col);
            UpdateSquareInsert(value, row, col);
            count++;
            PrevAction.Peek().Push((row,col));
        }

        private void UpdateSquareInsert(int value, int row, int col)
        {
            BitSet possible = GetRealSquarePossibilities(row, col);

            rowAvailabilityCounter[row][value - 1].ClearAll();
            colAvailabilityCounter[col][value - 1].ClearAll();
            gridAvailabilityCounter[GridPos(row, col)][value - 1].ClearAll();

            rowAvailability[row].Remove(value);
            colAvailability[col].Remove(value);
            gridAvailability[GridPos(row, col)].Remove(value);

            squarePossibilitesCounter[possible.Count()].Remove((row, col));
            squarePossibilitesCounter[0].Add((row, col));

            squarePossibilities[row, col] = new BitSet(0);
            sudoku[row, col] = value;

            possible.Remove(value);
            foreach (int possibilite in possible)
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
                if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInGrid(possibilite, row, col);
            }
        }

        private void UpdateRowInsert(int value, int row, int col)
        {
            for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
            {
                if (GridPos(rowPos, col) == GridPos(row, col)) continue;
                if (sudoku[rowPos, col] != NONE) continue;
                BitSet p = GetRealSquarePossibilities(rowPos, col);

                if (!p.Contains(value)) continue;

                int count = p.Count();
                if (count <= 1) throw new InvalidInsertionException();
                if (count == 2) PotentialInsertInSquare(rowPos, col);

                squarePossibilities[rowPos, col].Remove(value);
                squarePossibilitesCounter[count].Remove((rowPos, col));
                squarePossibilitesCounter[count - 1].Add((rowPos, col));

                rowAvailabilityCounter[rowPos][value - 1].Remove(col + 1);
                int posAva = rowAvailabilityCounter[rowPos][value - 1].Count();
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInRow(value, rowPos);

                gridAvailabilityCounter[GridPos(rowPos, col)][value - 1].Remove(rowPos % grid_height * grid_width + col % grid_width + 1);
                posAva = gridAvailabilityCounter[GridPos(rowPos, col)][value - 1].Count();
                
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInGrid(value, GridPos(rowPos, col));
                if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInGrid(value, rowPos, col);

            }
        }

        private void UpdateColInsert(int value, int row, int col)
        {
            for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
            {
                if (GridPos(row,colPos) == GridPos(row,col)) continue;
                if (sudoku[row, colPos] != NONE) continue;
                BitSet p = GetRealSquarePossibilities(row, colPos);
                if (!p.Contains(value)) continue;

                int count = p.Count();
                if (count <= 1) throw new InvalidInsertionException();
                if (count == 2) PotentialInsertInSquare(row, colPos);

                squarePossibilities[row, colPos].Remove(value);
                squarePossibilitesCounter[count].Remove((row, colPos));
                squarePossibilitesCounter[count - 1].Add((row, colPos));

                colAvailabilityCounter[colPos][value - 1].Remove(row + 1);
                int posAva = colAvailabilityCounter[colPos][value - 1].Count();
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInCol(value, colPos);

                gridAvailabilityCounter[GridPos(row, colPos)][value - 1].Remove(row % grid_height * grid_width + colPos % grid_width + 1);
                posAva = gridAvailabilityCounter[GridPos(row, colPos)][value - 1].Count();
                if (posAva == 0) throw new InvalidInsertionException();
                if (posAva == 1) PotentialInsertInGrid(value, GridPos(row, colPos));
                if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInGrid(value, row, colPos);

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

                    BitSet p = GetRealSquarePossibilities(initRow + rowPos, initCol + colPos);
                    if (!p.Contains(value)) continue;
                    int count = p.Count();
                    if (count <= 1) throw new InvalidInsertionException();
                    if (count == 2) PotentialInsertInSquare(initRow + rowPos, initCol + colPos);

                    squarePossibilities[initRow + rowPos, initCol + colPos].Remove(value);
                    squarePossibilitesCounter[count].Remove((initRow + rowPos, initCol + colPos));
                    squarePossibilitesCounter[count - 1].Add((initRow + rowPos, initCol + colPos));

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
            NextGarunteedAction.Push((value, row, (rowAvailabilityCounter[row][value - 1].GetSmallest() - 1)));
        }

        private void PotentialInsertInCol(int value, int col)
        {
            NextGarunteedAction.Push((value, colAvailabilityCounter[col][value - 1].GetSmallest() - 1, col));
        }

        private void PotentialInsertInGrid(int value, int grid)
        {
            int rowInPos = (gridAvailabilityCounter[grid][value - 1].GetSmallest() - 1) / grid_width;
            int colInPos = (gridAvailabilityCounter[grid][value - 1].GetSmallest() - 1) % grid_width;

            int initRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
            int initCol = grid * grid_width % sudoku.GetLength(1);

            NextGarunteedAction.Push((value, initRow + rowInPos, initCol + colInPos));
        }

        public void PotentialInsertInSquare(int row, int col)
        {
            BitSet possibilities = GetRealSquarePossibilities(row, col);
            NextGarunteedAction.Push((possibilities.GetSmallest() + 1, row, col));
        }


        public override void Remove(int row, int col)
        {
            if (!InRange(row,col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");

            if (sudoku[row, col] == NONE) 
                throw new InvalidRemovalException($"Cannot remove in an empty square {row},{col}");

            int value = sudoku[row, col];
            UpdateSquareRemove(value, row, col);
            UpdateRowRemove(value, row, col);
            UpdateColRemove(value, row, col);
            UpdateGridRemove(value, row, col);
        }

        private void UpdateSquareRemove(int value, int row, int col)
        {
            sudoku[row, col] = NONE;
            rowAvailability[row].Add(value);
            colAvailability[col].Add(value);
            gridAvailability[GridPos(row, col)].Add(value);


            BitSet possibilities = GetSquarePossibilities(row, col);

            squarePossibilitesCounter[0].Remove((row, col));
            squarePossibilitesCounter[possibilities.Count()].Add((row, col));

            foreach (int possibilite in possibilities) 
            {
                squarePossibilities[row, col].Add(possibilite);
                rowAvailabilityCounter[row][possibilite - 1].Add(col + 1);
                colAvailabilityCounter[col][possibilite - 1].Add(row + 1);
                gridAvailabilityCounter[GridPos(row, col)][possibilite - 1].Add(row % grid_height * grid_width + col % grid_width + 1);
            }

        }

        private void UpdateRowRemove(int value, int row, int col)
        {
            for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
            {
                if (rowPos == row) continue;
                if (sudoku[rowPos, col] != NONE) continue;
                BitSet p = GetSquarePossibilities(rowPos, col);
                if (!p.Contains(value)) continue;

                int count = p.Count();
                squarePossibilities[rowPos, col].Add(value);
                squarePossibilitesCounter[count - 1].Remove((rowPos, col));
                squarePossibilitesCounter[count].Add((rowPos, col));

                rowAvailabilityCounter[rowPos][value - 1].Add(col + 1);
                gridAvailabilityCounter[GridPos(rowPos, col)][value - 1].Add(rowPos % grid_height * grid_width + col % grid_width + 1);
                
                colAvailabilityCounter[col][value - 1].Add(rowPos + 1);
            }
        }

        private void UpdateColRemove(int value, int row, int col)
        {
            for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
            {
                if (colPos == col) continue;
                if (sudoku[row, colPos] != NONE) continue;

                BitSet p = GetSquarePossibilities(row, colPos);
                if (!p.Contains(value)) continue;

                int count = p.Count();
                squarePossibilities[row, colPos].Add(value);
                squarePossibilitesCounter[count - 1].Remove((row, colPos));
                squarePossibilitesCounter[count].Add((row, colPos));

                colAvailabilityCounter[colPos][value - 1].Add(row + 1);
                gridAvailabilityCounter[GridPos(row, colPos)][value - 1].Add(row % grid_height * grid_width + colPos % grid_width + 1);
                
                rowAvailabilityCounter[row][value - 1].Add(colPos + 1);
            }
        }

        private void UpdateGridRemove(int value, int row, int col)
        {
            int initRow = GridPos(row, col) / (sudoku.GetLength(1) / grid_width) * grid_height;
            int initCol = GridPos(row, col) * grid_width % sudoku.GetLength(1);
            for (int rowPos = 0; rowPos < grid_height; rowPos++)
            {
                for (int colPos = 0; colPos <grid_width; colPos++)
                {
                    if (initRow + rowPos == row && initCol + colPos == col) continue;
                    if (sudoku[initRow + rowPos, initCol + colPos] != NONE) continue;
                    BitSet p = GetSquarePossibilities(initRow + rowPos, initCol + colPos);
                    if (!p.Contains(value)) continue;

                    int count = p.Count();
                    squarePossibilities[initRow + rowPos, initCol + colPos].Add(value);
                    squarePossibilitesCounter[count - 1].Remove((initRow + rowPos, initCol + colPos));
                    squarePossibilitesCounter[count].Add((initRow + rowPos, initCol + colPos));

                    rowAvailabilityCounter[initRow + rowPos][value - 1].Add(initCol + colPos + 1);
                    colAvailabilityCounter[initCol + colPos][value - 1].Add(initRow + rowPos + 1);
                    gridAvailabilityCounter[GridPos(initRow + rowPos, initCol + colPos)][value - 1]
                        .Add((initRow + rowPos) % grid_height * grid_width + (initCol + colPos) % grid_width + 1);
                }
            }
        }

        private void HiddenInGrid(int value, int row, int col)
        {
            Console.WriteLine($"HELLO {value}: {row},{col}");
            BitSet possibilities_in_grid_positions = gridAvailabilityCounter[GridPos(row, col)][value - 1];
            BitSet poss = GetRealSquarePossibilities
                (row = (possibilities_in_grid_positions.GetSmallest() - 1) / grid_width, col = (possibilities_in_grid_positions.GetSmallest() - 1) % grid_width);

            Console.WriteLine("\n" + value +" " + possibilities_in_grid_positions);
            Console.WriteLine("P " + GetRealSquarePossibilities(row, col));
            Console.WriteLine("___");
            int other = -1;
            foreach (int p in poss)
            {
                if (p == value || gridAvailabilityCounter[GridPos(row, col)][p - 1].Count() == 0) continue;
                Console.WriteLine(p + " " + gridAvailabilityCounter[GridPos(row, col)][p - 1]);
                if (gridAvailabilityCounter[GridPos(row, col)][p - 1].IsSubSetOf(possibilities_in_grid_positions))
                {
                    Console.WriteLine($"HERE {value}-{p} {row},{col}");
                    other = p;
                }
            }

            if (other != -1)
            {
                BitSet inSet = new BitSet(2);
                inSet.Add(value);
                inSet.Add(other);
                foreach (int p2 in possibilities_in_grid_positions)
                {
                    row = (p2- 1) / grid_width;
                    col = (p2 - 1) % grid_width;
                    BitSet poss3 = GetRealSquarePossibilities(row, col);
                    squarePossibilities[row, col] = new BitSet(inSet); //
                }
            }
            

            //Console.WriteLine(" " + row + " " + col);
            //Console.WriteLine(gridAvailabilityCounter[GridPos(row, col)][3]);


        }

        public BitSet GetSquarePossibilities(int row, int col)
        {
            if (!InRange(row,col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");
            if (sudoku[row, col] != NONE) return new BitSet(0);

            return BitSet.Intersection(rowAvailability[row], colAvailability[col], gridAvailability[GridPos(row, col)]);
        }

        public BitSet GetRealSquarePossibilities(int row, int col)
        {
            if (!InRange(row, col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");
            if (sudoku[row, col] != NONE) return new BitSet(0);

            return squarePossibilities[row,col];
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

        public void printPoss()
        {
            for(int i = 0; i < squarePossibilities.GetLength(0); i++)
            {
                Console.Write($"{i}:");

                for (int j = 0; j < squarePossibilities.GetLength(1); j++)
                {
                    String msg = "";
                    Console.Write(j+":");
                    foreach (int v in squarePossibilities[i, j])
                        msg += v;
                    Console.Write($"{msg, -10}");
                }
                Console.WriteLine();
            }

        }



    }
}
