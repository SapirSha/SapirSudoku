using SapirSudoku;
using SapirSudoku.src;

namespace SapirSudokuTest
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void TestMethod1()
        {
            int i = 0;
            int x = 5 / i;
            Assert.AreEqual(1,1);
        }
    }
}
