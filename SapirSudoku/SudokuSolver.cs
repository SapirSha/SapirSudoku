using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
        double LLOAD = 0.5;
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
            /*
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
                else if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInCol(possibilite, row, col, posAva);

                gridAvailabilityCounter[GridPos(row, col)][possibilite - 1].Remove(row % grid_height * grid_width + col % grid_width + 1);
                posAva = gridAvailabilityCounter[GridPos(row, col)][possibilite - 1].Count();
                if (posAva == 0) throw new InvalidInsertionException();
                else if (posAva == 1) PotentialInsertInGrid(possibilite, GridPos(row, col));
                else
                {
                    if (posAva <= grid_width || posAva <= grid_height) PointingPair(possibilite, GridPos(row, col));
                    if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInGrid(possibilite, row, col, posAva);
                }
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
                else if (posAva == 1) PotentialInsertInRow(value, rowPos);
                else if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInRow(value, rowPos, col, posAva);

                gridAvailabilityCounter[GridPos(rowPos, col)][value - 1].Remove(rowPos % grid_height * grid_width + col % grid_width + 1);
                posAva = gridAvailabilityCounter[GridPos(rowPos, col)][value - 1].Count();

                if (posAva == 0) throw new InvalidInsertionException();
                else if (posAva == 1) PotentialInsertInGrid(value, GridPos(rowPos, col));
                else
                {
                    if (posAva <= grid_width || posAva <= grid_height) PointingPair(value, GridPos(rowPos, col));
                    if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInGrid(value, rowPos, col, posAva);
                }
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
                else if (posAva == 1) PotentialInsertInCol(value, colPos);
                else if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInCol(value, row, colPos, posAva);

                gridAvailabilityCounter[GridPos(row, colPos)][value - 1].Remove(row % grid_height * grid_width + colPos % grid_width + 1);
                posAva = gridAvailabilityCounter[GridPos(row, colPos)][value - 1].Count();
                if (posAva == 0) throw new InvalidInsertionException();
                else if (posAva == 1) PotentialInsertInGrid(value, GridPos(row, colPos));
                else
                {
                    if (posAva <= grid_width || posAva <= grid_height) PointingPair(value, GridPos(row, colPos));
                    if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInGrid(value, row, colPos, posAva);
                }

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
                        else if (posAva == 1) PotentialInsertInRow(value, initRow + rowPos);
                        else if (posAva <= sudoku.GetLength(1) * LLOAD) HiddenInRow(value, initRow + rowPos, initCol + colPos, posAva);
                    }
                    if (initCol + colPos != col)
                    {
                        colAvailabilityCounter[initCol + colPos][value - 1].Remove(initRow + rowPos + 1);
                        int posAva = colAvailabilityCounter[initCol + colPos][value - 1].Count();
                        if (posAva == 0) throw new InvalidInsertionException();
                        else if (posAva == 1) PotentialInsertInCol(value, initCol + colPos);
                        else if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInCol(value, initRow + rowPos, initCol + colPos, posAva);
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
        int i = 1;
        private void HiddenInGrid(int value, int row, int col, int hidden_type)
        {
            //Console.WriteLine($"IN HIDDEN CHECK {i++}");
            // THERE ARE X SQUARES WHERE V APPEARES IN THE GRID
            // IF THERE ARE X - 1 OTHER VALUES WHO ONLY APPEAR IN THE SAME GRIDS
            // HIDDEN VALUES IN GRID
            int grid = GridPos(row,col);

            // COLUMNS WHERE VALUE APPEARS AT
            BitSet possibilities_in_grid_for_value = gridAvailabilityCounter[grid][value - 1];
            BitSet possibilities_in_grid_other_values = gridAvailability[grid];

            BitSet Hidden = new BitSet(sudoku.GetLength(0));
            foreach (int possibility in possibilities_in_grid_other_values)
            {
                if (gridAvailabilityCounter[grid][possibility - 1].IsEmpty()) continue;
                else if (gridAvailabilityCounter[grid][possibility - 1].IsSubSetOf(possibilities_in_grid_for_value))
                    Hidden.Add(possibility);
                else if (gridAvailabilityCounter[grid][possibility - 1].IsSuperSetOf(possibilities_in_grid_for_value))
                {
                    if (gridAvailabilityCounter[grid][possibility - 1].Count() <= sudoku.GetLength(0) * LLOAD)
                    {
                        HiddenInGrid(possibility, row, col, gridAvailabilityCounter[grid][possibility - 1].Count());
                        return;
                    }
                }
            }

            if (Hidden.Count() >= hidden_type)
            {
                int initRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
                int initCol = grid * grid_width % sudoku.GetLength(1);
                Console.WriteLine($"REMOVING POSS FOR: V {value} - G {grid} - T{hidden_type}");
                Console.WriteLine("Grid Appears POS: " + possibilities_in_grid_for_value);
                Console.WriteLine("Hidden NUMBERS: " + Hidden);
                foreach (int i in Hidden)
                    Console.Write(i);
                Console.WriteLine();
                foreach (int position in possibilities_in_grid_for_value)
                {
                    row =initRow + (position - 1) / grid_width;
                    col = initCol + (position - 1) % grid_width;
                    Console.WriteLine($"REMOVING POSSIBILITIES FOR: {value} {row},{col}");
                    Console.WriteLine("CURRENT: " + squarePossibilities[row, col]);
                    Console.WriteLine("HIDDEN : " + Hidden);
                    Console.WriteLine("HIDDENS ___________________:");

                    foreach (int i in Hidden)
                        Console.Write(i);
                    Console.WriteLine();
                    foreach (int possibility_in_old in squarePossibilities[row, col])
                    {
                        if (Hidden.Contains(possibility_in_old)) continue;
                        squarePossibilities[row, col].Remove(possibility_in_old);
                        int posAva;
                        colAvailabilityCounter[col][possibility_in_old - 1].Remove(row + 1);
                        posAva = colAvailabilityCounter[col][possibility_in_old - 1].Count();
                        if (posAva == 0) throw new InvalidInsertionException();
                        else if (posAva == 1) PotentialInsertInCol(possibility_in_old, col);
                        else if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInCol(possibility_in_old, row, col, posAva);

                        rowAvailabilityCounter[row][possibility_in_old - 1].Remove(col + 1);
                        posAva = rowAvailabilityCounter[row][possibility_in_old - 1].Count();
                        if (posAva == 0)
                        {
                            Console.WriteLine($"R{row} C{col} --- V{possibility_in_old} --- OLD {squarePossibilities[row, col]}");
                            Console.WriteLine(rowAvailabilityCounter[row][possibility_in_old - 1]);
                            throw new InvalidInsertionException();
                        }
                        else if (posAva == 1) PotentialInsertInRow(possibility_in_old, row);
                        else if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInRow(possibility_in_old, row, col, posAva);

                        gridAvailabilityCounter[GridPos(row, col)][possibility_in_old - 1].Remove(row % grid_height * grid_width + col % grid_width + 1);
                        posAva = gridAvailabilityCounter[GridPos(row, col)][possibility_in_old - 1].Count();
                        if (posAva == 0) throw new InvalidInsertionException();
                        else if (posAva == 1) PotentialInsertInGrid(possibility_in_old, GridPos(row, col));
                        else
                        {
                            if (posAva <= grid_width || posAva <= grid_height) PointingPair(value, GridPos(row, col));
                            if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInGrid(possibility_in_old, row, col, posAva);
                        }
                    }

                    Console.WriteLine("INTER: " + BitSet.Intersection(squarePossibilities[row, col], Hidden));

                    if (!squarePossibilities[row, col].Equals(BitSet.Intersection(squarePossibilities[row, col], Hidden)))
                    {
                        Console.WriteLine("ERROR");
                        Console.WriteLine(squarePossibilities[row, col]);
                        Console.WriteLine(BitSet.Intersection(squarePossibilities[row, col], Hidden));
                    }
                }
            }
        }
        
        private void HiddenInRow(int value, int row, int col, int hidden_type)
        {
            //Console.WriteLine($"IN HIDDEN CHECK {i++}");
            // THERE ARE X SQUARES WHERE V APPEARES IN THE ROW
            // IF THERE ARE X - 1 OTHER VALUES WHO ONLY APPEAR IN THE SAME ROW
            // HIDDEN VALUES IN ROW
            BitSet possibilities_in_row_for_value = rowAvailabilityCounter[row][value - 1];
            BitSet possibilities_in_row_other_values = rowAvailability[row];

            BitSet Hidden = new BitSet(sudoku.GetLength(0));
            foreach (int possibility in possibilities_in_row_other_values)
            {
                if (rowAvailabilityCounter[row][possibility - 1].IsEmpty()) continue;
                if (rowAvailabilityCounter[row][possibility - 1].IsSubSetOf(possibilities_in_row_for_value))
                    Hidden.Add(possibility);
                else if (rowAvailabilityCounter[row][possibility - 1].IsSuperSetOf(possibilities_in_row_for_value))
                {
                    if (rowAvailabilityCounter[row][possibility - 1].Count() <= sudoku.GetLength(0) * LLOAD)
                    {
                        HiddenInRow(possibility, row, col, rowAvailabilityCounter[row][possibility - 1].Count());
                        return;
                    }
                }
            }
            
            if (Hidden.Count() >= hidden_type)
            {
                Console.WriteLine($"REMOVING POSS FOR: V {value} - R {row}");
                Console.WriteLine("Row Appears POS: " + possibilities_in_row_for_value);
                Console.WriteLine("Hidden NUMBERS: " + Hidden);
                foreach (int position in possibilities_in_row_for_value)
                {

                    col = position - 1;
                    Console.WriteLine($"REMOVING POSSIBILITIES FOR: {value} {row},{col}");
                    Console.WriteLine("CURRENT:\t" + squarePossibilities[row, col]);
                    Console.WriteLine("HIDDEN: \t" + Hidden);
                    Console.WriteLine("HIDDENS ___________________:");
                    foreach (int i in Hidden)
                        Console.Write(i);
                    Console.WriteLine();


                    foreach (int possibility_in_old in squarePossibilities[row, col])
                    {
                        if (Hidden.Contains(possibility_in_old)) continue;
                        squarePossibilities[row, col].Remove(possibility_in_old);
                        int posAva;
                        colAvailabilityCounter[col][possibility_in_old - 1].Remove(row + 1);
                        posAva = colAvailabilityCounter[col][possibility_in_old - 1].Count();
                        if (posAva == 0) throw new InvalidInsertionException();
                        else if (posAva == 1) PotentialInsertInCol(possibility_in_old, col);
                        else if (posAva <= sudoku.GetLength(0) * LLOAD)
                        {
                            if (col == 8)
                                Console.WriteLine("Here _________________------------------------------------------------------____________");
                            HiddenInCol(possibility_in_old, row, col, posAva);
                        }

                        rowAvailabilityCounter[row][possibility_in_old - 1].Remove(col + 1);
                        posAva = rowAvailabilityCounter[row][possibility_in_old - 1].Count();
                        if (posAva == 0) throw new InvalidInsertionException();
                        else if (posAva == 1) PotentialInsertInRow(possibility_in_old, row);
                        else if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInRow(possibility_in_old, row, col, posAva);

                        gridAvailabilityCounter[GridPos(row, col)][possibility_in_old - 1].Remove(row % grid_height * grid_width + col % grid_width + 1);
                        posAva = gridAvailabilityCounter[GridPos(row, col)][possibility_in_old - 1].Count();
                        if (posAva == 0) throw new InvalidInsertionException();
                        else if (posAva == 1) PotentialInsertInGrid(possibility_in_old, GridPos(row, col));
                        else
                        {
                            if (posAva <= grid_width || posAva <= grid_height) PointingPair(value, GridPos(row, col));
                            if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInGrid(possibility_in_old, row, col, posAva);
                        }
                    }

                    Console.WriteLine("INTER:  \t" + BitSet.Intersection(squarePossibilities[row, col], Hidden));
                    if (!squarePossibilities[row, col].Equals(BitSet.Intersection(squarePossibilities[row, col], Hidden)))
                    {
                        Console.WriteLine("ERROR");
                        Console.WriteLine(squarePossibilities[row, col]);
                        Console.WriteLine(BitSet.Intersection(squarePossibilities[row, col], Hidden));
                    }
                }
            }
        }

        private void HiddenInCol(int value, int row, int col, int hidden_type)
        {
            //Console.WriteLine($"IN HIDDEN CHECK {i++}");
            // THERE ARE X SQUARES WHERE V APPEARES IN THE COL
            // IF THERE ARE X - 1 OTHER VALUES WHO ONLY APPEAR IN THE SAME COL
            // HIDDEN VALUES IN COL
            BitSet possibilities_in_col_for_value = colAvailabilityCounter[col][value - 1];
            BitSet possibilities_in_col_other_values = colAvailability[col];
            BitSet Hidden = new BitSet(sudoku.GetLength(0));
            foreach (int possibility in possibilities_in_col_other_values)
            {
                if (colAvailabilityCounter[col][possibility - 1].IsEmpty()) continue;
                if (colAvailabilityCounter[col][possibility - 1].IsSubSetOf(possibilities_in_col_for_value))
                    Hidden.Add(possibility);
                else if (colAvailabilityCounter[col][possibility - 1].IsSuperSetOf(possibilities_in_col_for_value))
                {
                    if (colAvailabilityCounter[col][possibility - 1].Count() <= sudoku.GetLength(0) * LLOAD)
                    {
                        HiddenInCol(possibility, row, col, colAvailabilityCounter[col][possibility - 1].Count());
                        return;
                    }
                }
            }
            row = possibilities_in_col_for_value.GetSmallest() - 1;
            if (col == 8 && (value == 2 || value == 4 || value == 5 || value == 9) )
            {
                Console.WriteLine("HERE --------------------------------------------------------");
                Console.WriteLine($"V {value}: R{row},C{col} -> T{hidden_type}");
                Console.WriteLine($"HIDDEN - {Hidden}");
                Console.WriteLine("2: " + colAvailabilityCounter[col][1]);
                Console.WriteLine("4: " + colAvailabilityCounter[col][3]);
                Console.WriteLine("5: " + colAvailabilityCounter[col][4]);
                Console.WriteLine("9: " + colAvailabilityCounter[col][8]);
            }

            if (Hidden.Count() >= hidden_type)
            {
                Console.WriteLine($"V {value}: R{row},C{col} -> T{hidden_type}");
                Console.WriteLine("Col Appears POS: " + possibilities_in_col_for_value);
                Console.WriteLine("Hidden NUMBERS: " + Hidden);
                foreach (int position in possibilities_in_col_for_value)
                {
                    row = position - 1;
                    Console.WriteLine($"REMOVING POSSIBILITIES FOR: {value} {row},{col}");
                    Console.WriteLine("CURRENT:\t" + squarePossibilities[row, col]);
                    Console.WriteLine("HIDDEN: \t" + Hidden);
                    foreach(int possibility_in_old in squarePossibilities[row, col])
                    {
                        if (Hidden.Contains(possibility_in_old)) continue;
                        squarePossibilities[row, col].Remove(possibility_in_old);
                        int posAva;
                        colAvailabilityCounter[col][possibility_in_old - 1].Remove(row + 1);
                        posAva = colAvailabilityCounter[col][possibility_in_old - 1].Count();
                        if (posAva == 0) throw new InvalidInsertionException();
                        else if (posAva == 1) PotentialInsertInCol(possibility_in_old, col);
                        else if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInCol(possibility_in_old, row, col, posAva);

                        rowAvailabilityCounter[row][possibility_in_old - 1].Remove(col + 1);
                        posAva = rowAvailabilityCounter[row][possibility_in_old - 1].Count();
                        if (posAva == 0) throw new InvalidInsertionException();
                        else if (posAva == 1) PotentialInsertInRow(possibility_in_old, row);
                        else if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInRow(possibility_in_old, row, col, posAva);
                        
                        gridAvailabilityCounter[GridPos(row, col)][possibility_in_old - 1].Remove(row % grid_height * grid_width + col % grid_width + 1);
                        posAva = gridAvailabilityCounter[GridPos(row,col)][possibility_in_old - 1].Count();
                        if (posAva == 0) throw new InvalidInsertionException();
                        else if (posAva == 1) PotentialInsertInGrid(possibility_in_old, GridPos(row, col));
                        else
                        {
                            if (posAva <= grid_width || posAva <= grid_height) PointingPair(value, GridPos(row, col));
                            if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInGrid(possibility_in_old, row, col, posAva);
                        }
                    }

                    Console.WriteLine("INTER:  \t" + BitSet.Intersection(squarePossibilities[row, col], Hidden));
                    if (!squarePossibilities[row, col].Equals(BitSet.Intersection(squarePossibilities[row, col], Hidden)))
                    {
                        Console.WriteLine("ERROR");
                        Console.WriteLine(squarePossibilities[row, col]);
                        Console.WriteLine(BitSet.Intersection(squarePossibilities[row, col], Hidden));
                    }
                }
            }
        }

        public bool IsRowPointingGroup(int value, int grid)
        {
            BitSet possibilities = gridAvailabilityCounter[grid][value - 1];
            if (possibilities.IsEmpty()) return false;
            BitSet[] fullRows = new BitSet[grid_height];
            for (int row = 0; row < grid_height; row++) {
                fullRows[row] = new BitSet(grid_width);
                for(int col =0; col < grid_width; col++)
                {
                    fullRows[row].Add(col + row * grid_width + 1);
                }
            }
            for (int row = 0; row < grid_height; row++)
                if (possibilities.IsSubSetOf(fullRows[row]))
                    return true;
            return false;
        }

        public void PointingPair(int value, int grid)
        {
            if (IsRowPointingGroup(value, grid))
            {
                BitSet positions = gridAvailabilityCounter[grid][value - 1];
                int initRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
                int row = (positions.GetSmallest() - 1) / grid_width;
                int initCol = grid % (sudoku.GetLength(1) / grid_width) * grid_width;

                BitSet other_positions = rowAvailabilityCounter[initRow + row][value - 1];
                Console.WriteLine($"-------------------------------POINTING PAIR V{value}-G{grid}");
                foreach(int col in other_positions)
                {
                    if (GridPos(row, col - 1) == grid) continue;
                    Console.WriteLine($"REMOVING V{value} from {initRow + row},{col}");
                    squarePossibilities[initRow + row, col - 1].Remove(value);
                    
                    int posAva;
                    colAvailabilityCounter[col - 1][value - 1].Remove(initRow + row + 1);
                    posAva = colAvailabilityCounter[col - 1][value - 1].Count();
                    if (posAva == 0) throw new InvalidInsertionException();
                    else if (posAva == 1) PotentialInsertInCol(value, col - 1);
                    else if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInCol(value, initRow + row, col - 1, posAva);

                    rowAvailabilityCounter[initRow + row][value - 1].Remove(col);
                    posAva = rowAvailabilityCounter[initRow + row][value - 1].Count();
                    if (posAva == 0) throw new InvalidInsertionException();
                    else if (posAva == 1) PotentialInsertInRow(value, initRow + row);
                    else if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInRow(value, initRow + row, col - 1, posAva);

                    gridAvailabilityCounter[GridPos(initRow + row, col - 1)][value - 1].Remove((initRow + row) % grid_height * grid_width + (col - 1) % grid_width + 1);
                    posAva = gridAvailabilityCounter[GridPos(initRow + row, col - 1)][value - 1].Count();
                    if (posAva == 0) throw new InvalidInsertionException();
                    else if (posAva == 1) PotentialInsertInGrid(value, grid);
                    if (posAva <= sudoku.GetLength(0) * LLOAD) HiddenInGrid(value, initRow + row, col - 1, posAva);
                }
            }
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
