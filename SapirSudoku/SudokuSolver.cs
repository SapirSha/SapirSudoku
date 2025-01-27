using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using CustomExceptions;
using SapirStruct;
using SapirSudoku;

namespace SapirSudoku
{
    public class SudokuSolver : Sudoku, IEnumerable<Sudoku>
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

        public Stack<Stack<(int row, int col)>> PrevInsertion;
        // FIRST IN PREV INSERTION FOR GUESSING
        // SECOND IN PREV INSERTION FOR INSERTIONS

        public Stack<Stack<Stack<(int value, int row, int col)>>> PrevAction;
        // FIRST IN PREV ACTION FOR GUESSING
        // SECOND IN PREV ACTION FOR INSERTIONS
        // THIRD FOR PREV ACTION FOR POSSIBILITY

        int size;
        int count;

        public SudokuSolver(Sudoku s) : this(s.Array, s.IsGridHorizontal()) { }

        public SudokuSolver(SudokuSolver solver)
        {
            this.sudoku = (int[,])solver.sudoku.Clone();
            this.grid_height = solver.grid_height;
            this.grid_width = solver.grid_width;
            this.full = new BitSet(solver.full);
            this.squarePossibilities = new BitSet[solver.sudoku.GetLength(0), solver.sudoku.GetLength(1)];
            for (int row = 0; row < solver.sudoku.GetLength(0); row++)
                for (int col = 0; col < solver.sudoku.GetLength(1); col++)
                    this.squarePossibilities[row, col] = new BitSet(solver.squarePossibilities[row, col]);
            this.squarePossibilitesCounter = new HashSet<(int, int)>[solver.sudoku.GetLength(0) + 1];
            for (int i = 0; i <= solver.sudoku.GetLength(0); i++)
                this.squarePossibilitesCounter[i] = new HashSet<(int, int)>(solver.sudoku.GetLength(0) * solver.sudoku.GetLength(1));
            for (int row = 0; row < solver.sudoku.GetLength(0); row++)
                for (int col = 0; col < solver.sudoku.GetLength(1); col++)
                    this.squarePossibilitesCounter[solver.sudoku.GetLength(0)].Add((row, col));
            this.rowAvailability = new BitSet[solver.sudoku.GetLength(0)];
            for (int i = 0; i < solver.sudoku.GetLength(0); i++)
                this.rowAvailability[i] = new BitSet(solver.rowAvailability[i]);
            this.rowAvailabilityCounter = new BitSet[solver.sudoku.GetLength(0)][];
            for (int i = 0; i < solver.sudoku.GetLength(0); i++)
            {
                this.rowAvailabilityCounter[i] = new BitSet[solver.sudoku.GetLength(0)];
                for (int j = 0; j < solver.sudoku.GetLength(0); j++)
                    this.rowAvailabilityCounter[i][j] = new BitSet(solver.rowAvailabilityCounter[i][j]);
            }
            this.colAvailability = new BitSet[solver.sudoku.GetLength(1)];
            for (int i = 0; i < solver.sudoku.GetLength(1); i++)
                this.colAvailability[i] = new BitSet(solver.colAvailability[i]);

            this.colAvailabilityCounter = new BitSet[solver.sudoku.GetLength(1)][];
            for (int i = 0; i < solver.sudoku.GetLength(1); i++)
            {
                this.colAvailabilityCounter[i] = new BitSet[solver.sudoku.GetLength(1)];
                for (int j = 0; j < solver.sudoku.GetLength(1); j++)
                    this.colAvailabilityCounter[i][j] = new BitSet(solver.colAvailabilityCounter[i][j]);
            }

            this.gridAvailability =
                        this.gridAvailability = new BitSet[solver.sudoku.GetLength(1)];
            for (int i = 0; i < solver.sudoku.GetLength(1); i++)
                this.gridAvailability[i] = new BitSet(solver.gridAvailability[i]);
            this.gridAvailabilityCounter = new BitSet[solver.sudoku.GetLength(1)][];
            for (int i = 0; i < solver.sudoku.GetLength(1); i++)
            {
                this.gridAvailabilityCounter[i] = new BitSet[solver.sudoku.GetLength(1)];
                for (int j = 0; j < solver.sudoku.GetLength(1); j++)
                    this.gridAvailabilityCounter[i][j] = new BitSet(solver.gridAvailabilityCounter[i][j]);
            }

            this.NextGarunteedAction = new Stack<(int value, int row, int col)>(solver.NextGarunteedAction);

            this.PrevAction = solver.PrevAction;
            this.PrevInsertion = solver.PrevInsertion;
        }


