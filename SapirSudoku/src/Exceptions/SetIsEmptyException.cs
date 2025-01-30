using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapirSudoku.src.Exceptions
{
    public class SetIsEmptyException : Exception
    {
        public SetIsEmptyException() { }
        public SetIsEmptyException(String msg) : base(msg) { }
    }
}
