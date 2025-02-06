using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapirSudoku.src.Exceptions
{
    public class EndOfInputException : Exception
    {
        public EndOfInputException() { }
        public EndOfInputException(String msg) : base(msg) { }
    }
}
