using SapirSudoku;
using SapirSudoku.src;
using SapirSudoku.src.Exceptions;
using SapirSudoku.src.IO;

namespace SapirSudokuTest
{
    [TestClass]
    public sealed class StringToSudokuConvertionTest
    {
        [TestMethod]
        public void Should_CreateSudoku_When_RecievedValidEmpty1X1StringBoard()
        {
            String stringBoard = "0";
            Sudoku gottenSudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
            Sudoku expectedSudoku = new Sudoku(
                new int[,]
                {
                    {0 }
                }
            );
            

            Assert.IsTrue(gottenSudoku.Equals(expectedSudoku));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSudokuSizeException))]
        public void Should_ThrowInvalidSudokuSizeException_When_RecievedNonePerfectRootStringLength()
        {
            String stringBoard = "000000000000000";
            Sudoku gottenSudoku = SudokuConvertionsHelper.ConvertStringToSudoku(stringBoard);
            Assert.Fail();
        }
    }
}
