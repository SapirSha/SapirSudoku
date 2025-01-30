using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapirSudoku.src.Exceptions
{
    public class InvalidBitSetSizeException : Exception
    {
        public InvalidBitSetSizeException() { }
        public InvalidBitSetSizeException(String msg) : base(msg) { }



    }
}
