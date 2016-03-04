using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ProcDiag
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Options options = new Options();

            if (!CommandLine.Parser.Default.ParseArguments(args, options))
                return;

            var process = Process.GetProcessById(options.ProcessId);
            if (RedirectToX86(process, args, Console.Out, Console.Error))
                return;

            Dumper.Start(options, process, Console.Out);
        }

        private static bool RedirectToX86(Process process, string[] args, TextWriter outWriter, TextWriter errorWriter)
        {
            if (IntPtr.Size != 8 || !IsWin64Emulator(process)) return false;

            string x86Wrapper = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "procdiag.x86.exe");
            using (new TemporaryFile(x86Wrapper, Resources.procdiag_x86))
            {
                var processStartInfo = new ProcessStartInfo(x86Wrapper, Environment.CommandLine)
                {
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                var wrapperProcess = Process.Start(processStartInfo);
                outWriter.Write(wrapperProcess.StandardOutput.ReadToEnd());
                errorWriter.Write(wrapperProcess.StandardError.ReadToEnd());
                wrapperProcess.WaitForExit();
            }

            return true;
        }

        private static bool IsWin64Emulator(Process process)
        {
            if ((Environment.OSVersion.Version.Major > 5)
                || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1)))
            {
                bool retVal;

                return NativeMethods.IsWow64Process(process.Handle, out retVal) && retVal;
            }

            return false; // not on 64-bit Windows Emulator
        }

        internal static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);
        }
    }
}