using System;

namespace ProcDiag
{
    public static class ConsoleMixins
    {
        public static void WriteLine(ConsoleColor color, string line, params object[] args)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(line, args);
            Console.ResetColor();
        }

        public static void WriteError(string line, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(line, args);
            Console.ResetColor();
        }
    }
}