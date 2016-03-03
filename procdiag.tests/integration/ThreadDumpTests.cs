using System;
using NUnit.Framework;
using static procdiag.tests.RunUtility;

namespace procdiag.tests.integration
{
    [TestFixture]
    [Category("Integration")]
    public class ThreadDumpTests
    {
        [Test]
        public void X64_Process_ThreadDump()
        {
            // arrange
            using (var process = StartProcess("TestProcessX64.exe"))
            {
                //act
                var result = Execute(new[] { $"-p {process.Id}", "--threadsonly" });

                //assert
                AssertThreadDump(result, "TestProcessX64");
            }
        }

        [Test]
        public void X86_Process_ThreadDump()
        {
            // arrange
            using (var process = StartProcess("TestProcessX86.exe"))
            {
                //act
                var result = Execute(new[] { $"-p {process.Id}", "--threadsonly" });

                //assert
                AssertThreadDump(result, "TestProcessX86");
            }
        }

        private static void AssertThreadDump(string result, string processName)
        {
            StringAssert.StartsWith("Thread dump:", result);
            StringAssert.Contains($"{processName}.Program.Main(System.String[])", result);
            StringAssert.Contains("System.Console.ReadLine()", result);
            StringAssert.EndsWith("Thread dump finished." + Environment.NewLine, result);
        }
    }
}