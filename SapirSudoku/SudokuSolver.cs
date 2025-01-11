using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using CustomExceptions;

namespace SapirSudoku
{
    public class SudokuSolver : Sudoku
    {
        private HashSet<int>[][] avaliables;
        public LinkedList<(int value, int row, int col)> onlyOne;
        public int Size;
        public int Count;

        public SudokuSolver(SudokuSolver solver) : base((Sudoku)solver)
        {
            this.avaliables = (HashSet<int>[][])solver.avaliables.Clone();
            for (int i = 0; i < avaliables.Length; i++) {
                this.avaliables[i] = (HashSet<int>[])solver.avaliables[i].Clone();
                for (int j = 0; j < avaliables[i].Length; j++)
                {
                    this.avaliables[i][j] = new HashSet<int>(solver.avaliables[i][j]);
                }
            }

            this.onlyOne = solver.onlyOne;
            this.Size = solver.Size;
            this.Count = solver.Count;

        }
        public SudokuSolver(int length = 9) : base(length)
        {
            this.Size = length * length;
            this.Count = 0;
            avaliables = new HashSet<int>[sudoku.Length][];
            onlyOne = new LinkedList<(int, int, int)>();

            for (int row = 0; row < sudoku.Length; row++)
            {
                avaliables[row] = new HashSet<int>[sudoku[row].Length];
                for (int col = 0; col < sudoku[row].Length; col++)
                {
                    avaliables[row][col] = new HashSet<int>(ALLOWABLES.Count());
                    foreach (int i in ALLOWABLES)
                        if (i != NONE)
                            avaliables[row][col].Add(i);
                }
            }
        }
        public SudokuSolver(int[][] sudoku) : this(sudoku.Length) {
            if (sudoku.Length != sudoku[0].Length)
                throw new InvalidSudokuException("Cannot create a sudoku with different height and width");

            for (int row = 0; row < sudoku.Length; row++)
            {
                for (int col = 0; col < sudoku[0].Length; col++)
                {
                    if (sudoku[row][col] != NONE)
                        try
                        {
                            Insert(sudoku[row][col], row, col);
                        }
                        catch (InvalidInsertionException iie)
                        {
                            throw new InvalidSudokuException("Invalid Sudoku");
                        }
                }
            }
                        
        }
        public SudokuSolver(Sudoku sudoku) : this(sudoku.SudokuGrid) { }

        public new void Insert(int value, int row, int col)
        {
            if (sudoku[row][col] != NONE)
                throw new InvalidInsertionException($"Cannot insert {value} at {row},{col} because {sudoku[row][col]} is there");

            if (!avaliables[row][col].Contains(value))
                throw new InvalidInsertionException($"Cannot insert {value} at {row},{col} due to collisions");

            avaliables[row][col].Clear();

            base.Insert(value, row, col);
            this.Count++;

            for (int rowPos = 0; rowPos < sudoku.Length; rowPos++)
            {
                if (avaliables[rowPos][col].Contains(value))
                {
                    avaliables[rowPos][col].Remove(value);
                    if (avaliables[rowPos][col].Count() == 1)
                        onlyOne.AddFirst((avaliables[rowPos][col].Single(), rowPos, col));
                }
            }

            for (int colPos = 0; colPos < sudoku.Length; colPos++)
            {
                if (avaliables[row][colPos].Contains(value))
                {
                    avaliables[row][colPos].Remove(value);
                    if (avaliables[row][colPos].Count() == 1)
                        onlyOne.AddFirst((avaliables[row][colPos].Single(), row, colPos));
                }
            }

            int gridPos = (col / grid_width) + ((row / grid_height) * 3);
            int gridX = gridPos % (sudoku[0].Length / grid_width) * grid_width;
            int gridY = gridPos - (gridX / 3);

            for (int y = 0; y < grid_height; y++)
            {
                for (int x = 0; x < grid_width; x++)
                {
                    if (gridY + y == row || gridX + x == col)
                        continue;

                    if (avaliables[gridY + y][gridX + x].Contains(value))
                    {
                        avaliables[gridY + y][gridX + x].Remove(value);
                        if (avaliables[gridY + y][gridX + x].Count() == 1)
                            onlyOne.AddFirst((avaliables[gridY + y][gridX + x].Single(), gridY + y, gridX + x));

                    }
                }
            }
        }

        public bool IsSolved()
        {
            return Size == Count /*&& base.IsValid()*/;
        }

        public IEnumerable<Sudoku> NextSolve()
        {
            while (onlyOne.Count != 0)
            {
                (int value, int row, int col) nextInsertion = onlyOne.First();
                onlyOne.RemoveFirst();
                try
                {
                    Insert(nextInsertion.value, nextInsertion.row, nextInsertion.col);
                }
                catch (InvalidInsertionException)
                {
                }
            }

            if (IsSolved())
                yield return this;
            else
            {

                int index;
                int min_index = -1;
                for (index = 0; index < Size; index++)
                    if (sudoku[index / sudoku.Length][index % sudoku.Length] == 0)
                        if (avaliables[index / sudoku.Length][index % sudoku.Length].Count() == 2)
                        {
                            min_index = index;
                            break;
                        }
                        else if (min_index == -1 || avaliables[index / sudoku.Length][index % sudoku.Length].Count() < avaliables[min_index / sudoku.Length][min_index % sudoku.Length].Count())
                        {
                            min_index = index;
                        }


                int row = min_index / sudoku.Length;
                int col = min_index % sudoku.Length;

                foreach (int all in avaliables[row][col])
                {
                    SudokuSolver solver = new SudokuSolver(this);
                    solver.Insert(all, row, col);
                    foreach (Sudoku s in solver.NextSolve())
                    {
                        yield return s;
                    }
                }

            }

        }




    }
}