        public SudokuSolver(int length = 9, bool horizontal = true) : base(length, horizontal){
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
            this.PrevAction = new Stack<Stack<Stack<(int value, int row, int col)> > >(length);
            this.PrevInsertion = new Stack<Stack<(int row, int col)>>(length);
        }
        public SudokuSolver(int[,] grid, bool horizontal = true) : this(grid.GetLength(0), horizontal)
        {
            int length = grid.GetLength(0);
            if (grid.GetLength(1) != length)
                throw new InvalidSudokuException($"Sudoku size must be N*N, instead was {grid.GetLength(0)}*{grid.GetLength(1)}");

            PrevAction.Push(new Stack<Stack<(int value, int row, int col)>>(length));

            PrevInsertion.Push(new Stack<(int row, int col)>(length));

            for (int row = 0; row < sudoku.GetLength(0); row++)
                for (int col = 0; col < sudoku.GetLength(1); col++)
                    if (sudoku[row, col] == NONE && grid[row, col] != NONE)
                        Insert(grid[row, col], row, col);

            PrevAction.Clear();
            PrevInsertion.Clear();

            PrevAction.Push(new Stack<Stack<(int value, int row, int col)>>(length));
            PrevInsertion.Push(new Stack<(int row, int col)>(length));
            InsertGuranteed();
        }

        public void InsertGuranteed()
        {
            while (NextGarunteedAction.Count() != 0)
            {

                (int value, int row, int col) = NextGarunteedAction.Pop();
                if (sudoku[row, col] == 0)
                {
                    Insert(value, row, col);
                }
            }
        }

        public (int, int)? MinimumPossibilitySquare()
        {
            int i;
            for (i = 1; i <= sudoku.GetLength(0); i++)
                if (squarePossibilitesCounter[i].Count() != 0) break;
            return i <= sudoku.GetLength(0) ? squarePossibilitesCounter[i].First() : null;
        }

        public void RemoveLatestGuess()
        {
            
            if (PrevInsertion.Count() == 0 || PrevAction.Count() == 0) return;
            while (PrevInsertion.Peek().Count() != 0 && PrevAction.Peek().Count() != 0)
            {
                while (PrevAction.Peek().Peek().Count() != 0)
                {
                    (int valueAc, int rowAc, int colAc) = PrevAction.Peek().Peek().Pop();
                    AddPossibility(valueAc, rowAc, colAc);
                }
                (int rowIn, int colIn) = PrevInsertion.Peek().Pop();
                DeInsert(rowIn, colIn);

                PrevAction.Peek().Pop();
            }
            PrevAction.Pop();
            PrevInsertion.Pop();
            
        }


        public void AddPossibility(int value, int row, int col)
        {
            squarePossibilities[row, col].Add(value);
            int count = squarePossibilities[row, col].Count();

            squarePossibilitesCounter[count - 1].Remove((row, col));
            squarePossibilitesCounter[count].Add((row, col));

            rowAvailabilityCounter[row][value - 1].Add(col + 1);
            colAvailabilityCounter[col][value - 1].Add(row + 1);
            gridAvailabilityCounter[GridPositionOf(row, col)][value - 1].Add(row % grid_height * grid_width + col % grid_width + 1);
        }

        public void DeInsert(int row, int col)
        {
            int value = sudoku[row, col];
            if (value == NONE) return;
            rowAvailability[row].Add(value);
            colAvailability[col].Add(value);
            gridAvailability[GridPositionOf(row, col)].Add(value);
            AddPossibility(value, row, col);
            sudoku[row, col] = NONE;
            count--;
        }


        public override void Insert(int value, int row, int col)
        {
            if (!InRange(row,col))
                throw new ArgumentOutOfRangeException($"Row({row}) And Col({col}) Cannot Be Outside The Sudoku");
            
            if (!squarePossibilities[row,col].Contains(value))
                throw new InvalidInsertionException($"Cannot Insert '{value}' to {row},{col} in sudoku");

            PrevInsertion.Peek().Push((row, col));
            PrevAction.Peek().Push(new Stack<(int value, int row, int col)>());

            UpdateRowInsert(value, row, col);
            UpdateColInsert(value, row, col);
            UpdateGridInsert(value, row, col);
            UpdateSquareInsert(value, row, col);
            count++;

        }

