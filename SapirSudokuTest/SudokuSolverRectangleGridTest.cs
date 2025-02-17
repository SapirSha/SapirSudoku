using SapirSudoku.src;

namespace SapirSudokuTest;

[TestClass]
public sealed class SudokuSolverRectangleGridTest
{
    [TestMethod]
    public void Should_SolveSudoku_When_GotA6X6ValidSudoku1()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {0 ,4 ,6 , 5 ,0 ,0 ,},
                {3 ,0 ,0 , 2 ,4 ,0 ,},

                {0 ,0 ,3 , 0 ,6 ,2 ,},
                {0 ,6 ,2 , 4 ,0 ,0 ,},

                {6 ,0 ,0 , 0 ,5 ,1 ,},
                {5 ,3 ,1 , 6 ,0 ,0 ,},
            }
        );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {2 ,4 ,6 , 5 ,1 ,3 ,},
                {3 ,1 ,5 , 2 ,4 ,6 ,},

                {4 ,5 ,3 , 1 ,6 ,2 ,},
                {1 ,6 ,2 , 4 ,3 ,5 ,},

                {6 ,2 ,4 , 3 ,5 ,1 ,},
                {5 ,3 ,1 , 6 ,2 ,4 ,},
            }
        );

        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }

    [TestMethod]
    public void Should_SolveSudoku_When_GotA6X6ValidSudoku2()
    {
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {1 ,0 ,0 , 0 ,6 ,0 ,},
                {0 ,5 ,3 , 0 ,1 ,2 ,},

                {0 ,0 ,2 , 1 ,5 ,0 ,},
                {0 ,4 ,1 , 0 ,2 ,3 ,},

                {2 ,3 ,6 , 0 ,0 ,1 ,},
                {4 ,1 ,5 , 2 ,0 ,6 ,},
            }
        );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {1 ,2 ,4 , 3 ,6 ,5 ,},
                {6 ,5 ,3 , 4 ,1 ,2 ,},

                {3 ,6 ,2 , 1 ,5 ,4 ,},
                {5 ,4 ,1 , 6 ,2 ,3 ,},

                {2 ,3 ,6 , 5 ,4 ,1 ,},
                {4 ,1 ,5 , 2 ,3 ,6 ,},
            }
        );

        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }

    [TestMethod]
    public void Should_SolveSudoku_When_GotA6X6ValidSudoku3()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {0 ,0 ,0 , 0 ,0 ,4 ,},
                {2 ,0 ,6 , 0 ,0 ,0 ,},

                {4 ,0 ,5 , 3 ,0 ,0 ,},
                {0 ,0 ,0 , 0 ,4 ,1 ,},

                {0 ,5 ,0 , 0 ,0 ,6 ,},
                {6 ,2 ,3 , 0 ,0 ,5 ,},
            }
        );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {5 ,3 ,1 , 6 ,2 ,4 ,},
                {2 ,4 ,6 , 1 ,5 ,3 ,},

                {4 ,1 ,5 , 3 ,6 ,2 ,},
                {3 ,6 ,2 , 5 ,4 ,1 ,},

                {1 ,5 ,4 , 2 ,3 ,6 ,},
                {6 ,2 ,3 , 4 ,1 ,5 ,},
            }
        );

        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }
    [TestMethod]
    public void Should_SolveSudoku_When_GotA6X6ValidSudoku4()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {0 ,2 ,0 , 0 ,0 ,1 ,},
                {5 ,0 ,0 , 0 ,0 ,0 ,},

                {0 ,5 ,0 , 0 ,0 ,6 ,},
                {0 ,0 ,0 , 0 ,3 ,0 ,},

                {3 ,0 ,5 , 1 ,0 ,0 ,},
                {6 ,0 ,0 , 0 ,0 ,0 ,},
            }
        );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {4 ,2 ,6 , 3 ,5 ,1 ,},
                {5 ,3 ,1 , 6 ,2 ,4 ,},

                {2 ,5 ,3 , 4 ,1 ,6 ,},
                {1 ,6 ,4 , 2 ,3 ,5 ,},

                {3 ,4 ,5 , 1 ,6 ,2 ,},
                {6 ,1 ,2 , 5 ,4 ,3 ,},
            }
        );

        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }
    [TestMethod]
    public void Should_SolveSudoku_When_GotA8X8ValidSudoku1()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {0 ,0 ,3 ,0 , 4 ,2 ,0 ,0 ,},
                {4 ,0 ,0 ,2 , 0 ,0 ,3 ,0 ,},

                {0 ,1 ,0 ,0 , 3 ,0 ,0 ,6 ,},
                {8 ,0 ,5 ,0 , 0 ,4 ,0 ,1 ,},

                {5 ,2 ,0 ,8 , 0 ,7 ,1 ,0 ,},
                {0 ,6 ,0 ,7 , 2 ,0 ,0 ,0 ,},

                {2 ,0 ,0 ,3 , 0 ,6 ,8 ,0 ,},
                {0 ,4 ,0 ,1 , 5 ,0 ,7 ,0 ,},
            }
        );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {1 ,8 ,3 ,5 , 4 ,2 ,6 ,7 ,},
                {4 ,7 ,6 ,2 , 8 ,1 ,3 ,5 ,},

                {7 ,1 ,2 ,4 , 3 ,8 ,5 ,6 ,},
                {8 ,3 ,5 ,6 , 7 ,4 ,2 ,1 ,},

                {5 ,2 ,4 ,8 , 6 ,7 ,1 ,3 ,},
                {3 ,6 ,1 ,7 , 2 ,5 ,4 ,8 ,},

                {2 ,5 ,7 ,3 , 1 ,6 ,8 ,4 ,},
                {6 ,4 ,8 ,1 , 5 ,3 ,7 ,2 ,},
            }
        );

        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }

    [TestMethod]
    public void Should_SolveSudoku_When_GotA8X8ValidSudoku2()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {0 ,0 ,3 ,0 , 4 ,2 ,0 ,0 ,},
                {4 ,0 ,0 ,2 , 0 ,0 ,3 ,0 ,},

                {0 ,1 ,0 ,0 , 3 ,0 ,0 ,6 ,},
                {8 ,0 ,5 ,0 , 0 ,4 ,0 ,1 ,},

                {5 ,2 ,0 ,8 , 0 ,7 ,1 ,0 ,},
                {0 ,6 ,0 ,7 , 2 ,0 ,0 ,0 ,},

                {2 ,0 ,0 ,3 , 0 ,6 ,8 ,0 ,},
                {0 ,4 ,0 ,1 , 5 ,0 ,7 ,0 ,},
            }
        );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {1 ,8 ,3 ,5 , 4 ,2 ,6 ,7 ,},
                {4 ,7 ,6 ,2 , 8 ,1 ,3 ,5 ,},

                {7 ,1 ,2 ,4 , 3 ,8 ,5 ,6 ,},
                {8 ,3 ,5 ,6 , 7 ,4 ,2 ,1 ,},

                {5 ,2 ,4 ,8 , 6 ,7 ,1 ,3 ,},
                {3 ,6 ,1 ,7 , 2 ,5 ,4 ,8 ,},

                {2 ,5 ,7 ,3 , 1 ,6 ,8 ,4 ,},
                {6 ,4 ,8 ,1 , 5 ,3 ,7 ,2 ,},
            }
        );

        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }
    [TestMethod]
    public void Should_SolveSudoku_When_GotA10X10ValidSudoku()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {0 ,0 ,0 ,4 ,8 , 7 ,0 ,6 ,3 ,1 ,},
                {2 ,0 ,0 ,0 ,6 , 0 ,10,0 ,0 ,4 ,},

                {0 ,1 ,0 ,9 ,0 , 2 ,3 ,0 ,0 ,6 ,},
                {3 ,0 ,0 ,0 ,10, 0 ,0 ,7 ,9 ,0 ,},

                {0 ,0 ,8 ,0 ,2 , 0 ,9 ,10 ,0 ,7 ,},
                {0 ,9 ,0 ,7 ,0 , 5 ,0 ,0 ,8 ,0 ,},

                {6 ,0 ,0 ,2 ,5 , 0 ,0 ,9 ,0 ,0 ,},
                {7 ,3 ,1 ,0 ,0 , 4 ,0 ,0 ,0 ,8 ,},

                {4 ,6 ,10,0 ,0 , 3 ,5 ,2 ,0 ,0 ,},
                {9 ,0 ,7 ,0 ,0 , 8 ,4 ,0 ,0 ,0 ,},
            }
        );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {5 ,10,9 ,4 ,8 , 7 ,2 ,6 ,3 ,1 ,},
                {2 ,7 ,3 ,1 ,6 , 9 ,10,8 ,5 ,4 ,},

                {8 ,1 ,5 ,9 ,7 , 2 ,3 ,4 ,10,6 ,},
                {3 ,4 ,2 ,6 ,10, 1 ,8 ,7 ,9 ,5 ,},

                {1 ,5 ,8 ,3 ,2 , 6 ,9 ,10,4 ,7 ,},
                {10,9 ,6 ,7 ,4 , 5 ,1 ,3 ,8 ,2 ,},

                {6 ,8 ,4 ,2 ,5 , 10,7 ,9 ,1 ,3 ,},
                {7 ,3 ,1 ,10,9 , 4 ,6 ,5 ,2 ,8 ,},

                {4 ,6 ,10,8 ,1 , 3 ,5 ,2 ,7 ,9 ,},
                {9 ,2 ,7 ,5 ,3 , 8 ,4 ,1 ,6 ,10,},
            }
        );

        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }
    [TestMethod]
    public void Should_SolveSudoku_When_GotA12X12ValidSudoku()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {0 ,10,8 ,2 , 1 ,0 ,0 ,9 , 0 ,12,0 ,4 ,},
                {0 ,0 ,7 ,3 , 8 ,2 ,0 ,0 , 1 ,0 ,11,0 ,},
                {11,9 ,0 ,0 , 0 ,6 ,7 ,0 , 0 ,0 ,2 ,0 ,},

                {6 ,0 ,0 ,0 , 9 ,0 ,12,0 , 0 ,7 ,3 ,0 ,},
                {0 ,1 ,4 ,0 , 0 ,0 ,6 ,7 , 0 ,11,12,5 ,},
                {3 ,0 ,0 ,12, 4 ,5 ,0 ,0 , 8 ,0 ,0 ,10,},

                {7 ,0 ,0 ,8 , 2 ,0 ,0 ,6 , 10,0 ,4 ,0 ,},
                {0 ,5 ,11,9 , 0 ,0 ,0 ,10, 0 ,3 ,6 ,0 ,},
                {1 ,0 ,0 ,0 , 12,3 ,0 ,0 , 0 ,9 ,0 ,2 ,},

                {0 ,2 ,12,7 , 0 ,1 ,0 ,0 , 0 ,5 ,8 ,0 ,},
                {10,0 ,9 ,0 , 0 ,0 ,0 ,8 , 11,0 ,1 ,0 ,},
                {0 ,0 ,6 ,0 , 5 ,7 ,9 ,4 , 3 ,0 ,0 ,0 ,},
            }
        );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {5 ,10,8 ,2 , 1 ,11,3 ,9 , 6 ,12,7 ,4 ,},
                {12,6 ,7 ,3 , 8 ,2 ,4 ,5 , 1 ,10,11,9 ,},
                {11,9 ,1 ,4 , 10,6 ,7 ,12, 5 ,8 ,2 ,3 ,},

                {6 ,8 ,5 ,11, 9 ,10,12,2 , 4 ,7 ,3 ,1 ,},
                {9 ,1 ,4 ,10, 3 ,8 ,6 ,7 , 2 ,11,12,5 ,},
                {3 ,7 ,2 ,12, 4 ,5 ,11,1 , 8 ,6 ,9 ,10,},

                {7 ,12,3 ,8 , 2 ,9 ,5 ,6 , 10,1 ,4 ,11,},
                {2 ,5 ,11,9 , 7 ,4 ,1 ,10, 12,3 ,6 ,8 ,},
                {1 ,4 ,10,6 , 12,3 ,8 ,11, 7 ,9 ,5 ,2 ,},

                {4 ,2 ,12,7 , 11,1 ,10,3 , 9 ,5 ,8 ,6 ,},
                {10,3 ,9 ,5 , 6 ,12,2 ,8 , 11,4 ,1 ,7 ,},
                {8 ,11,6 ,1 , 5 ,7 ,9 ,4 , 3 ,2 ,10,12,},
            }
        );
        
        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }
}
