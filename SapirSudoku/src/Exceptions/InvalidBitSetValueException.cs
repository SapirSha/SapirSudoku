using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapirSudoku.src.Exceptions
{
    public class InvalidBitSetValueException : Exception
    {
        public InvalidBitSetValueException() { }
        public InvalidBitSetValueException(String msg) : base(msg) { }
    }
}
