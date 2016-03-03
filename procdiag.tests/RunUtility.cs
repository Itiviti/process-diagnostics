using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace procdiag.tests
{
    internal class RunUtility
    {
        public static Process StartProcess(string testExe)
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var processStartInfo = new ProcessStartInfo(Path.Combine(directoryName, testExe))
            {
                CreateNoWindow = true
            };

            return Process.Start(processStartInfo);
        }

        public static string Execute(Process process, string[] args)
        {
            string result;
            using (var memoryStream = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(memoryStream))
            using (StreamReader sr = new StreamReader(memoryStream))
            {
                Console.SetOut(sw);

                ProcDiag.Program.Main(args);
                Console.Out.Flush();

                process.Kill();
                memoryStream.Position = 0;
                result = sr.ReadToEnd();
            }
            return result;
        }
    }
}