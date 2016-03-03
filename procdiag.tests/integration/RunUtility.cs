using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace procdiag.tests
{
    internal class RunUtility
    {
        public static ProcessWrapper StartProcess(string testExe)
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var processStartInfo = new ProcessStartInfo(Path.Combine(directoryName, testExe))
            {
                CreateNoWindow = true
            };

            return new ProcessWrapper(Process.Start(processStartInfo));
        }

        public static string Execute(string[] args)
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var processStartInfo = new ProcessStartInfo(Path.Combine(directoryName, "procdiag.exe"), string.Join(" ", args))
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            var debugProcess = Process.Start(processStartInfo);
            return debugProcess.StandardOutput.ReadToEnd();
        }

        public class ProcessWrapper : IDisposable
        {
            private Process _proc;

            public ProcessWrapper(Process proc)
            {
                _proc = proc;
            }

            public string Name => _proc.ProcessName;

            public int Id => _proc.Id;

            public void Dispose()
            {
                if (!_proc.HasExited)
                    _proc.Kill();

                _proc.Dispose();
            }
        }
    }
}