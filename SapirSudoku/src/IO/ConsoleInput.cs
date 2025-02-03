using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SapirSudoku.src.Exceptions;

namespace SapirSudoku.src.IO
{
    public static class ConsoleInput
    {
        private static void ExitKeyHandler(object? sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
        }

        public static String? GetInput()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(ExitKeyHandler);

            try
            {
                return Console.ReadLine();
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static int? GetInputInt()
        {
            String? input = GetInput();

            if (input is null)
                return null;

            if (int.TryParse(input, out int n))
                return n;
            else return -1;

        }
    }
}
