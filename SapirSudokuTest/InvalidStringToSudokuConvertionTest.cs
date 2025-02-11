using SapirSudoku.src.IO;
using SapirSudoku.src;
using SapirSudoku.src.Exceptions;

namespace SapirSudokuTest;

[TestClass]
public class InvalidStringToSudokuConvertionTest
{
    [TestMethod]
    [ExpectedException(typeof(InvalidSudokuSizeException))]
    public void Should_ThrowInvalidSudokuSizeException_When_LengthOfStringIsNotAPerfectRoot()
    {
        //Arrange
        String stringBoard = "00000000000000000";
        //Act
        //Should throw an exception
        Sudoku sudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
        //Assert
        Assert.Fail();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidSudokuSizeException))]
    public void Should_ThrowInvalidSudokuSizeException_When_LengthIsAboveValidRange()
    {
        //Arrange
        char[] longCharArray = new char[Sudoku.MAX_SUDOKU_LENGTH * Sudoku.MAX_SUDOKU_LENGTH + 1];
        String stringBoard = new String(longCharArray);
        //Act
        Sudoku sudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
        //Should throw an exception
        //Assert
        Assert.Fail();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidSudokuSizeException))]
    public void Should_ThrowInvalidSudokuSizeException_When_LengthIsBelowValidRange()
    {
        //Arrange
        String stringBoard = "";
        //Act
        Sudoku sudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
        //Should throw an exception
        //Assert
        Assert.Fail();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidValueException))]
    public void Should_ThrowInvalidValueException_When_InsertedOutOfRangeValue()
    {
        //Arrange
        // Inserting 5 to a 4X4 sudoku
        String stringBoard = "0005000000000000";
        //Act
        //Should throw an exception
        Sudoku sudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
        //Assert
        Assert.Fail();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidSudokuSizeException))]
    public void Should_ThrowInvalidSudokuSizeException_When_RootOfLengthIsAPrimeNumber()
    {
        //Arrange
        char[] longCharArray = new char[49];
        String stringBoard = new String(longCharArray);
        //Act
        //Should throw an exception
        Sudoku sudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
        //Assert
        Assert.Fail();
    }
}
