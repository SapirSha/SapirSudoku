using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        
        public SudokuSolver(int length = 9) : base(length){
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
                Console.WriteLine($"HERE {value} - {row},{col}");
                if (GetRealSquarePossibilities(row, col).Contains(value))
                    Insert(value, row, col);
            }
            if (IsSolved()) Console.WriteLine("SOLVED");

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

        public void RemoveSquarePossibility(int value, int row, int col)
        {
            if (!squarePossibilities[row, col].Contains(value)) return;
            int count;

            squarePossibilities[row, col].Remove(value);
            count = squarePossibilities[row, col].Count();
            if (count == 0) throw new InvalidInsertionException();
            else if (count == 1) PotentialInsertInSquare(row, col);

            squarePossibilitesCounter[count + 1].Remove((row, col));
            squarePossibilitesCounter[count].Add((row, col));

            rowAvailabilityCounter[row][value - 1].Remove(col + 1);
            count = rowAvailabilityCounter[row][value - 1].Count();
            if (count == 0) throw new InvalidInsertionException();
            else if (count == 1) PotentialInsertInRow(value, row);
            else if (count <= sudoku.GetLength(0) * LLOAD) HiddenInRow(value, row, col, count);

            colAvailabilityCounter[col][value - 1].Remove(row + 1);
            count = colAvailabilityCounter[col][value - 1].Count();
            if (count == 0) throw new InvalidInsertionException();
            else if (count == 1) PotentialInsertInCol(value, col);
            else if (count <= sudoku.GetLength(0) * LLOAD) HiddenInCol(value, row, col, count);

            gridAvailabilityCounter[GridPos(row, col)][value - 1].Remove(row % grid_height * grid_width + col % grid_width + 1);
            count = gridAvailabilityCounter[GridPos(row, col)][value - 1].Count();
            if (count == 0) throw new InvalidInsertionException();
            else if (count == 1) PotentialInsertInGrid(value, GridPos(row, col));
            else
            {
                if (count <= grid_width || count <= grid_height) PointingPair(value, GridPos(row, col));
                if (count <= sudoku.GetLength(0) * LLOAD) HiddenInGrid(value, row, col, count);
            }

            // NOTICE THAT SOMETIMES I WANT TO CHANGE ONLY TWO <----
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

                RemoveSquarePossibility(value, rowPos, col);
            }
        }

        private void UpdateColInsert(int value, int row, int col)
        {
            for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
            {
                if (GridPos(row,colPos) == GridPos(row,col)) continue;
                if (sudoku[row, colPos] != NONE) continue;

                RemoveSquarePossibility(value, row, colPos);
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

                    RemoveSquarePossibility(value, initRow + rowPos, initCol + colPos);
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
            NextGarunteedAction.Push((possibilities.GetSmallest(), row, col));
        }


        private void HiddenInGrid(int value, int row, int col, int hidden_type)
        {
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
                    row = initRow + (position - 1) / grid_width;
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
                        RemoveSquarePossibility(possibility_in_old, row, col);
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
                        RemoveSquarePossibility(possibility_in_old, row, col);
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
                        RemoveSquarePossibility(possibility_in_old, row, col);
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
            // MAKE STATIC
            BitSet[] fullRows = new BitSet[grid_height];
            for (int i = 0; i < grid_height; i++)
            {
                fullRows[i] = new BitSet(grid_width);
                for (int j = 0; j < grid_width; j++)
                    fullRows[i].Add(i * grid_width + j + 1);
            }
            // MAKE STATIC

            BitSet possitions = gridAvailabilityCounter[grid][value - 1];
            if (possitions.IsEmpty()) return false;

            foreach (BitSet possibleRow in fullRows)
                if (possitions.IsSubSetOf(possibleRow))
                    return true;
            return false;
        }

        public bool IsColPointingGroup(int value, int grid)
        {
            // MAKE STATIC
            BitSet[] fullCols = new BitSet[grid_width];
            for (int i = 0; i < grid_width; i++)
            {
                fullCols[i] = new BitSet(sudoku.GetLength(0));
                for (int j = 0; j < grid_height; j++)
                    fullCols[i].Add(i + j * grid_height + 1);
            }
            // MAKE STATIC

            BitSet possitions = gridAvailabilityCounter[grid][value - 1];
            if (possitions.IsEmpty()) return false;

            foreach (BitSet possibleRow in fullCols)
                if (possitions.IsSubSetOf(possibleRow))
                    return true;
            return false;
        }

        public void PointingPair(int value, int grid)
        {
            if (IsRowPointingGroup(value, grid))
            {
                Console.WriteLine($"POINTING ROW V{value} - G{grid}");
                int initRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
                int row = initRow + (gridAvailabilityCounter[grid][value - 1].GetSmallest() - 1) / grid_width;
                for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
                {
                    if (GridPos(row, colPos) == grid) continue;
                    if (squarePossibilities[row,colPos].Contains(value)) Console.WriteLine($"Removing {value} from {row},{colPos}");
                    RemoveSquarePossibility(value, row, colPos);
                }
            }
            else if (IsColPointingGroup(value, grid))
            {
                Console.WriteLine($"COL POINTING GROUP V{value} - G{grid}");
                int initCol = grid * grid_width % sudoku.GetLength(1);
                int col = initCol + (gridAvailabilityCounter[grid][value - 1].GetSmallest() - 1) % grid_width;
                for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
                {
                    if (GridPos(rowPos, col) == grid) continue;
                    if (squarePossibilities[rowPos, col].Contains(value)) Console.WriteLine($"Removing {value} from {rowPos},{col}");
                    RemoveSquarePossibility(value, rowPos, col);
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