        public void RemoveSquarePossibility(int value, int row, int col, bool changeRow = true, bool changeCol = true, bool changeGrid = true)
        {
            if (!squarePossibilities[row, col].Contains(value)) return;
            int count;

            PrevAction.Peek().Peek().Push((value, row, col));

            squarePossibilities[row, col].Remove(value);
            count = squarePossibilities[row, col].Count();
            if (count == 0) throw new InvalidInsertionException();
            else if (count == 1) PotentialInsertInSquare(row, col);

            squarePossibilitesCounter[count + 1].Remove((row, col));
            squarePossibilitesCounter[count].Add((row, col));

            if (changeRow)
            {
                rowAvailabilityCounter[row][value - 1].Remove(col + 1);
                count = rowAvailabilityCounter[row][value - 1].Count();
                if (count == 0) throw new InvalidInsertionException();
                else if (count == 1) PotentialInsertInRow(value, row);
                else
                {
                    if (count == 2) RowXWing(value, row, col);
                    if (count <= sudoku.GetLength(0) * LLOAD) HiddenInRow(value, row, col, count);
                }
            }

            if (changeCol)
            {
                colAvailabilityCounter[col][value - 1].Remove(row + 1);
                count = colAvailabilityCounter[col][value - 1].Count();
                if (count == 0) throw new InvalidInsertionException();
                else if (count == 1) PotentialInsertInCol(value, col);
                else
                {
                    if (count == 2) ColXWing(value, row, col);
                    if (count <= sudoku.GetLength(0) * LLOAD) HiddenInCol(value, row, col, count);
                }
            }

            if (changeGrid)
            {
                gridAvailabilityCounter[GridPositionOf(row, col)][value - 1].Remove(row % grid_height * grid_width + col % grid_width + 1);
                count = gridAvailabilityCounter[GridPositionOf(row, col)][value - 1].Count();
                if (count == 0) throw new InvalidInsertionException();
                else if (count == 1) PotentialInsertInGrid(value, GridPositionOf(row, col));
                else
                {
                    if (count <= grid_width || count <= grid_height) PointingPair(value, GridPositionOf(row, col));
                    if (count <= sudoku.GetLength(0) * LLOAD) HiddenInGrid(value, row, col, count);
                }
            }
        }

        private void UpdateSquareInsert(int value, int row, int col)
        {
            BitSet possible = squarePossibilities[row, col];

            foreach (int possibilite in possible)
                if (possibilite != value)
                    RemoveSquarePossibility(possibilite, row, col);

            squarePossibilitesCounter[possible.Count()].Remove((row, col));
            squarePossibilitesCounter[0].Add((row, col));

            rowAvailability[row].Remove(value);
            colAvailability[col].Remove(value);
            gridAvailability[GridPositionOf(row, col)].Remove(value);

            rowAvailabilityCounter[row][value - 1].ClearAll();
            colAvailabilityCounter[col][value - 1].ClearAll();
            gridAvailabilityCounter[GridPositionOf(row, col)][value - 1].ClearAll();

            squarePossibilities[row, col] = new BitSet(0);
            base.Insert(value, row, col);
        }

        private void UpdateRowInsert(int value, int row, int col)
        {
            for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
            {
                if (GridPositionOf(rowPos, col) == GridPositionOf(row, col)) continue;
                if (sudoku[rowPos, col] != NONE) continue;

                RemoveSquarePossibility(value, rowPos, col, true, false, true);
            }
        }

        private void UpdateColInsert(int value, int row, int col)
        {
            for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
            {
                if (GridPositionOf(row,colPos) == GridPositionOf(row,col)) continue;
                if (sudoku[row, colPos] != NONE) continue;

                RemoveSquarePossibility(value, row, colPos, false, true, true);
            }
        }

