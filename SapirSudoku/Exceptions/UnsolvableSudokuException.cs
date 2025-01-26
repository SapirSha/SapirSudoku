using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExceptions
{
    internal class UnsolvableSudokuException : Exception
    {
        public UnsolvableSudokuException() { }
        public UnsolvableSudokuException(string message) : base(message) { }


    }
}
