using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using static procdiag.tests.RunUtility;

namespace procdiag.tests.integration
{
    [TestFixture]
    [Category("Integration")]
    public class MemoryDumpTests
    {
        [Test]
        public void X64_Process_MemoryDump()
        {
            // arrange
            using (var process = StartProcess("TestProcessX64.exe"))
            {
                EnsureNoDumpFilesArePresent(Environment.CurrentDirectory, process.Name);

                //act
                var result = Execute(new[] { $"-p {process.Id}" });

                AssertOutput(process.Name, result, "USERDU64");
            }
        }

        [Test]
        public void ShouldWrite_DumpFile_AtSpecifiedLocation()
        {
            // arrange
            using (var outFolder = new TemporaryFolder("New Folder"))
            using (var process = StartProcess("TestProcessX86.exe"))
            {
                EnsureNoDumpFilesArePresent(outFolder.FullName, process.Name);

                //act
                var result = Execute(new[] { $"-p {process.Id} --out=\"{outFolder.FullName}\" " });

                AssertOutput(process.Name, outFolder.FullName, result, "USERDUMP");
            }
        }

        [Test]
        public void X86_Process_MemoryDump()
        {
            // arrange
            using (var process = StartProcess("TestProcessX86.exe"))
            {
                EnsureNoDumpFilesArePresent(Environment.CurrentDirectory, process.Name);

                //act
                var result = Execute(new[] { $"-p {process.Id}" });

                AssertOutput(process.Name, result, "USERDUMP");
            }
        }

        private void EnsureNoDumpFilesArePresent(string outFolder, string process)
        {
            foreach (var dumpFile in GetDumpFiles(outFolder, process))
            {
                File.Delete(dumpFile);
            }
        }

        private static void AssertOutput(string process, string result, string dumpPrefix)
        {
            AssertOutput(process, Environment.CurrentDirectory, result, dumpPrefix);
        }

        private static void AssertOutput(string process, string outputFoler, string result, string dumpPrefix)
        {
            FileInfo fi = new FileInfo(GetDumpFiles(outputFoler, process).Single());

            try
            {
                AssertOutput(result);

                //TODO: A better way to validate the dump file
                var array = new byte[8];
                using (var fileStream = fi.OpenRead())
                    fileStream.Read(array, 0, 8);

                Assert.AreEqual(dumpPrefix, Encoding.Default.GetString(array));
            }
            finally
            {
                fi.Delete();
            }
        }

        private static string[] GetDumpFiles(string folder, string process)
        {
            return Directory.GetFiles(folder, $"{process}-dump-*.dmp");
        }

        private static void AssertOutput(string result)
        {
            StringAssert.Contains("Heap stats:", result);
            StringAssert.Contains("Size        Count Type", result);
            StringAssert.EndsWith("Heap stats finished." + Environment.NewLine, result);
        }

        private class TemporaryFolder : IDisposable
        {
            private readonly string _location;

            public TemporaryFolder(string location)
            {
                _location = Directory.CreateDirectory(location).FullName;
            }

            public string FullName => _location;

            public void Dispose()
            {
                Directory.Delete(_location);
            }
        }
    }
}