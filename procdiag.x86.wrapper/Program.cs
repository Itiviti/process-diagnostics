using System;
using System.IO;
using System.Reflection;

namespace procdiag.x86
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"procdiag.exe");
            AppDomain.CurrentDomain.ExecuteAssembly(file, args);
        }
    }
}