using SapirSudoku.src;

namespace SapirSudokuTest;

[TestClass]
public class SudokuSolverSingleAnswerTest9X9
{
    [TestMethod]
    public void Should_SolveSudoku_When_GotA9X9ValidSudoku1()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {8,0,0, 0,0,0, 0,0,0},
                {0,0,3, 6,0,0, 0,0,0},
                {0,7,0, 0,9,0, 2,0,0},

                {0,5,0, 0,0,7, 0,0,0},
                {0,0,0, 0,4,5, 7,0,0},
                {0,0,0, 1,0,0, 0,3,0},

                {0,0,1, 0,0,0, 0,6,8},
                {0,0,8, 5,0,0, 0,1,0},
                {0,9,0, 0,0,0, 4,0,0}
            }
            );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {8,1,2, 7,5,3, 6,4,9},
                {9,4,3, 6,8,2, 1,7,5},
                {6,7,5, 4,9,1, 2,8,3},

                {1,5,4, 2,3,7, 8,9,6},
                {3,6,9, 8,4,5, 7,2,1},
                {2,8,7, 1,6,9, 5,3,4},

                {5,2,1, 9,7,4, 3,6,8},
                {4,3,8, 5,2,6, 9,1,7},
                {7,9,6, 3,1,8, 4,5,2},
            }
        );

        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }
    [TestMethod]
    public void Should_SolveSudoku_When_GotA9X9ValidSudoku2()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {5,3,0, 0,7,0, 0,0,0 },
                {6,0,0, 1,9,5, 0,0,0 },
                {0,9,8, 0,0,0, 0,6,0 },

                {8,0,0, 0,6,0, 0,0,3 },
                {4,0,0, 8,0,3, 0,0,1 },
                {7,0,0, 0,2,0, 0,0,6 },

                {0,6,0, 0,0,0, 2,8,0 },
                {0,0,0, 4,1,9, 0,0,5 },
                {0,0,0, 0,8,0, 0,7,9 }
            }
            );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {5,3,4, 6,7,8, 9,1,2,},
                {6,7,2, 1,9,5, 3,4,8,},
                {1,9,8, 3,4,2, 5,6,7,},

                {8,5,9, 7,6,1, 4,2,3,},
                {4,2,6, 8,5,3, 7,9,1,},
                {7,1,3, 9,2,4, 8,5,6,},

                {9,6,1, 5,3,7, 2,8,4,},
                {2,8,7, 4,1,9, 6,3,5,},
                {3,4,5, 2,8,6, 1,7,9,}
            }
        );

        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }

    [TestMethod]
    public void Should_SolveSudoku_When_GotA9X9ValidSudoku3()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {5,0,7, 0,0,0, 0,3,0,},
                {0,0,0, 0,6,1, 0,0,0,},
                {1,0,8, 0,0,0, 0,0,0,},

                {6,2,0, 0,4,0, 0,0,0,},
                {0,0,0, 7,0,0, 0,8,0,},
                {0,0,0, 0,0,0, 0,0,0,},

                {0,1,0, 0,0,0, 6,0,4,},
                {3,0,0, 5,0,0, 0,0,0,},
                {0,0,0, 0,0,0, 2,0,0,},
            }
            );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {5,6,7, 4,9,2, 1,3,8,},
                {2,3,9, 8,6,1, 7,4,5,},
                {1,4,8, 3,7,5, 9,2,6,},

                {6,2,3, 1,4,8, 5,7,9,},
                {9,5,1, 7,3,6, 4,8,2,},
                {8,7,4, 2,5,9, 3,6,1,},

                {7,1,2, 9,8,3, 6,5,4,},
                {3,9,6, 5,2,4, 8,1,7,},
                {4,8,5, 6,1,7, 2,9,3,},
            }
        );

        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }

    [TestMethod]
    public void Should_SolveSudoku_When_GotA9X9ValidSudoku4()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {9,0,0, 8,0,0, 0,0,0,},
                {0,0,0, 0,0,0, 5,0,0,},
                {0,0,0, 0,0,0, 0,0,0,},

                {0,2,0, 0,1,0, 0,0,3,},
                {0,1,0, 0,0,0, 0,6,0,},
                {0,0,0, 4,0,0, 0,7,0,},

                {7,0,8, 6,0,0, 0,0,0,},
                {0,0,0, 0,3,0, 1,0,0,},
                {4,0,0, 0,0,0, 2,0,0,},
            }
            );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {9,7,2, 8,5,3, 6,1,4,},
                {1,4,6, 2,7,9, 5,3,8,},
                {5,8,3, 1,4,6, 7,2,9,},

                {6,2,4, 7,1,8, 9,5,3,},
                {8,1,7, 3,9,5, 4,6,2,},
                {3,5,9, 4,6,2, 8,7,1,},

                {7,9,8, 6,2,1, 3,4,5,},
                {2,6,5, 9,3,4, 1,8,7,},
                {4,3,1, 5,8,7, 2,9,6,},
            }
        );

        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }

    [TestMethod]
    public void Should_SolveSudoku_When_GotA9X9ValidSudoku5()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {5,2,8, 6,0,0, 0,4,9,},
                {1,3,6, 4,9,0, 0,2,5,},
                {7,9,4, 2,0,5, 6,3,0,},

                {0,0,0, 1,0,0, 2,0,0,},
                {0,0,7, 8,2,6, 3,0,0,},
                {0,0,2, 5,0,9, 0,6,0,},

                {2,4,0, 3,0,0, 9,7,6,},
                {8,0,9, 7,0,2, 4,1,3,},
                {0,7,0, 9,0,4, 5,8,2,},
            }
            );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {5,2,8, 6,3,1, 7,4,9,},
                {1,3,6, 4,9,7, 8,2,5,},
                {7,9,4, 2,8,5, 6,3,1,},

                {4,6,5, 1,7,3, 2,9,8,},
                {9,1,7, 8,2,6, 3,5,4,},
                {3,8,2, 5,4,9, 1,6,7,},

                {2,4,1, 3,5,8, 9,7,6,},
                {8,5,9, 7,6,2, 4,1,3,},
                {6,7,3, 9,1,4, 5,8,2,},
            }
        );

        // Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        // Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }

    [TestMethod]
    public void Should_SolveSudoku_When_GotA9X9ValidSudoku6()
    {
        // Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {3,7,0, 4,0,8, 1,0,0,},
                {0,0,0, 9,0,3, 7,0,4,},
                {9,4,0, 1,0,0, 0,8,3,},

                {4,2,0, 0,0,0, 0,0,5,},
                {0,0,0, 5,0,4, 0,0,0,},
                {8,0,0, 0,0,0, 0,4,6,},

                {0,1,0, 0,4,9, 0,0,0,},
                {5,0,9, 6,0,0, 4,0,0,},
                {0,0,4, 2,0,0, 9,3,1,},
            }
            );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {3,7,2, 4,5,8, 1,6,9,},
                {1,5,8, 9,6,3, 7,2,4,},
                {9,4,6, 1,7,2, 5,8,3,},

                {4,2,7, 8,9,6, 3,1,5,},
                {6,3,1, 5,2,4, 8,9,7,},
                {8,9,5, 3,1,7, 2,4,6,},

                {2,1,3, 7,4,9, 6,5,8,},
                {5,8,9, 6,3,1, 4,7,2,},
                {7,6,4, 2,8,5, 9,3,1,},
            }
        );
        
        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }

    [TestMethod]
    public void Should_SolveSudoku_When_GotA9X9ValidSudoku7()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {9,0,3, 4,0,0, 6,7,0,},
                {7,1,2, 6,5,8, 9,4,3,},
                {0,6,4, 7,3,9, 0,0,2,},

                {0,3,0, 0,4,7, 5,0,9,},
                {0,0,0, 0,9,0, 3,0,0,},
                {0,0,9, 3,0,0, 0,8,0,},

                {4,0,0, 0,6,3, 0,9,0,},
                {0,9,0, 2,7,0, 4,3,6,},
                {3,0,6, 9,8,4, 7,0,1,},
            }
            );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {9,8,3, 4,1,2, 6,7,5,},
                {7,1,2, 6,5,8, 9,4,3,},
                {5,6,4, 7,3,9, 8,1,2,},

                {2,3,8, 1,4,7, 5,6,9,},
                {1,5,7, 8,9,6, 3,2,4,},
                {6,4,9, 3,2,5, 1,8,7,},

                {4,7,1, 5,6,3, 2,9,8,},
                {8,9,5, 2,7,1, 4,3,6,},
                {3,2,6, 9,8,4, 7,5,1,}
            }
        );
        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;
        
        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }

    [TestMethod]
    public void Should_SolveSudoku_When_GotA9X9ValidSudoku8()
    {
        //Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {0,3,0, 0,0,0, 0,1,0,},
                {0,0,8, 0,9,0, 0,0,0,},
                {4,0,0, 6,0,8, 0,0,0,},

                {0,0,0, 5,7,6, 9,4,0,},
                {0,0,0, 9,8,3, 5,2,0,},
                {0,0,0, 1,2,4, 0,0,0,},

                {2,7,6, 0,0,5, 1,9,0,},
                {0,0,0, 7,0,9, 0,0,0,},
                {0,9,5, 0,0,0, 4,7,0,}
            }
            );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {6,3,9, 2,4,7, 8,1,5,},
                {5,2,8, 3,9,1, 7,6,4,},
                {4,1,7, 6,5,8, 2,3,9,},

                {1,8,2, 5,7,6, 9,4,3,},
                {7,6,4, 9,8,3, 5,2,1,},
                {9,5,3, 1,2,4, 6,8,7,},

                {2,7,6, 4,3,5, 1,9,8,},
                {8,4,1, 7,6,9, 3,5,2,},
                {3,9,5, 8,1,2, 4,7,6,},
            }
        );
        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }

    [TestMethod]
    public void Should_SolveSudoku_When_GotA9X9ValidSudoku9()
    {
        // Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {0,0,2, 0,8,5, 0,0,4,},
                {0,0,0, 0,3,0, 0,6,0,},
                {0,0,4, 2,1,0, 0,3,0,},

                {0,0,0, 0,0,0, 0,5,2,},
                {0,0,0, 0,0,0, 3,1,0,},
                {9,0,0, 0,0,0, 0,0,0,},

                {8,0,0, 0,0,6, 0,0,0,},
                {2,5,0, 4,0,0, 0,0,8,},
                {0,0,0, 0,0,1, 6,0,0,},
            }
            );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {1,3,2, 6,8,5, 9,7,4,},
                {5,9,8, 7,3,4, 2,6,1,},
                {7,6,4, 2,1,9, 8,3,5,},

                {6,8,3, 1,9,7, 4,5,2,},
                {4,2,7, 5,6,8, 3,1,9,},
                {9,1,5, 3,4,2, 7,8,6,},

                {8,7,1, 9,2,6, 5,4,3,},
                {2,5,6, 4,7,3, 1,9,8,},
                {3,4,9, 8,5,1, 6,2,7,},
            }
        );

        //Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }
    [TestMethod]
    public void Should_SolveSudoku_When_GotA9X9ValidSudoku10()
    {
        // Arrange
        Sudoku toSolveSudoku = new Sudoku(
            new int[,]
            {
                {0,0,3, 8,0,0, 5,1,0,},
                {0,0,8, 7,0,0, 9,3,0,},
                {1,0,0, 3,0,5, 7,2,8,},

                {0,0,0, 2,0,0, 8,4,9,},
                {8,0,1, 9,0,6, 2,5,7,},
                {0,0,0, 5,0,0, 1,6,3,},

                {9,6,4, 1,2,7, 3,8,5,},
                {3,8,2, 6,5,9, 4,7,1,},
                {0,1,0, 4,0,0, 6,9,2,},
            }
            );

        Sudoku expectedSudoku = new Sudoku(
            new int[,]
            {
                {7,9,3, 8,6,2, 5,1,4,},
                {2,5,8, 7,1,4, 9,3,6,},
                {1,4,6, 3,9,5, 7,2,8,},

                {6,7,5, 2,3,1, 8,4,9,},
                {8,3,1, 9,4,6, 2,5,7,},
                {4,2,9, 5,7,8, 1,6,3,},

                {9,6,4, 1,2,7, 3,8,5,},
                {3,8,2, 6,5,9, 4,7,1,},
                {5,1,7, 4,8,3, 6,9,2,},
            }
        );
        // Act
        IEnumerable<Sudoku> Answers = toSolveSudoku.Answers;

        //Assert
        Assert.IsTrue(Answers.First().Equals(expectedSudoku) && Answers.Count() == 1);
    }
}
