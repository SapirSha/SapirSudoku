using System;
using System.Collections.Generic;

namespace CustomExceptions
{
    internal class HeapException : Exception
    {
        public HeapException() { }
        public HeapException(string message) : base(message) { }
    }
}