        private void UpdateGridInsert(int value, int row, int col)
        {
            int initRow = GridPositionOf(row, col) / (sudoku.GetLength(1) / grid_width) * grid_height;
            int initCol = GridPositionOf(row, col) * grid_width % sudoku.GetLength(1);
            for (int rowPos = 0; rowPos < grid_height; rowPos++)
            {
                for (int colPos = 0; colPos < grid_width; colPos++)
                {
                    if (initRow + rowPos == row && initCol + colPos == col) continue;
                    if (sudoku[initRow + rowPos, initCol + colPos] != NONE) continue;

                    RemoveSquarePossibility(value, initRow + rowPos, initCol + colPos, true, true, false);
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
            BitSet possibilities = squarePossibilities[row, col];
            NextGarunteedAction.Push((possibilities.GetSmallest(), row, col));
        }


        private void HiddenInGrid(int value, int row, int col, int hidden_type)
        {
            // THERE ARE X SQUARES WHERE V APPEARES IN THE GRID
            // IF THERE ARE X - 1 OTHER VALUES WHO ONLY APPEAR IN THE SAME GRIDS
            // HIDDEN VALUES IN GRID
            int grid = GridPositionOf(row,col);

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
                foreach (int position in possibilities_in_grid_for_value)
                {
                    row = initRow + (position - 1) / grid_width;
                    col = initCol + (position - 1) % grid_width;
                    foreach (int possibility_in_old in squarePossibilities[row, col])
                    {
                        if (Hidden.Contains(possibility_in_old)) continue;
                        RemoveSquarePossibility(possibility_in_old, row, col);
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
                foreach (int position in possibilities_in_row_for_value)
                {

                    col = position - 1;
                    foreach (int possibility_in_old in squarePossibilities[row, col])
                    {
                        if (Hidden.Contains(possibility_in_old)) continue;
                        RemoveSquarePossibility(possibility_in_old, row, col);
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
                foreach (int position in possibilities_in_col_for_value)
                {
                    row = position - 1;
                    foreach(int possibility_in_old in squarePossibilities[row, col])
                    {
                        if (Hidden.Contains(possibility_in_old)) continue;
                        RemoveSquarePossibility(possibility_in_old, row, col);
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
                int initRow = grid / (sudoku.GetLength(1) / grid_width) * grid_height;
                int row = initRow + (gridAvailabilityCounter[grid][value - 1].GetSmallest() - 1) / grid_width;
                for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
                {
                    if (GridPositionOf(row, colPos) == grid) continue;
                    RemoveSquarePossibility(value, row, colPos);
                }
            }
            else if (IsColPointingGroup(value, grid))
            {
                int initCol = grid * grid_width % sudoku.GetLength(1);
                int col = initCol + (gridAvailabilityCounter[grid][value - 1].GetSmallest() - 1) % grid_width;
                for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
                {
                    if (GridPositionOf(rowPos, col) == grid) continue;
                    RemoveSquarePossibility(value, rowPos, col);
                }
            }
        }

        int i = 0;
        public void RowXWing(int value, int row, int col)
        {
            if (rowAvailabilityCounter[row][value - 1].Count() == 2)
            {
                int col1 = rowAvailabilityCounter[row][value - 1].GetSmallest() - 1;
                int col2 = rowAvailabilityCounter[row][value - 1].GetLargest() - 1;
                int row2 = -1;
                foreach (int rowPos in colAvailabilityCounter[col1][value - 1])
                {
                    if (rowPos - 1 == row) continue;
                    if (rowAvailabilityCounter[rowPos - 1][value - 1].Equals(rowAvailabilityCounter[row][value - 1]))
                    {
                        row2 = rowPos - 1;
                        break;
                    }
                }

                if (row2 != -1)
                {
                    for (int rowPos = 0; rowPos < sudoku.GetLength(0); rowPos++)
                    {
                        if (rowPos == row || rowPos == row2) continue;

                        RemoveSquarePossibility(value, rowPos, col1);
                        RemoveSquarePossibility(value, rowPos, col2);
                    }
                }
            }
        }

        public void ColXWing(int value, int row, int col)
        {
            if (colAvailabilityCounter[col][value - 1].Count() == 2)
            {
                int row1 = colAvailabilityCounter[col][value - 1].GetSmallest() - 1;
                int row2 = colAvailabilityCounter[col][value - 1].GetLargest() - 1;
                int col2 = -1;
                foreach (int colPos in rowAvailabilityCounter[row1][value - 1])
                {
                    if (colPos - 1 == col) continue;
                    if (colAvailabilityCounter[colPos - 1][value - 1].Equals(colAvailabilityCounter[col][value - 1]))
                    {
                        col2 = colPos - 1;
                        break;
                    }
                }
                if (col2 != -1)
                {
                    for (int colPos = 0; colPos < sudoku.GetLength(1); colPos++)
                    {
                        if (colPos == col || colPos == col2) continue;

                        RemoveSquarePossibility(value, row1, colPos);
                        RemoveSquarePossibility(value, row2, colPos);
                    }
                }
            }

        }

        public override bool CanInsert(int value, int row, int col)
        {
            if (sudoku[row, col] == NONE || !InRange(row,col)) return true;
            return !BitSet.Union(rowAvailability[row], colAvailability[col], gridAvailability[GridPositionOf(row, col)]).Contains(value);
        }

        public bool IsSolved()
        {
            return size <= count;
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

        public IEnumerator<Sudoku> GetEnumerator()
        {
            (int, int)? min = MinimumPossibilitySquare();
            if (min == null)
            {
                if (IsSolved()) yield return new Sudoku(this);
                yield break;
            }

            (int row, int col) = (min.Value.Item1, min.Value.Item2);

            foreach (int possibility in squarePossibilities[row, col])
            {
                PrevAction.Push(new Stack<Stack<(int value, int row, int col)>>(sudoku.GetLength(0)));
                PrevInsertion.Push(new Stack<(int row, int col)>(sudoku.GetLength(0)));

                bool flag = true;

                try {
                    Insert(possibility, row, col);
                    InsertGuranteed();
                }
                catch (InvalidInsertionException)
                {
                    flag = false;
                    NextGarunteedAction.Clear();
                }

                if (flag)
                    foreach (Sudoku s in this)
                        yield return s;

                RemoveLatestGuess();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
