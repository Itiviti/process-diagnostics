using System;
using NUnit.Framework;
using static procdiag.tests.RunUtility;

namespace procdiag.tests.integration
{
    [TestFixture]
    [Category("Integration")]
    internal class MemoryStatsTests
    {
        [Test]
        public void X86_ShouldPrintMemoryStats()
        {
            // arrange
            using (var process = StartProcess("TestProcessX86.exe"))
            {
                //act
                var result = Execute(new[] { $"-p {process.Id} --stats" });

                AssertOutput(result);
            }
        }

        [Test]
        public void X64private_ShouldPrintMemoryStats()
        {
            // arrange
            using (var process = StartProcess("TestProcessX64.exe"))
            {
                //act
                var result = Execute(new[] { $"-p {process.Id} --stats" });

                AssertOutput(result);
            }
        }

        private static void AssertOutput(string result)
        {
            StringAssert.Contains("Heap stats:", result);
            StringAssert.Contains("Size        Count Type", result);
            StringAssert.EndsWith("Heap stats finished." + Environment.NewLine, result);
        }
    }
}