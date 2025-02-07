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
        String stringBoard = "00000000000000000";
        Sudoku sudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
        //Should throw an exception
        Assert.Fail();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidSudokuSizeException))]
    public void Should_ThrowInvalidSudokuSizeException_When_LengthIsAboveValidRange()
    {
        char[] longCharArray = new char[Sudoku.MAX_SUDOKU_LENGTH * Sudoku.MAX_SUDOKU_LENGTH + 1];
        String stringBoard = new String(longCharArray);
        Sudoku sudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
        //Should throw an exception
        Assert.Fail();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidSudokuSizeException))]
    public void Should_ThrowInvalidSudokuSizeException_When_LengthIsBelowValidRange()
    {
        String stringBoard = "";
        Sudoku sudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
        //Should throw an exception
        Assert.Fail();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidValueException))]
    public void Should_ThrowInvalidValueException_When_InsertedOutOfRangeValue()
    {
        String stringBoard = "0005000000000000";
        Sudoku sudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
        //Should throw an exception
        Assert.Fail();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidSudokuSizeException))]
    public void Should_ThrowInvalidSudokuSizeException_When_RootOfLengthIsAPrimeNumber()
    {
        char[] longCharArray = new char[49];
        String stringBoard = new String(longCharArray);
        Sudoku sudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
        //Should throw an exception
        Assert.Fail();
    }
}
