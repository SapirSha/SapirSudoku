using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapirSudoku.src.Exceptions
{
    public class InvalidSudokuSizeException : Exception
    {
        public InvalidSudokuSizeException() { }
        public InvalidSudokuSizeException(string message) : base(message) { }
    }
}
