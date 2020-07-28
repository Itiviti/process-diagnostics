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
                var result = Execute(new[] { $"-p {process.Id}", "--threads" });

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
                var result = Execute(new[] { $"-p {process.Id}", "--threads" });

                //assert
                AssertThreadDump(result, "TestProcessX86");
            }
        }

        private static void AssertThreadDump(string result, string processName)
        {
            var cleanResult = result.TrimEnd(' ', '\r', '\n' );
            StringAssert.StartsWith("Thread dump:", cleanResult);
            StringAssert.Contains($"{processName}.Program.Main(System.String[])", cleanResult);
            StringAssert.Contains("System.Console.ReadLine()", cleanResult);
            StringAssert.EndsWith("Thread dump finished.", cleanResult);
        }
    }
}
