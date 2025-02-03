using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapirSudoku.src.IO
{
    public static class FileInput
    {
        public static IEnumerable<String>? GetLines(String pathFile)
        {
            try
            {
                String[] Lines = File.ReadAllLines(pathFile);
                return Lines;
            }
            catch (Exception) 
            { 
                return null;
            }
        }

    }
}
